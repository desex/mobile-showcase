using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Sotbifay.Images
{
    public static class ImageCacheExtensions
    {
        public static UniTask<Texture2D> LoadTextureOrNullAsync
        (
            this IImageCache cache,
            string key,
            Func<FileStream, IProgress<float>, CancellationToken, UniTask> writeImageTaskFactory,
            IProgress<float> progress = null,
            CancellationToken cancellationToken = default
        )
        {
            if (cache is null)
                throw new ArgumentNullException(nameof(cache));

            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Cannot be null or empty", nameof(key));

            if (writeImageTaskFactory is null)
                throw new ArgumentNullException(nameof(writeImageTaskFactory));

            return cache.LoadTextureOrNullAsync
            (
                key,
                writeImageTaskFactory,
                static (factory, fileStream, progress, cancellationToken) => factory.Invoke(fileStream, progress, cancellationToken),
                progress,
                cancellationToken
            );
        }
    }
}