using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sotbifay.Launcher.Common.Tools.Editor.Extensions
{
    public static class ScenesExtensions
    {
        public static IEnumerable<GameObject> GetAllGameObjects(this Scene scene)
        {
            return scene
                .GetRootGameObjects()
                .GetChildGameObjects(true);
        }

        public static IEnumerable<Scene> GetAllScenes() => 
            GetAllScenes("Assets");

        public static IEnumerable<Scene> GetAllScenes(params string[] searchInFolders)
        {
            foreach (var scenePath in GetAllScenesPaths(searchInFolders))
            {
                var scene = SceneManager.GetSceneByPath(scenePath);
                if (scene.isLoaded)
                {
                    yield return scene;
                }
                else
                {
                    var openedScene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                    yield return openedScene;
                    EditorSceneManager.CloseScene(openedScene, true);
                }
            }
        }

        public static IEnumerable<string> GetAllScenesPaths() =>
            GetAllScenesPaths("Assets");

        public static IEnumerable<string> GetAllScenesPaths(params string[] searchInFolders)
        {
            return AssetDatabase.FindAssets("t:Scene", searchInFolders)
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => string.IsNullOrEmpty(path) == false);
        }
        
        public static IEnumerable<GameObject> FindGameObjectsWithMissingComponents(this in Scene scene)
        {
            return scene
                .GetAllGameObjects()
                .Where(gameObject => gameObject.CheckHasMissingComponents());
        }

        public static IEnumerable<GameObject> FindMissingPrefabsInstances(this in Scene scene)
        {
            return scene
                .GetAllGameObjects()
                .Where(PrefabUtility.IsPrefabAssetMissing);
        }
    }
}