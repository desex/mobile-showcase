using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Sotbify.MobileApp.Builder.Editor
{
    public static class VersionBumper
    {
        public static void Bump()
        {
            var newAndroidBundleVersionCode = PlayerSettings.Android.bundleVersionCode + 1;
            var (major, minor,patch) = ConvertToVersion(newAndroidBundleVersionCode);
            
            if (long.TryParse(PlayerSettings.iOS.buildNumber, out var currentIOSBundleVersionCode) == false)
                throw new Exception("Failed to parse iOS build number");
            
            var newIOSBundleVersionCode = currentIOSBundleVersionCode + 1;
            
            var version = $"{major}.{minor}.{patch}";
            
            PlayerSettings.Android.bundleVersionCode = newAndroidBundleVersionCode;
            PlayerSettings.iOS.buildNumber = newIOSBundleVersionCode.ToString();
            PlayerSettings.bundleVersion = version;
            AssetDatabase.SaveAssets();

            var result = newAndroidBundleVersionCode.ToString();
        
            var tempFilePath = Path.Combine(Path.GetTempPath(), "nuke_result.txt");
            Directory.CreateDirectory(Path.GetDirectoryName(tempFilePath) ?? "");
            File.WriteAllText(tempFilePath, result);
            
            Debug.LogError($"Saving in {tempFilePath}");

            EditorApplication.Exit(0);
        }
        
        private static (long Major, long Minor, long Patch) ConvertToVersion(long versionCode) => 
            (versionCode / 10000, versionCode / 100 % 100, versionCode % 100);
    }
}