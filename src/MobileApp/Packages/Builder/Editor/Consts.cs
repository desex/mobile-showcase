using System.IO;

#if UNITY_ANDROID
using UnityEditor.Android;
#endif

namespace Sotbifay.Builder.Editor
{
    public static class Consts
    {
        public const string MenuItemRoot = "Sotbifay/Builder/";
        
        // public const string UnityPlayerActivity = "com.unity3d.player.UnityPlayerNativeActivityPico";
        public const string UnityPlayerActivity = "com.unity3d.player.UnityPlayerActivity";
        
        public static readonly string PackageRootDirectoryFullPath = PackageUtils.GetPackageFullPathByName(PackageUtils.GetPackageNameByCallerFile());

        public static readonly string JdkRootPath = 
#if UNITY_ANDROID
            AndroidExternalToolsSettings.jdkRootPath;
#else
            string.Empty;
#endif
        
        public static readonly string SdkRootPath = 
#if UNITY_ANDROID
            AndroidExternalToolsSettings.sdkRootPath;
#else
            string.Empty;
#endif
        
        public static readonly string FilesForSignDirectoryFullPath = Path.Combine(PackageRootDirectoryFullPath, "FilesForSign~");
        public static readonly string Jdk8DirectoryFullPath = Path.Combine(PackageRootDirectoryFullPath, "jdk8~");
        
        public static readonly string Pk8FileFullPath = Path.Combine(FilesForSignDirectoryFullPath, "platform.pk8");
        public static readonly string PemFileFullPath = Path.Combine(FilesForSignDirectoryFullPath, "platform.x509.pem");
    }
}