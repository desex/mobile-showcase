using System.Collections.Generic;
using System.Linq;
using Extensions;
using Nuke.Common;
using Nuke.Common.Tools.Unity;

public partial class Build
{
    const string IOSExecuteMethod = "Sotbify.MobileApp.Builder.Editor.Builder.BuildIOS";
    
    [Parameter("iOS team id"), Required] 
    readonly string IOSTeamID = string.Empty;
    
    [Parameter("Package name"), Required]
    readonly string IOSPackageName = string.Empty;
    
    Target BuildIOSUnity => target => target
        .After(CleanArtifacts)
        .After(ChangeSettingFiles)
        .Executes(() =>
        {
            var customArguments = GetCommonUnityBuildArguments()
                .Concat(GetIOSUnityBuildArguments());

            var settings = GetUnityCommonSettings()
                .SetBuildTarget(UnityBuildTarget.iOS)
                .SetCustomArguments(customArguments)
                .SetExecuteMethod(IOSExecuteMethod);

            UnityTasks.Unity(settings);
        });
    
    private IEnumerable<(string, object)> GetIOSUnityBuildArguments()
    {
        if (string.IsNullOrEmpty(IOSTeamID) == false)
            yield return ("iosTeamID", IOSTeamID);
        
        if (string.IsNullOrEmpty(IOSTeamID) == false)
            yield return ("iosPackageName", IOSPackageName);
        
        if (string.IsNullOrEmpty(PartnerName) == false)
            yield return ("partnerName", PartnerName);
    }
}