using System;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;
using VContainer.Unity;

namespace Sotbify.MobileApp.ItemRenderer
{
    [Serializable]
    public sealed class ItemRendererProjectInstaller : IInstaller
    {
        [SerializeField] private Vector3 _firstInstancePosition = new(100f, 0f, 0f);
        [SerializeField] private Vector3 _instanceOffset = new(100f, 0f, 0f);
        [SerializeField] private Vector2Int _defaultResolution = new(512, 512);
        [SerializeField] private ItemRenderer _itemRendererPrefab;
        
        public void Install(IContainerBuilder builder)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}