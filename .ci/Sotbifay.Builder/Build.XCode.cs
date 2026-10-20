using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Serilog;

public partial class Build
{
    [Parameter("iOS APS environment (development/production)")] 
    readonly string IOSApsEnvironment = "production";

    [Parameter("iOS Xcode scheme")]
    readonly string IOSXcodeScheme = "Unity-iPhone";

    [Parameter("iOS Xcode build configuration")]
    readonly string IOSXcodeConfiguration = "Release";

    [Parameter("iOS export method")]
    readonly string IOSExportMethod = "app-store-connect";

    [Parameter("Allow Xcode provisioning updates")]
    readonly bool IOSAllowProvisioningUpdates = true;

    [Parameter("App Store Connect API key id"), Secret]
    readonly string AppStoreConnectApiKeyId = string.Empty;

    [Parameter("App Store Connect API issuer id"), Secret]
    readonly string AppStoreConnectApiIssuerId = string.Empty;

    static AbsolutePath IOSArtifactsDirectory => ArtifactsDirectory / "ios";
    static AbsolutePath IOSArchivePath => IOSArtifactsDirectory / "Unity-iPhone.xcarchive";
    static AbsolutePath IOSExportDirectory => IOSArtifactsDirectory / "ipa";
    static AbsolutePath IOSExportOptionsPlistPath => IOSArtifactsDirectory / "ExportOptions.plist";
    
    Target BuildIOSXcode => target => target
        .DependsOn(BuildIOSUnity)
        .Before(RestoreSettingFiles)
        .Executes(() =>
        {
            var xcodeExportDirectory = FindXcodeExportDirectory();

            PatchInfoPlist(xcodeExportDirectory / "Info.plist");
            PatchEntitlements(xcodeExportDirectory / "Unity-iPhone.entitlements");
            PatchXcodeFrameworks(xcodeExportDirectory);
        });

    Target InstallIOSPods => target => target
        .DependsOn(BuildIOSXcode)
        .Before(RestoreSettingFiles)
        .Executes(() =>
        {
            var xcodeExportDirectory = FindXcodeExportDirectory();

            ProcessTasks.StartProcess("pod", "install", workingDirectory: xcodeExportDirectory)
                .AssertZeroExitCode();
        });

    Target ValidateXcodeInstallation => target => target
        .Before(ArchiveIOSXcode)
        .Executes(() =>
        {
            var developerDirectoryProcess = ProcessTasks.StartProcess("xcode-select", "-p")
                .AssertZeroExitCode();

            var developerDirectory = developerDirectoryProcess.Output
                .Select(output => output.Text)
                .FirstOrDefault(text => string.IsNullOrWhiteSpace(text) == false)
                ?.Trim() ?? string.Empty;

            if (developerDirectory.Contains("CommandLineTools"))
            {
                throw new InvalidOperationException(
                    "Full Xcode is required to archive/export iOS IPA, but active developer directory points to CommandLineTools: " +
                    $"'{developerDirectory}'. Run: sudo xcode-select -s /Applications/Xcode.app/Contents/Developer");
            }

            ProcessTasks.StartProcess("xcodebuild", "-version")
                .AssertZeroExitCode();

            Log.Information("Using Xcode developer directory: {DeveloperDirectory}", developerDirectory);
        });

    Target ArchiveIOSXcode => target => target
        .DependsOn(InstallIOSPods)
        .DependsOn(ValidateXcodeInstallation)
        .Before(RestoreSettingFiles)
        .Executes(() =>
        {
            IOSArtifactsDirectory.CreateDirectory();

            var xcodeExportDirectory = FindXcodeExportDirectory();
            var workspacePath = FindXcodeWorkspacePath(xcodeExportDirectory);
            var arguments = new StringBuilder()
                .Append("archive")
                .Append($" -workspace {Quote(workspacePath)}")
                .Append($" -scheme {Quote(IOSXcodeScheme)}")
                .Append($" -configuration {Quote(IOSXcodeConfiguration)}")
                .Append($" -archivePath {Quote(IOSArchivePath)}")
                .Append(" -destination generic/platform=iOS")
                .Append(" -sdk iphoneos")
                .Append($" DEVELOPMENT_TEAM={Quote(IOSTeamID)}");

            if (IOSAllowProvisioningUpdates)
                arguments.Append(" -allowProvisioningUpdates");

            ProcessTasks.StartProcess("xcodebuild", arguments.ToString(), workingDirectory: xcodeExportDirectory)
                .AssertZeroExitCode();
        });

    Target ExportIOSIpa => target => target
        .DependsOn(ArchiveIOSXcode)
        .Before(RestoreSettingFiles)
        .Executes(() =>
        {
            IOSExportDirectory.CreateDirectory();
            WriteIOSExportOptionsPlist(IOSExportOptionsPlistPath);

            var xcodeExportDirectory = FindXcodeExportDirectory();
            var arguments = new StringBuilder()
                .Append("-exportArchive")
                .Append($" -archivePath {Quote(IOSArchivePath)}")
                .Append($" -exportPath {Quote(IOSExportDirectory)}")
                .Append($" -exportOptionsPlist {Quote(IOSExportOptionsPlistPath)}");

            if (IOSAllowProvisioningUpdates)
                arguments.Append(" -allowProvisioningUpdates");

            ProcessTasks.StartProcess("xcodebuild", arguments.ToString(), workingDirectory: xcodeExportDirectory)
                .AssertZeroExitCode();
        });

    Target UploadIOSToTestFlight => target => target
        .DependsOn(ExportIOSIpa)
        .Before(RestoreSettingFiles)
        .Executes(() =>
        {
            if (string.IsNullOrWhiteSpace(AppStoreConnectApiKeyId))
                throw new InvalidOperationException("App Store Connect API key id is required for TestFlight upload.");

            if (string.IsNullOrWhiteSpace(AppStoreConnectApiIssuerId))
                throw new InvalidOperationException("App Store Connect API issuer id is required for TestFlight upload.");

            var ipaPath = FindIOSIpaPath();
            var arguments = new StringBuilder()
                .Append("altool --upload-app")
                .Append(" --type ios")
                .Append($" --file {Quote(ipaPath)}")
                .Append($" --apiKey {Quote(AppStoreConnectApiKeyId)}")
                .Append($" --apiIssuer {Quote(AppStoreConnectApiIssuerId)}");

            ProcessTasks.StartProcess("xcrun", arguments.ToString())
                .AssertZeroExitCode();
        });

    private static AbsolutePath FindXcodeWorkspacePath(AbsolutePath xcodeExportDirectory)
    {
        var workspacePath = xcodeExportDirectory
            .GlobDirectories("*.xcworkspace")
            .FirstOrDefault();

        return workspacePath == null
            ? throw new FileNotFoundException($"Xcode workspace was not found in Xcode export directory: {xcodeExportDirectory}. Run pod install before archive.")
            : workspacePath;
    }

    private static AbsolutePath FindIOSIpaPath()
    {
        var ipaPath = IOSExportDirectory
            .GlobFiles("*.ipa")
            .OrderByDescending(GetLastWriteTimeUtc)
            .FirstOrDefault();

        return ipaPath == null
            ? throw new FileNotFoundException($"IPA file was not found in iOS export directory: {IOSExportDirectory}")
            : ipaPath;
    }

    private void WriteIOSExportOptionsPlist(AbsolutePath path)
    {
        var document = new XDocument(
            new XElement("plist",
                new XAttribute("version", "1.0"),
                new XElement("dict",
                    new XElement("key", "method"),
                    new XElement("string", IOSExportMethod),
                    new XElement("key", "teamID"),
                    new XElement("string", IOSTeamID),
                    new XElement("key", "signingStyle"),
                    new XElement("string", "automatic"),
                    new XElement("key", "stripSwiftSymbols"),
                    new XElement("true"),
                    new XElement("key", "uploadBitcode"),
                    new XElement("false"),
                    new XElement("key", "uploadSymbols"),
                    new XElement("true"))));

        SavePlist(document, path);
    }

    private static string Quote(object value)
    {
        return $"\"{value}\"";
    }
}