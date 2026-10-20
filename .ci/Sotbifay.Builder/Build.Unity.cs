using Extensions;
using Models;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Unity;
using Nuke.Common;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;
using Types;

public partial class Build
{
    static AbsolutePath ProjectDirectory => RootDirectory / "src" / UnityProjectName;
    static AbsolutePath UnityTestResultsFile => ArtifactsDirectory / "testResults_editor.xml";
    static AbsolutePath UnityLogFile => ArtifactsDirectory / "unity_build.log";
    
    [Parameter("Unity serial"), Secret] 
    readonly string UnitySerial = string.Empty;
    
    [Parameter("Unity username")] 
    readonly string UnityUsername = string.Empty;
    
    [Parameter("Unity password"), Secret]
    readonly string UnityPassword = string.Empty;
    
    [Parameter("Build target")] 
    readonly UnityBuildTarget BuildTarget = UnityBuildTarget.Android;
    
    [Parameter("Build type")] 
    readonly UnityBuildType BuildType = UnityBuildType.Debug;
    
    [Parameter("Defines to add")]
    readonly string[] Defines = [];
    
    [Parameter("Defines to remove")]
    readonly string[] RemoveDefines = [];

    [Parameter("Prefix added before the build name")]
    readonly string BuildPrefix = string.Empty;
    
    [Parameter("Suffix added after the build name")]
    readonly string BuildSuffix = string.Empty;
    
    Target CleanUnityProject => target => target
        .Executes(() =>
        {
            (ProjectDirectory / "Library").SafeRemove();
            (ProjectDirectory / "Logs").SafeRemove();
            (ProjectDirectory / "obj").SafeRemove();
            (ProjectDirectory / "Temp").SafeRemove();

            foreach (var cSharpProjectFile in ProjectDirectory.GlobFiles("*.csproj", "*.sln")) 
                cSharpProjectFile.DeleteFile();
        });
    
    Target RestoreNuGetPackages => target => target
        .After(CleanArtifacts)
        .Executes(() =>
        {
            ProcessTasks
                .StartProcess("dotnet", $"nugetforunity restore {ProjectDirectory}")
                .AssertZeroExitCode();
        });
    
    Target RunEditorTests => target => target
        .Executes(() =>
        {
            var unitySettings = GetUnityCommonSettings()
                .EnableRunEditorTests()
                .SetEditorTestsResultFile(UnityTestResultsFile);

            UnityTasks.Unity(unitySettings);
        });
    
    private static AbsolutePath GetBuildReportFileOrDefault()
    {
        return ArtifactsDirectory.GlobFiles("build_report_*.json")
            .OrderByDescending(filePath => File.GetCreationTime(filePath))
            .FirstOrDefault();
    }

    private static async ValueTask<BuildReport> GetBuildReportAsync(AbsolutePath buildReportPath, CancellationToken cancellationToken)
    {
        await using var fileStream = File.OpenRead(buildReportPath);
        return await JsonSerializer.DeserializeAsync<BuildReport>(fileStream, JsonSerializerOptions.Web, cancellationToken);
    }

    [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    private UnitySettings GetUnityCommonSettings()
    {
        return new UnitySettings()
            .EnableBatchMode()
            .EnableNoGraphics()
            .EnableQuit()
            .SetProjectPath(ProjectDirectory)
            .SetLogFile(UnityLogFile)
            .SetBuildTarget(BuildTarget)
            .With(x => x.SetSerial(UnitySerial), string.IsNullOrEmpty(UnitySerial) == false)
            .With(x => x.SetUsername(UnityUsername), string.IsNullOrEmpty(UnityUsername) == false)
            .With(x => x.SetPassword(UnityPassword), string.IsNullOrEmpty(UnityPassword) == false);
    }
    
    private IEnumerable<(string, object)> GetCommonUnityBuildArguments()
    {
        yield return ("type", BuildType);
        
        yield return ("outputDirectory", ArtifactsDirectory);
        
        foreach (var define in Defines)
            yield return ("defines", define);
        
        foreach (var define in RemoveDefines)
            yield return ("removeDefines", define);

        if(string.IsNullOrEmpty(BuildPrefix) == false)
            yield return ("buildPrefix", BuildPrefix);
        
        if(string.IsNullOrEmpty(BuildSuffix) == false)
            yield return ("buildSuffix", BuildSuffix);
    }
}