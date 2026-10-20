using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbifay.MobileApp.Assets;
using Sotbify.MobileApp.Db.RelicInstancesConfig;
using Sotbify.MobileApp.ItemRenderer;
using UnityEngine;
using UnityEngine.Pool;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.Relic
{
    public sealed class RelicInstance : BaseRelicInstance
    {
        private readonly string _key;
        private readonly IRelicInstancesConfig _relicsConfig;
        private readonly IAssetsProvider _assetsProvider;
        private readonly IObjectPool<IItemRenderer> _itemRendererPool;

        private IItemRenderer _itemRenderer;
        private GameObject _itemInstance;

        public RelicInstance
        (
            string key,
            Color backgroundColor,
            IRelicInstancesConfig relicsConfig,
            IAssetsProvider assetsProvider,
            IObjectPool<IItemRenderer> itemRendererPool,
            ILogger logger
        ) : base(backgroundColor, logger)
        {
            _key = key ?? throw new ArgumentNullException(nameof(key));
            _relicsConfig = relicsConfig ?? throw new ArgumentNullException(nameof(relicsConfig));
            _assetsProvider = assetsProvider ?? throw new ArgumentNullException(nameof(assetsProvider));
            _itemRendererPool = itemRendererPool ?? throw new ArgumentNullException(nameof(itemRendererPool));
        }

        protected override async UniTask<Texture> LoadInternalAsync(CancellationToken cancellationToken)
        {
            var relicInstanceInfos = _relicsConfig.Instances;
            if (relicInstanceInfos.TryGetValue(_key, out var instanceInfo) == false)
                throw new InvalidOperationException($"Instance info for {_key} not found");

            var itemRenderer = _itemRendererPool.Get();
            GameObject itemInstance = null;

            try
            {
                itemInstance = await _assetsProvider.InstantiateAsync
                (
                    instanceInfo.AssetReference,
                    itemRenderer.Container,
                    cancellationToken: CancellationToken.None
                );

                cancellationToken.ThrowIfCancellationRequested();

                _itemRenderer = itemRenderer;
                _itemInstance = itemInstance;
                
                return itemRenderer.Texture;
            }
            catch
            {
                if (itemInstance != null)
                    _assetsProvider.ReleaseInstance(itemInstance);

                _itemRendererPool.Release(itemRenderer);
                throw;
            }
        }

        protected override void OnDispose()
        {
            if (_itemRenderer != null)
                _itemRenderer.RenderingIsActive = false;
            
            if (_itemInstance != null)
            {
                _assetsProvider.ReleaseInstance(_itemInstance);
                _itemInstance = null;
            }
            
            if (_itemRenderer != null)
            {
                _itemRendererPool.Release(_itemRenderer);
                _itemRenderer = null;
            }
        }
    }
}