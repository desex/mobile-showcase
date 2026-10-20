using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Sotbify.MobileApp.UI.Common.Texture
{
    public interface ITextureConverter
    {
        UniTask<Texture2D> CreateTextureFromBytesAsync(byte[] bytes, CancellationToken cancellationToken = default);

        UniTask<Texture2D> CreateTextureFromMemoryAsync(ReadOnlyMemory<byte> bytes,
            CancellationToken cancellationToken = default);

        UniTask<Texture2D> CreateTextureFromFileAsync(string filePath, CancellationToken cancellationToken = default);

        UniTask SaveTextureToFileAsync(Texture2D texture, string savePath,
            CancellationToken cancellationToken = default);
    }
}