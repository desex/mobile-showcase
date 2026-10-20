using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Sotbify.MobileApp.Addressables
{
    public interface IResourceLocationResolver
    {
        UniTask<IReadOnlyList<IResourceLocation>> ResolveAsync(object key, Type type = null, CancellationToken cancellationToken = default);
    }
}