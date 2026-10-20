using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbifay.Images;
using UnityEngine;
using UnityEngine.Networking;

namespace Sotbify.MobileApp.TextureProvider
{
    public sealed class TextureProvider : ITextureProvider, IDisposable
    {
        private const int MaxActiveLoadTextureCount = 1;

        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly SemaphoreSlim _loadTextureSemaphore = new(MaxActiveLoadTextureCount, MaxActiveLoadTextureCount);
        private readonly ConcurrentDictionary<string, AsyncLazy<Texture2D>> _textureCache = new();

        private readonly IImageCache _imageCache;

        public TextureProvider(IImageCache imageCache)
        {
            _imageCache = imageCache ?? throw new ArgumentNullException(nameof(imageCache));
        }

        public async UniTask<Texture2D> LoadTextureAsync
        (
            FileInfo file,
            IProgress<float> progress = null,
            CancellationToken cancellationToken = default
        )
        {
            if (file is null)
                throw new ArgumentNullException(nameof(file));

            if (file.Exists == false)
                throw new FileNotFoundException("File not found", file.FullName);

            return await _textureCache.GetOrAdd
                (
                    file.FullName,
                    static (key, state) =>
                    {
                        var (@this, file, progress, cancellationToken) = state;
                        return @this.LoadTextureFromFileAsync(key, file, progress, cancellationToken)
                            .ToAsyncLazy();
                    },
                    (this, file, progress, _cancellationTokenSource.Token)
                )
                .Task
                .AttachExternalCancellation(cancellationToken);
        }

        public async UniTask<Texture2D> LoadTextureAsync(Uri url, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            if (url is null)
                throw new ArgumentNullException(nameof(url));

            return await _textureCache.GetOrAdd
                (
                    url.AbsoluteUri,
                    static (key, state) =>
                    {
                        var (@this, url, progress, cancellationToken) = state;
                        return @this.LoadTextureFromUrlAsync(key, url, progress, cancellationToken)
                            .ToAsyncLazy();
                    },
                    (this, url, progress, _cancellationTokenSource.Token)
                )
                .Task
                .AttachExternalCancellation(cancellationToken);
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();

            foreach (var lazyTasks in _textureCache.Values)
            {
                var task = lazyTasks.Task;
                if (task.Status != UniTaskStatus.Succeeded)
                    continue;

                var texture = lazyTasks.GetAwaiter().GetResult();
                UnityEngine.Object.Destroy(texture);
            }

            _textureCache.Clear();
        }

        private async UniTask<Texture2D> LoadTextureFromFileAsync
        (
            string key,
            FileInfo file,
            IProgress<float> progress,
            CancellationToken cancellationToken
        )
        {
            await _loadTextureSemaphore.WaitAsync(cancellationToken);
            try
            {
                progress?.Report(0f);

                using var textureRequest = UnityWebRequestTexture.GetTexture($"file:///{file.FullName}");
                var response = await textureRequest.SendWebRequest()
                    .ToUniTask(progress: progress, cancellationToken: cancellationToken);

                return response.result is UnityWebRequest.Result.Success
                    ? DownloadHandlerTexture.GetContent(textureRequest)
                    : null;
            }
            catch (Exception)
            {
                _textureCache.TryRemove(key, out _);
                throw;
            }
            finally
            {
                _loadTextureSemaphore.Release();
            }
        }

        private async UniTask<Texture2D> LoadTextureFromUrlAsync(string key, Uri url, IProgress<float> progress, CancellationToken cancellationToken)
        {
            await _loadTextureSemaphore.WaitAsync(cancellationToken);
            try
            {
                progress?.Report(0f);
                return await _imageCache.LoadTextureOrNullAsync
                (
                    key,
                    url,
                    async static (url, fileStream, progress, cancellationToken) =>
                    {
                        progress.Report(0f);

                        using var request = UnityWebRequest.Get(url);
                        using var downloadHandlerStream = new DownloadHandlerStream(fileStream);
                        request.downloadHandler = downloadHandlerStream;

                        var response = await request.SendWebRequest()
                            .ToUniTask(progress: progress, cancellationToken: cancellationToken);

                        if (response.result != UnityWebRequest.Result.Success)
                            throw new Exception($"Failed to download image. Url: {url}, Error: {response.error}");
                    },
                    progress,
                    cancellationToken
                );
            }
            catch (Exception)
            {
                _textureCache.TryRemove(key, out _);
                throw;
            }
            finally
            {
                _loadTextureSemaphore.Release();
            }
        }

        #region DownloadHandler

        private sealed class DownloadHandlerStream : DownloadHandlerScript
        {
            private readonly Stream _stream;

            public DownloadHandlerStream(Stream stream, long bufferSize = 1024 * 64) : base(new byte[bufferSize])
            {
                _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            }

            protected override bool ReceiveData(byte[] receivedData, int dataLength)
            {
                _stream.Write(receivedData, 0, dataLength);
                return true;
            }
        }

        #endregion
    }
}