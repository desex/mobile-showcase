using Extensions;
using Nuke.Common;
using Nuke.Common.IO;

public partial class Build : NukeBuild
{
    const string UnityProjectName = "MobileApp";
    
    static AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    public static int Main () => Execute<Build>();

    Target BuildAndDeploy => target => target
        .DependsOn(CleanArtifacts)
        .DependsOn(RestoreNuGetPackages)
        .DependsOn(IncrementVersionAndPush)
        .DependsOn(BuildAndroidUnity)
        .DependsOn(Deploy);

    Target BuildIOSAndSave => target => target
        .DependsOn(CleanArtifacts)
        .DependsOn(RestoreNuGetPackages)
        .DependsOn(IncrementVersion)
        .DependsOn(ChangeSettingFiles)
        .DependsOn(BuildIOSXcode)
        .DependsOn(RestoreSettingFiles);

    Target BuildIOSAndDeploy => target => target
        .DependsOn(CleanArtifacts)
        .DependsOn(RestoreNuGetPackages)
        .DependsOn(IncrementVersion)
        .DependsOn(ChangeSettingFiles)
        .DependsOn(UploadIOSToTestFlight)
        .DependsOn(RestoreSettingFiles);
    
    Target CleanArtifacts => target => target
        .Executes(() =>
        {
            ArtifactsDirectory.SafeRemove();
            ArtifactsDirectory.CreateDirectory();
        });
}
