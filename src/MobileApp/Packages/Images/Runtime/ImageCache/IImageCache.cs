using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Sotbifay.Images
{
    public interface IImageCache
    {
        UniTask<Texture2D> LoadTextureOrNullAsync<TState>
        (
            string key,
            TState state,
            Func<TState, FileStream, IProgress<float>, CancellationToken, UniTask> writeImageTaskFactory,
            IProgress<float> progress = null,
            CancellationToken cancellationToken = default
        );
    }
}