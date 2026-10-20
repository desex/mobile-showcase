using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using USceneInstance = UnityEngine.ResourceManagement.ResourceProviders.SceneInstance;

namespace Sotbifay.MobileApp.Assets
{
    public struct SceneInstance : IEquatable<SceneInstance>
    {
        private USceneInstance _sceneInstance;
        
        public SceneInstance(USceneInstance sceneInstance)
        {
            _sceneInstance = sceneInstance;
        }
        
        public Scene Scene => _sceneInstance.Scene;
        
        public static implicit operator USceneInstance(SceneInstance sceneInstance) => 
            sceneInstance._sceneInstance;
        
        public static implicit operator SceneInstance(USceneInstance sceneInstance) => 
            new(sceneInstance);

        public UniTask ActivateAsync(CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public bool Equals(SceneInstance other) {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public override bool Equals(object obj) {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public override int GetHashCode() {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}