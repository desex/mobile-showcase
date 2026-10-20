using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Sotbifay.Images
{
    public interface IImageEntry : IDisposable
    {
        bool IsLoaded { get; }
        Texture2D Texture { get; }

        event Action Loaded;

        UniTask LoadAsync(IProgress<float> progress = null, CancellationToken cancellationToken = default);
    }
}