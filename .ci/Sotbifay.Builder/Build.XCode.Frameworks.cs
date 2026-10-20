using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Nuke.Common.IO;

public partial class Build
{
    private const string IOSMainTargetName = "Unity-iPhone";
    private const string IOSEntitlementsFileName = "Unity-iPhone.entitlements";
    private const string IOSUserNotificationsFrameworkName = "UserNotifications.framework";

    private static void PatchXcodeFrameworks(AbsolutePath xcodeExportDirectory)
    {
        var pbxprojPath = FindPbxprojPath(xcodeExportDirectory);
        var content = File.ReadAllText(pbxprojPath);

        var target = FindNativeTarget(content, IOSMainTargetName);

        content = PatchCodeSignEntitlements(content, target.ConfigurationListId, IOSEntitlementsFileName);
        content = PatchSystemCapabilities(content, target.Id);
        content = AddFrameworkToFrameworksPhase(content, target.FrameworksBuildPhaseId, IOSUserNotificationsFrameworkName);

        File.WriteAllText(pbxprojPath, content, new UTF8Encoding(false));
    }

    private static AbsolutePath FindPbxprojPath(AbsolutePath xcodeExportDirectory)
    {
        var pbxprojFile = xcodeExportDirectory
            .GlobFiles("*.xcodeproj/project.pbxproj")
            .FirstOrDefault();

        return pbxprojFile == null
            ? throw new FileNotFoundException($"project.pbxproj was not found in Xcode export directory: {xcodeExportDirectory}")
            : pbxprojFile;
    }

    private static PbxNativeTargetInfo FindNativeTarget(string content, string preferredTargetName)
    {
        var targets = Regex
            .Matches(
                content,
                @"(?ms)^\s*(?<id>[A-F0-9]{24}) /\* (?<name>.*?) \*/ = \{\s*isa = PBXNativeTarget;(?<body>.*?)^\s*\};")
            .Cast<Match>()
            .Select(match => new
            {
                Id = match.Groups["id"].Value,
                Name = match.Groups["name"].Value,
                Body = match.Groups["body"].Value,
            })
            .ToArray();

        if (targets.Length == 0)
            throw new InvalidOperationException("PBXNativeTarget section was not found in project.pbxproj.");

        var target = targets.FirstOrDefault(x => x.Name == preferredTargetName) ?? targets[0];

        var configurationListId = Regex.Match(
            target.Body,
            @"buildConfigurationList = (?<id>[A-F0-9]{24}) /\* .*? \*/;").Groups["id"].Value;

        if (string.IsNullOrWhiteSpace(configurationListId))
            throw new InvalidOperationException($"Build configuration list was not found for PBXNativeTarget '{target.Name}'.");

        var frameworksBuildPhaseId = Regex.Match(
            target.Body,
            @"(?m)^\s*(?<id>[A-F0-9]{24}) /\* Frameworks \*/,").Groups["id"].Value;

        if (string.IsNullOrWhiteSpace(frameworksBuildPhaseId))
            throw new InvalidOperationException($"Frameworks build phase was not found for PBXNativeTarget '{target.Name}'.");

        return new PbxNativeTargetInfo(target.Id, target.Name, configurationListId, frameworksBuildPhaseId);
    }

    private static string PatchCodeSignEntitlements(string content, string configurationListId, string entitlementsFile)
    {
        var buildConfigurationIds = FindBuildConfigurationIds(content, configurationListId);

        foreach (var buildConfigurationId in buildConfigurationIds)
            content = PatchBuildConfigurationEntitlements(content, buildConfigurationId, entitlementsFile);

        return content;
    }

    private static string PatchSystemCapabilities(string content, string targetId)
    {
        content = AddSystemCapability(content, targetId, "com.apple.BackgroundModes");
        content = AddSystemCapability(content, targetId, "com.apple.Push");

        return content;
    }

    private static string AddSystemCapability(string content, string targetId, string capabilityName)
    {
        var targetAttributesRange = FindObjectRange(content, @"(?m)^\s*TargetAttributes = \{");

        if (targetAttributesRange == null)
            throw new InvalidOperationException("TargetAttributes section was not found in project.pbxproj.");

        var targetRange = FindObjectRange(
            content,
            @"(?m)^\s*" + Regex.Escape(targetId) + @" = \{",
            targetAttributesRange.Value.OpenBraceIndex,
            targetAttributesRange.Value.CloseBraceIndex);

        if (targetRange == null)
            return InsertTargetAttributes(content, targetAttributesRange.Value, targetId, capabilityName);

        var systemCapabilitiesRange = FindObjectRange(
            content,
            @"(?m)^\s*SystemCapabilities = \{",
            targetRange.Value.OpenBraceIndex,
            targetRange.Value.CloseBraceIndex);

        if (systemCapabilitiesRange == null)
        {
            var systemCapabilitiesBlock =
                "\t\t\t\t\tSystemCapabilities = {\n" +
                CreateSystemCapabilityBlock(capabilityName, "\t\t\t\t\t\t") +
                "\t\t\t\t\t};\n";

            return content.Insert(targetRange.Value.CloseBraceIndex, systemCapabilitiesBlock);
        }

        var systemCapabilitiesContent = content.Substring(
            systemCapabilitiesRange.Value.OpenBraceIndex,
            systemCapabilitiesRange.Value.CloseBraceIndex - systemCapabilitiesRange.Value.OpenBraceIndex);

        if (systemCapabilitiesContent.Contains(capabilityName))
            return content;

        return content.Insert(
            systemCapabilitiesRange.Value.CloseBraceIndex,
            CreateSystemCapabilityBlock(capabilityName, "\t\t\t\t\t\t"));
    }

    private static string InsertTargetAttributes(string content, TextObjectRange targetAttributesRange, string targetId, string capabilityName)
    {
        var targetAttributesBlock =
            $"\t\t\t\t{targetId} = {{\n" +
            "\t\t\t\t\tSystemCapabilities = {\n" +
            CreateSystemCapabilityBlock(capabilityName, "\t\t\t\t\t\t") +
            "\t\t\t\t\t};\n" +
            "\t\t\t\t};\n";

        return content.Insert(targetAttributesRange.CloseBraceIndex, targetAttributesBlock);
    }

    private static string CreateSystemCapabilityBlock(string capabilityName, string indent)
    {
        return
            $"{indent}{capabilityName} = {{\n" +
            $"{indent}\tenabled = 1;\n" +
            $"{indent}}};\n";
    }

    private static string[] FindBuildConfigurationIds(string content, string configurationListId)
    {
        var match = Regex.Match(
            content,
            "(?ms)^\\s*" + Regex.Escape(configurationListId) + " /\\* .*? \\*/ = \\{\\s*isa = XCConfigurationList;.*?buildConfigurations = \\((?<configs>.*?)\\);.*?^\\s*\\};");

        if (match.Success == false)
            throw new InvalidOperationException($"XCConfigurationList '{configurationListId}' was not found in project.pbxproj.");

        return Regex
            .Matches(match.Groups["configs"].Value, @"(?<id>[A-F0-9]{24}) /\*")
            .Cast<Match>()
            .Select(x => x.Groups["id"].Value)
            .Distinct()
            .ToArray();
    }

    private static string PatchBuildConfigurationEntitlements(string content, string buildConfigurationId, string entitlementsFile)
    {
        var pattern = "(?ms)(?<prefix>^\\s*" + Regex.Escape(buildConfigurationId) + " /\\* .*? \\*/ = \\{\\s*isa = XCBuildConfiguration;.*?buildSettings = \\{\\n?)(?<settings>.*?)(?<suffix>^\\s*\\};.*?^\\s*\\};)";

        return Regex.Replace(content, pattern, match =>
        {
            var settings = match.Groups["settings"].Value;

            if (Regex.IsMatch(settings, @"(?m)^\s*CODE_SIGN_ENTITLEMENTS = .*?;"))
            {
                settings = Regex.Replace(
                    settings,
                    @"(?m)^(?<indent>\s*)CODE_SIGN_ENTITLEMENTS = .*?;",
                    $"${{indent}}CODE_SIGN_ENTITLEMENTS = {entitlementsFile};");
            }
            else
            {
                settings = $"\t\t\t\tCODE_SIGN_ENTITLEMENTS = {entitlementsFile};\n" + settings;
            }

            return match.Groups["prefix"].Value + settings + match.Groups["suffix"].Value;
        }, RegexOptions.None, TimeSpan.FromSeconds(1));
    }

    private static string AddFrameworkToFrameworksPhase(string content, string frameworksBuildPhaseId, string frameworkName)
    {
        if (content.Contains($"{frameworkName} in Frameworks"))
            return content;

        var fileReferenceId = FindFrameworkFileReferenceId(content, frameworkName)
            ?? GeneratePbxId(content, $"PBXFileReference:{frameworkName}");

        var buildFileId = GeneratePbxId(content, $"PBXBuildFile:{frameworkName}:{fileReferenceId}");

        if (FindFrameworkFileReferenceId(content, frameworkName) == null)
        {
            content = InsertBeforeSectionEnd(
                content,
                "PBXFileReference",
                $"\t\t{fileReferenceId} /* {frameworkName} */ = {{isa = PBXFileReference; lastKnownFileType = wrapper.framework; name = {frameworkName}; path = System/Library/Frameworks/{frameworkName}; sourceTree = SDKROOT; }};\n");
        }

        content = InsertBeforeSectionEnd(
            content,
            "PBXBuildFile",
            $"\t\t{buildFileId} /* {frameworkName} in Frameworks */ = {{isa = PBXBuildFile; fileRef = {fileReferenceId} /* {frameworkName} */; }};\n");

        return AddBuildFileToFrameworksPhase(content, frameworksBuildPhaseId, buildFileId, frameworkName);
    }

    private static string AddBuildFileToFrameworksPhase(
        string content,
        string frameworksBuildPhaseId,
        string buildFileId,
        string frameworkName)
    {
        var pattern = "(?ms)(?<prefix>^\\s*" + Regex.Escape(frameworksBuildPhaseId) + " /\\* Frameworks \\*/ = \\{\\s*isa = PBXFrameworksBuildPhase;.*?files = \\(\\n?)(?<files>.*?)(?<suffix>^\\s*\\);.*?^\\s*\\};)";

        return Regex.Replace(content, pattern, match =>
        {
            var files = match.Groups["files"].Value;

            if (files.Contains(buildFileId) || files.Contains($"{frameworkName} in Frameworks"))
                return match.Value;

            files += $"\t\t\t\t{buildFileId} /* {frameworkName} in Frameworks */,\n";

            return match.Groups["prefix"].Value + files + match.Groups["suffix"].Value;
        }, RegexOptions.None, TimeSpan.FromSeconds(1));
    }

    private static string FindFrameworkFileReferenceId(string content, string frameworkName)
    {
        var match = Regex.Match(
            content,
            @"(?s)(?<id>[A-F0-9]{24}) /\* " + Regex.Escape(frameworkName) + @" \*/ = \{isa = PBXFileReference;.*?\};");

        return match.Success ? match.Groups["id"].Value : null;
    }

    private static string InsertBeforeSectionEnd(string content, string sectionName, string line)
    {
        var sectionEnd = $"/* End {sectionName} section */";
        var index = content.IndexOf(sectionEnd, StringComparison.Ordinal);

        if (index < 0)
            throw new InvalidOperationException($"Section '{sectionName}' was not found in project.pbxproj.");

        return content.Insert(index, line);
    }

    private static TextObjectRange? FindObjectRange(string content, string pattern, int startIndex = 0, int? endIndex = null)
    {
        var length = (endIndex ?? content.Length) - startIndex;
        var match = Regex.Match(content.Substring(startIndex, length), pattern);

        if (match.Success == false)
            return null;

        var absoluteMatchIndex = startIndex + match.Index;
        var openBraceIndex = content.IndexOf('{', absoluteMatchIndex);

        if (openBraceIndex < 0 || openBraceIndex >= (endIndex ?? content.Length))
            return null;

        var closeBraceIndex = FindMatchingCloseBraceIndex(content, openBraceIndex);

        return new TextObjectRange(openBraceIndex, closeBraceIndex);
    }

    private static int FindMatchingCloseBraceIndex(string content, int openBraceIndex)
    {
        var depth = 0;

        for (var index = openBraceIndex; index < content.Length; index++)
        {
            switch (content[index])
            {
                case '{':
                    depth++;
                    break;
                case '}':
                    depth--;

                    if (depth == 0)
                        return index;
                    break;
            }
        }

        throw new InvalidOperationException("Could not find matching closing brace in project.pbxproj.");
    }

    private static string GeneratePbxId(string content, string salt)
    {
        for (var attempt = 0; attempt < 100; attempt++)
        {
            var bytes = SHA1.HashData(Encoding.UTF8.GetBytes($"{salt}:{attempt}"));
            var id = Convert.ToHexString(bytes)[..24];

            if (content.Contains(id) == false)
                return id;
        }

        throw new InvalidOperationException($"Could not generate unique PBX id for '{salt}'.");
    }

    private sealed record PbxNativeTargetInfo(
        string Id,
        string Name,
        string ConfigurationListId,
        string FrameworksBuildPhaseId);

    private readonly record struct TextObjectRange(int OpenBraceIndex, int CloseBraceIndex);
}