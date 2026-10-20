using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Sotbifay.MobileApp.Assets
{
    public interface IAssetsProvider
    {
        UniTask<bool> UpdateCatalogAsync(CancellationToken cancellationToken = default);
        
        UniTask<long> GetDownloadSizeAsync(object key, CancellationToken cancellationToken = default);
        
        UniTask<TObject> LoadAsync<TObject>(object key, IProgress<float> progress = null, CancellationToken cancellationToken = default);

        UniTask<GameObject> InstantiateAsync
        (
            object key,
            Transform parent = null,
            bool instantiateInWorldSpace = false,
            IProgress<float> progress = null,
            CancellationToken cancellationToken = default
        );

        UniTask<GameObject> InstantiateAsync
        (
            object key,
            Vector3 position,
            Quaternion rotation,
            Transform parent = null,
            IProgress<float> progress = null,
            CancellationToken cancellationToken = default
        );

        void ReleaseAsset<TObject>(TObject asset);

        void ReleaseInstance(GameObject instance);
    }
}