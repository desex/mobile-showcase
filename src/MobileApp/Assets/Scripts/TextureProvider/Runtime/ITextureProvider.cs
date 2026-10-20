using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Sotbify.MobileApp.TextureProvider
{
    public interface ITextureProvider
    {
        UniTask<Texture2D> LoadTextureAsync(FileInfo file, IProgress<float> progress = null, CancellationToken cancellationToken = default);
        UniTask<Texture2D> LoadTextureAsync(Uri url, IProgress<float> progress = null, CancellationToken cancellationToken = default);
    }
}