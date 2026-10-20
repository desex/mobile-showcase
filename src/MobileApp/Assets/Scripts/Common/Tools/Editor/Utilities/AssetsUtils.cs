using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Sotbifay.MobileApp.Common.Tools.Editor.Utilities
{
    public static class AssetsUtils
    {
        public static IEnumerable<string> FindAllByExtensions(string extension) => 
            FindAllByExtensions(extension, "Assets");

        public static IEnumerable<string> FindAllByExtensions(string extension, params string[] searchInFolders)
        {
            return AssetDatabase.FindAssets(string.Empty, searchInFolders)
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(static path => string.IsNullOrEmpty(path) == false)
                .Where(path => Path.GetExtension(path) == extension);
        }
        
        public static IEnumerable<T> FindAllByType<T>() where T : Object => 
            FindAllByType<T>("Assets");

        public static IEnumerable<T> FindAllByType<T>(params string[] searchInFolders) where T : Object
        {
            return AssetDatabase.FindAssets($"t:{typeof(T).Name}", searchInFolders)
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(static path => string.IsNullOrEmpty(path) == false)
                .Select(static path => AssetDatabase.LoadAssetAtPath<T>(path));
        }
    }
}