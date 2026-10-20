using System.Collections.Generic;
using System.Linq;
using Extensions;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tools.Unity;

public partial class Build
{
    const string AndroidExecuteMethod = "Sotbify.MobileApp.Builder.Editor.Builder.Build";
    
    static AbsolutePath KeystoresDirectory => RootDirectory / "keystores";
    static AbsolutePath KeystoreFile => KeystoresDirectory / "launcher.keystore";
    
    [Parameter("Android keystore password")] 
    readonly string AndroidKeystorePassword = string.Empty;
    
    [Parameter("Android alias name")] 
    readonly string AndroidAliasName = string.Empty;
    
    [Parameter("Android alias password")] 
    readonly string AndroidAliasPassword = string.Empty;
    
    Target BuildAndroidUnity => target => target
        .After(IncrementVersionAndPush)
        .Executes(() =>
        {
            var customArguments = GetCommonUnityBuildArguments()
                .Concat(GetAndroidUnityBuildArguments());

            var settings = GetUnityCommonSettings()
                .SetBuildTarget(UnityBuildTarget.Android)
                .SetCustomArguments(customArguments)
                .SetExecuteMethod(AndroidExecuteMethod);

            UnityTasks.Unity(settings);
        });
    
    private IEnumerable<(string, object)> GetAndroidUnityBuildArguments()
    {
        if (KeystoreFile.Exists())
            yield return ("androidKeystoreName", KeystoreFile.ToString());

        if (string.IsNullOrEmpty(AndroidKeystorePassword) == false)
            yield return ("androidKeystorePassword", AndroidKeystorePassword);

        if (string.IsNullOrEmpty(AndroidAliasName) == false)
            yield return ("androidAliasName", AndroidAliasName);

        if (string.IsNullOrEmpty(AndroidAliasPassword) == false)
            yield return ("androidAliasPassword", AndroidAliasPassword);

        yield return ("signApk", true);
    }
}