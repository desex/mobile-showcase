using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.Common.Collections;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityAddressables = UnityEngine.AddressableAssets.Addressables;

namespace Sotbify.MobileApp.Addressables
{
    public sealed class ResourceLocationResolver : IResourceLocationResolver
    {
        public async UniTask<IReadOnlyList<IResourceLocation>> ResolveAsync(object key, Type type = null, CancellationToken cancellationToken = default)
        {
            using (ListPool<object>.Get(out var keys))
            {
                if (key is IEnumerable<object> keyEnumerable)
                {
                    keys.AddRange(keyEnumerable);
                }
                else
                {
                    keys.Add(key);
                }

                var loadResourceLocationsHandle = UnityAddressables.LoadResourceLocationsAsync
                (
                    keys,
                    UnityAddressables.MergeMode.Intersection,
                    type
                );
                try
                {
                    var resourceLocations = await loadResourceLocationsHandle.ToUniTask(cancellationToken: cancellationToken);
                    return resourceLocations.SafeToReadOnlyList();
                }
                finally
                {
                    if (loadResourceLocationsHandle.IsValid())
                        UnityAddressables.Release(loadResourceLocationsHandle);
                }
            }
        }
    }
}