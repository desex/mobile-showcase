using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Addressables;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.SceneManagement;
using UnityAddressables = UnityEngine.AddressableAssets.Addressables;

namespace Sotbifay.MobileApp.Assets
{
    public sealed class SceneLoader : ISceneLoader
    {
        private readonly IResourceLocationResolver _resourceLocationResolver;
        private readonly ILogger _logger;

        public SceneLoader(IResourceLocationResolver resourceLocationResolver, ILogger logger)
        {
            _resourceLocationResolver = resourceLocationResolver ?? throw new ArgumentNullException(nameof(resourceLocationResolver));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async UniTask<SceneInstance> LoadSceneAsync
        (
            string sceneName,
            LoadSceneMode mode = LoadSceneMode.Single,
            bool activeOnLoad = true,
            IProgress<float> progress = null,
            CancellationToken cancellationToken = default
        )
        {
            var location = await ResolveLocationAsync(sceneName, cancellationToken);
            if (location is null)
                throw new InvalidOperationException($"Resource location for \"{sceneName}\" not found");
            
            return await UnityAddressables
                .LoadSceneAsync(location, mode, activateOnLoad: activeOnLoad)
                .ToUniTask(progress: progress, cancellationToken: cancellationToken)
                .ContinueWith(instance => (SceneInstance)instance);
        }

        public UniTask UnloadSceneAsync
        (
            SceneInstance sceneInstance,
            UnloadSceneOptions options = UnloadSceneOptions.None,
            IProgress<float> progress = null,
            CancellationToken cancellationToken = default
        ) => UnityAddressables
            .UnloadSceneAsync(sceneInstance, options)
            .ToUniTask(progress: progress, cancellationToken: cancellationToken);
        
        private async UniTask<IResourceLocation> ResolveLocationAsync(object key, CancellationToken cancellationToken)
        {
            var resourceLocations = await _resourceLocationResolver.ResolveAsync(key, cancellationToken: cancellationToken);
            switch (resourceLocations.Count)
            {
                case 0:
                    return null;
                
                case > 1 when _logger.IsEnabled(LogLevel.Warning):
                    _logger.Warning($"Found multiple locations for asset. Key: {key}; Count: {resourceLocations.Count};");
                    break;
            }

            var resourceLocation = resourceLocations[0];
            if(_logger.IsEnabled(LogLevel.Debug))
                _logger.Debug($"Found location. Key: {key}; Internal ID: {resourceLocation.InternalId};", context: this);
            
            return resourceLocation;
        }
    }
}