using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Sotbifay.Launcher.Common.Tools.Editor.Extensions;
using Sotbifay.Launcher.Common.Tools.Editor.Utilities;
using UnityEditor;
using UnityEngine;

namespace Sotbifay.Launcher.Common.Tests.EditMode
{
    public sealed class PrefabsValidationTests
    {
        [TestCaseSource(nameof(GetAllPrefabsPaths))]
        public void All_Prefabs_Should_Not_Have_Missing_Components(string prefabPath)
        {
            var gameObjectsWithMissingComponents = AssetDatabase
                .LoadAssetAtPath<GameObject>(prefabPath)
                .GetChildGameObjects(true)
                .Where(gameObject => gameObject.CheckHasMissingComponents())
                .GroupBy(gameObject => gameObject.name)
                .Select(group => $"Game object(s): \"{group.Key}\"; Count: {group.Count()}");

            Assert.IsEmpty(gameObjectsWithMissingComponents);
        }

        private static IEnumerable<string> GetAllPrefabsPaths() => PrefabsUtils.FindAll();
    }
}