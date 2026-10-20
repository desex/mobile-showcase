using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Sotbifay.Launcher.Common.Tools.Editor.Extensions;
using UnityEditor.SceneManagement;

namespace Sotbifay.Launcher.Common.Tests.EditMode
{
    public sealed class ScenesValidationTests
    {
        [TestCaseSource(nameof(GetAllScenesPaths))]
        public void All_Scenes_Should_Not_Have_Missing_Components(string scenePath)
        {
            var openedScene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            var gameObjectsWithMissingComponents = openedScene
                .FindGameObjectsWithMissingComponents()
                .GroupBy(gameObject => gameObject.name)
                .Select(group => $"Game object(s): \"{group.Key}\"; Count: {group.Count()}");
            
            Assert.IsEmpty(gameObjectsWithMissingComponents);
            EditorSceneManager.CloseScene(openedScene, true);
        }
        
        [TestCaseSource(nameof(GetAllScenesPaths))]
        public void All_Scenes_Should_Not_Have_Missing_Prefabs(string scenePath)
        {
            var openedScene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            var gameObjectsIsAMissingPrefabs = openedScene
                .FindMissingPrefabsInstances()
                .GroupBy(gameObject => gameObject.name)
                .Select(group => $"Prefab(s): \"{group.Key}\"; Count: {group.Count()}");
            
            Assert.IsEmpty(gameObjectsIsAMissingPrefabs);
            EditorSceneManager.CloseScene(openedScene, true);
        }
     
        private static IEnumerable<string> GetAllScenesPaths() => ScenesExtensions.GetAllScenesPaths();
    }
}