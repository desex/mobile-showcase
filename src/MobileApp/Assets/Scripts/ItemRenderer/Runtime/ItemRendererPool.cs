using System;
using System.Collections.Generic;
using Sotbifay.MobileApp.Logger;
using UnityEngine;
using UnityEngine.Pool;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;
using Object = UnityEngine.Object;

namespace Sotbify.MobileApp.ItemRenderer
{
    public sealed class ItemRendererPool : IObjectPool<IItemRenderer>, IDisposable
    {
        private readonly HashSet<IItemRenderer> _leasedRenderers = new();

        private readonly Vector3 _firstInstancePosition;
        private readonly Vector3 _instanceOffset;
        private readonly ItemRenderer _itemRendererPrefab;
        private readonly Vector2Int _defaultResolution;
        private readonly ILogger _logger;
        private readonly IObjectPool<IItemRenderer> _pool;

        private int _instanceIndex = 0;

        public ItemRendererPool
        (
            Vector3 firstInstancePosition,
            Vector3 instanceOffset,
            ItemRenderer itemRendererPrefab,
            Vector2Int defaultResolution,
            ILogger logger
        )
        {
            _firstInstancePosition = firstInstancePosition;
            _instanceOffset = instanceOffset;
            _itemRendererPrefab = itemRendererPrefab ?? throw new ArgumentNullException(nameof(itemRendererPrefab));
            _defaultResolution = defaultResolution;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _pool = new ObjectPool<IItemRenderer>(Create, OnGet, OnRelease, OnDestroy, collectionCheck: true);
        }

        public int CountInactive => _pool.CountInactive;

        public IItemRenderer Get()
        {
            var itemRenderer = _pool.Get();

            if (_leasedRenderers.Add(itemRenderer) == false)
                throw new InvalidOperationException($"{nameof(ItemRenderer)} was leased more than once");
            
            itemRenderer.RenderingIsActive = true;

            return itemRenderer;
        }

        public PooledObject<IItemRenderer> Get(out IItemRenderer itemRenderer)
        {
            itemRenderer = Get();
            return new PooledObject<IItemRenderer>(itemRenderer, this);
        }

        public void Release(IItemRenderer element)
        {
            if (element == null)
                return;

            if (_leasedRenderers.Remove(element) == false)
            {
                _logger.Error($"Attempted to release an {nameof(ItemRenderer)} that is not leased by this pool", context: this);
                return;
            }

            element.RenderingIsActive = false;

            _pool.Release(element);
        }

        public void Clear() =>
            _pool.Clear();

        public void Dispose() =>
            _pool.Clear();

        private ItemRenderer Create()
        {
            var prefabGameObject = _itemRendererPrefab.gameObject;
            var wasActive = prefabGameObject.activeSelf;
            using (new PrefabDirtyScope(prefabGameObject))
            {
                prefabGameObject.SetActive(false);

                var itemRenderer = Object.Instantiate(_itemRendererPrefab);
                var itemRendererGameObject = itemRenderer.gameObject;
                try
                {
                    itemRendererGameObject.name = $"{nameof(ItemRenderer)}_{_instanceIndex}";

                    var itemRendererTransform = itemRendererGameObject.transform;
                    itemRendererTransform.position = _firstInstancePosition + _instanceOffset * _instanceIndex;
                    
                    itemRenderer.Resolution = _defaultResolution;

                    _instanceIndex++;
                    return itemRenderer;
                }
                finally
                {
                    prefabGameObject.SetActive(wasActive);
                    itemRendererGameObject.SetActive(wasActive);
                }
            }
        }

        private static void OnGet(IItemRenderer itemRenderer)
        {
            itemRenderer.RenderingIsActive = true;

            if (itemRenderer is ItemRenderer monoItemRenderer == false)
                return;
            
            if(monoItemRenderer == null)
                return;

            var itemRendererGameObject = monoItemRenderer.gameObject;
            if(itemRendererGameObject == null)
                return;
            
            itemRendererGameObject.SetActive(true);
        }

        private static void OnRelease(IItemRenderer itemRenderer)
        {
            itemRenderer.RenderingIsActive = false;

            if (itemRenderer is ItemRenderer monoItemRenderer == false)
                return;
            
            if(monoItemRenderer == null)
                return;

            var itemRendererGameObject = monoItemRenderer.gameObject;
            if(itemRendererGameObject == null)
                return;
            
            itemRendererGameObject.SetActive(false);
        }

        private static void OnDestroy(IItemRenderer itemRenderer)
        {
            if (itemRenderer is ItemRenderer monoItemRenderer == false)
                return;

            if (monoItemRenderer == null)
                return;

            var itemRendererGameObject = monoItemRenderer.gameObject;
            if(itemRendererGameObject == null)
                return;
            
            Object.Destroy(itemRendererGameObject);
        }

        #region Prefab dirty scope

        private readonly struct PrefabDirtyScope : IDisposable
        {
            private readonly GameObject _prefab;
            private readonly bool _madeDirty;

            public PrefabDirtyScope(GameObject prefab)
            {
                _prefab = prefab;

#if UNITY_EDITOR && UNITY_2020_1_OR_NEWER
                _madeDirty = prefab.activeSelf && UnityEditor.EditorUtility.IsDirty(_prefab) == false;
#else
                _madeDirty = false;
#endif
            }

            public void Dispose()
            {
#if UNITY_EDITOR && UNITY_2020_1_OR_NEWER
                if (_madeDirty)
                    UnityEditor.EditorUtility.ClearDirty(_prefab);
#endif
            }
        }

        #endregion
    }
}