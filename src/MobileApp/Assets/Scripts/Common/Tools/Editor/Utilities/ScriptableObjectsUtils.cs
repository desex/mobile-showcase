using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Sotbifay.Launcher.Common.Tools.Editor.Utilities
{
    public static class ScriptableObjectsUtils
    {
        public static IEnumerable<string> FindAll() => 
            FindAll("Assets");

        public static IEnumerable<string> FindAll(params string[] searchInFolders)
        {
            return AssetDatabase.FindAssets("t:ScriptableObject", searchInFolders)
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(static path => string.IsNullOrEmpty(path) == false);
        }
    }
}