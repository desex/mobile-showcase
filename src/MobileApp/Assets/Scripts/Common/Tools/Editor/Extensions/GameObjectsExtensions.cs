using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Sotbifay.Launcher.Common.Tools.Editor.Extensions
{
    public static class GameObjectsExtensions
    {
        public static IEnumerable<GameObject> GetChildGameObjects(this GameObject gameObject, bool includeParent = false)
        {
            if (gameObject is null)
                throw new ArgumentNullException(nameof(gameObject));

            return gameObject.transform
                .GetAllChildren(includeParent)
                .Select(static child => child.gameObject);
        }
        
        public static IEnumerable<GameObject> GetChildGameObjects(this IEnumerable<GameObject> gameObjects, bool includeParent = false)
        {
            if (gameObjects is null)
                throw new ArgumentNullException(nameof(gameObjects));

            return gameObjects
                .Select(static gameObject => gameObject.transform)
                .GetAllChildren(includeParent)
                .Select(static transform => transform.gameObject);
        }
        
        public static bool CheckHasMissingComponents(this GameObject gameObject)
        {
            if (gameObject is null)
                throw new ArgumentNullException(nameof(gameObject));

            return GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(gameObject) > 0;
        }

        public static int RemoveMissingComponents(this GameObject gameObject)
        {
            if (gameObject is null)
                throw new ArgumentNullException(nameof(gameObject));

            return GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObject);
        }
    }
}