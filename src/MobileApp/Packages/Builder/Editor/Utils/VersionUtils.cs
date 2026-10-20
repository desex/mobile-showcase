using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Sotbifay.Builder.Editor
{
    public static class VersionUtils
    {
        public static (int Major, int Minor, int Patch) ParseVersion(string versionString)
        {
            var separatedBundleVersion = versionString
                .Split('.')
                .Reverse()
                .Take(3)
                .Select(s => int.TryParse(s, out var value) ? value : 0)
                .ToList();
            
            var separatedBundleVersionCount = separatedBundleVersion.Count;
            
            var patch = separatedBundleVersionCount > 0 ? Mathf.Clamp(separatedBundleVersion[0], 0, 99) : 0; 
            var minor = separatedBundleVersionCount > 1 ? Mathf.Clamp(separatedBundleVersion[1], 0, 99) : 0; 
            var major = separatedBundleVersionCount > 2 ? separatedBundleVersion[2] : 0;

            return (major, minor, patch);
        }

        public static long ConvertToVersionCode(int major, int minor, int patch) => 
            patch + minor * 100 + major * 10000;

        public static (int Major, int Minor, int Patch) ConvertToVersion(long versionCode) => 
            ((int)versionCode / 10000, (int)versionCode / 100 % 100, (int)versionCode % 100);

        public static (int Major, int Minor, int Patch) GetVersion() => 
            ParseVersion(PlayerSettings.bundleVersion);

        public static void SetVersion(int major, int minor, int patch) => 
            PlayerSettings.bundleVersion = $"{major}.{minor}.{patch}";

        public static long GetVersionCode()
        {
            var buildTarget = NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            if (buildTarget == NamedBuildTarget.Android)
                return PlayerSettings.Android.bundleVersionCode;
            
            if(buildTarget == NamedBuildTarget.iOS)
                return long.TryParse(PlayerSettings.iOS.buildNumber, out var result) ? result : 0;

            Debug.LogWarning($"[{nameof(GetVersionCode)}]: Unsupported target: {buildTarget}");
            return 0;
        }

        public static void SetVersionCode(long versionCode)
        {
            var buildTarget = NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            if (buildTarget == NamedBuildTarget.Android)
            {
                PlayerSettings.Android.bundleVersionCode = (int)versionCode;
                return;
            }

            if (buildTarget == NamedBuildTarget.iOS)
            {
                PlayerSettings.iOS.buildNumber = versionCode.ToString();
                return;
            }
            
            Debug.LogWarning($"[{nameof(SetVersionCode)}]: Unsupported target: {buildTarget}");
        }
    }
}