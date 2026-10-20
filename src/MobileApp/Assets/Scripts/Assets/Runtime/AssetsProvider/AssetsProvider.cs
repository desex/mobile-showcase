using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Addressables;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceLocations;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;
using UnityAddressables = UnityEngine.AddressableAssets.Addressables;

namespace Sotbifay.MobileApp.Assets
{
    public sealed class AssetsProvider : IAssetsProvider
    {
        private readonly IResourceLocationResolver _resourceLocationResolver;
        private readonly ILogger _logger;

        public AssetsProvider(IResourceLocationResolver resourceLocationResolver, ILogger logger)
        {
            _resourceLocationResolver = resourceLocationResolver ?? throw new ArgumentNullException(nameof(resourceLocationResolver));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async UniTask<bool> UpdateCatalogAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var catalogsToUpdate = await UnityAddressables
                    .CheckForCatalogUpdates()
                    .ToUniTask(cancellationToken: cancellationToken);

                if (catalogsToUpdate is null || catalogsToUpdate.Count == 0)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                        _logger.Information("Catalog does not need to be updated", context: this);

                    return true;
                }

                if (_logger.IsEnabled(LogLevel.Information))
                    _logger.Information($"Found {catalogsToUpdate.Count} catalog(s) to update", context: this);

                var updatedLocators = await UnityAddressables
                    .UpdateCatalogs(catalogsToUpdate)
                    .ToUniTask(cancellationToken: cancellationToken);

                if (_logger.IsEnabled(LogLevel.Information))
                    _logger.Information($"Updated {updatedLocators.Count} catalog(s)", context: this);

                return true;
            }
            catch (Exception e)
            {
                _logger.Error("Failed to update catalog", exception: e, context: this);
                return false;
            }
        }

        public async UniTask<long> GetDownloadSizeAsync(object key, CancellationToken cancellationToken = default)
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));

            var location = await ResolveLocationOrNullAsync(key, null, cancellationToken: cancellationToken);
            if (location is null)
                throw new InvalidOperationException($"Resource location for \"{key}\" not found");
            
            var handle = UnityAddressables.GetDownloadSizeAsync(location);
            try
            {
                var size = await handle.ToUniTask(cancellationToken: cancellationToken);

                if (_logger.IsEnabled(LogLevel.Debug))
                    _logger.Debug($"Download size for \"{key}\": {size} bytes", context: this);

                return size;
            }
            finally
            {
                if (handle.IsValid())
                    UnityAddressables.Release(handle);
            }
        }

        public async UniTask<TObject> LoadAsync<TObject>
        (
            object key,
            IProgress<float> progress = null,
            CancellationToken cancellationToken = default
        )
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.Debug($"Loading asset \"{key}\"", context: this);

            var location = await ResolveLocationOrNullAsync<TObject>(key, cancellationToken);
            if (location is null)
                throw new InvalidOperationException($"Resource location for \"{key}\" not found");
            
            cancellationToken.ThrowIfCancellationRequested();
            
            var asset = await UnityAddressables
                .LoadAssetAsync<TObject>(location)
                // ReSharper disable once MethodSupportsCancellation
                .ToUniTask(progress: progress);
            
            // ReSharper disable once InvertIf
            if (cancellationToken.IsCancellationRequested)
            {
                ReleaseAsset(asset);
                cancellationToken.ThrowIfCancellationRequested();
            }
            
            return asset;
        }

        public async UniTask<GameObject> InstantiateAsync
        (
            object key,
            Transform parent = null,
            bool instantiateInWorldSpace = false,
            IProgress<float> progress = null,
            CancellationToken cancellationToken = default
        )
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.Debug($"Instantiate asset \"{key}\"", context: this);

            var location = await ResolveLocationOrNullAsync<GameObject>(key, cancellationToken);
            if (location is null)
                throw new InvalidOperationException($"Resource location for \"{key}\" not found");
            
            cancellationToken.ThrowIfCancellationRequested();
            
            var instance = await UnityAddressables
                .InstantiateAsync(location, parent, instantiateInWorldSpace)
                // ReSharper disable once MethodSupportsCancellation
                .ToUniTask(progress: progress);
            
            // ReSharper disable once InvertIf
            if (cancellationToken.IsCancellationRequested)
            {
                ReleaseInstance(instance);
                cancellationToken.ThrowIfCancellationRequested();
            }
            
            return instance;
        }

        public async UniTask<GameObject> InstantiateAsync
        (
            object key,
            Vector3 position,
            Quaternion rotation,
            Transform parent = null,
            IProgress<float> progress = null,
            CancellationToken cancellationToken = default
        )
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.Debug($"Instantiate asset \"{key}\"", context: this);

            var location = await ResolveLocationOrNullAsync<GameObject>(key, cancellationToken);
            if (location is null)
                throw new InvalidOperationException($"Resource location for \"{key}\" not found");
            
            cancellationToken.ThrowIfCancellationRequested();
            
            var instance = await UnityAddressables
                .InstantiateAsync(location, position, rotation, parent)
                .ToUniTask(progress: progress, cancellationToken: cancellationToken);
            
            // ReSharper disable once InvertIf
            if (cancellationToken.IsCancellationRequested)
            {
                ReleaseInstance(instance);
                cancellationToken.ThrowIfCancellationRequested();
            }

            return instance;
        }

        public void ReleaseAsset<TObject>(TObject asset)
        {
            if (asset is null)
                throw new ArgumentNullException(nameof(asset));

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.Debug("Release asset", context: this);

            UnityAddressables.Release(asset);
        }

        public void ReleaseInstance(GameObject instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.Debug($"Release instance \"{instance.name}\"", context: this);

            UnityAddressables.ReleaseInstance(instance);
        }

        private UniTask<IResourceLocation> ResolveLocationOrNullAsync<TObject>(object key, CancellationToken cancellationToken) =>
            ResolveLocationOrNullAsync(key, typeof(TObject), cancellationToken);

        private async UniTask<IResourceLocation> ResolveLocationOrNullAsync(object key, Type type, CancellationToken cancellationToken)
        {
            var resourceLocations = await _resourceLocationResolver.ResolveAsync(key, type, cancellationToken);
            switch (resourceLocations.Count)
            {
                case 0:
                    return null;

                case > 1 when _logger.IsEnabled(LogLevel.Warning):
                    _logger.Warning($"Found multiple locations for asset. Key: {key}; Count: {resourceLocations.Count};");
                    break;
            }

            var resourceLocation = resourceLocations[0];
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.Debug($"Found location. Key: {key}; Internal ID: {resourceLocation.InternalId};", context: this);

            return resourceLocation;
        }
    }
}