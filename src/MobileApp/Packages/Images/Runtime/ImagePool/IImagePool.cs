using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Sotbifay.Images
{
    public interface IImagePool
    {
        UniTask<Texture2D> LoadAsync(FileInfo file, IProgress<float> progress = null, CancellationToken cancellationToken = default);
        UniTask<Texture2D> LoadAsync(Uri url, IProgress<float> progress = null, CancellationToken cancellationToken = default);

        void Release(FileInfo file);
        void Release(Uri url);
    }
}