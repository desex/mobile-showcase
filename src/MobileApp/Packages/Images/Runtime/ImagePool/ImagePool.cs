using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Sotbifay.Images
{
    public sealed class ImagePool : IImagePool, IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly ConcurrentDictionary<string, ImagePoolEntry> _entries = new();

        private readonly IImageCache _imageCache;

        private int _isDisposed;

        public ImagePool(IImageCache imageCache)
        {
            _imageCache = imageCache ?? throw new ArgumentNullException(nameof(imageCache));
        }

        public async UniTask<Texture2D> LoadAsync(FileInfo file, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            ThrowExceptionIfDisposed();

            if (file is null)
                throw new ArgumentNullException(nameof(file));

            if (file.Exists == false)
                throw new FileNotFoundException("File not found", file.FullName);

            try
            {
                var entry = _entries.GetOrAdd
                (
                    file.FullName,
                    static (key, file) => new ImagePoolEntry(key, file.ReadAsTextureOrNullAsync),
                    file
                );

                using var cancellationTokenSource =
                    CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);
                return await entry.LoadAsync(progress, cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ImagePoolLoadException("Failed to load image", e);
            }
        }

        public async UniTask<Texture2D> LoadAsync(Uri url, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            ThrowExceptionIfDisposed();

            if (url is null)
                throw new ArgumentNullException(nameof(url));

            try
            {
                var entry = _entries.GetOrAdd
                (
                    url.AbsoluteUri,
                    static (key, state) =>
                    {
                        var (imageCache, url) = state;
                        return new ImagePoolEntry(key, DownloadAsync);

                        UniTask<Texture2D> DownloadAsync(IProgress<float> progress, CancellationToken cancellationToken)
                        {
                            return imageCache.LoadTextureOrNullAsync
                            (
                                key,
                                url,
                                static (url, fileStream, progress, cancellationToken) =>
                                    url.DownloadToStreamAsync(fileStream, progress, cancellationToken),
                                progress,
                                cancellationToken
                            );
                        }
                    },
                    (_imageCache, url)
                );

                using var cancellationTokenSource =
                    CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);
                return await entry.LoadAsync(progress, cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ImagePoolLoadException("Failed to load image", e);
            }
        }

        public void Release(FileInfo file)
        {
            if (file is null)
                throw new ArgumentNullException(nameof(file));

            try
            {
                ReleaseByKey(file.FullName);
            }
            catch (Exception e)
            {
                throw new ImagePoolReleaseException("Failed to release image", e);
            }
        }

        public void Release(Uri url)
        {
            if (url is null)
                throw new ArgumentNullException(nameof(url));

            try
            {
                ReleaseByKey(url.AbsoluteUri);
            }
            catch (Exception e)
            {
                throw new ImagePoolReleaseException("Failed to release image", e);
            }
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _isDisposed, 1) == 1)
                return;

            _cancellationTokenSource.Cancel();

            foreach (var entry in _entries.Values)
                entry.Dispose();

            _entries.Clear();
            
            _cancellationTokenSource.Dispose();
        }

        private void ThrowExceptionIfDisposed()
        {
            if (Volatile.Read(ref _isDisposed) == 1)
                throw new ObjectDisposedException(nameof(ImagePool));
        }

        private void ReleaseByKey(string key)
        {
            if (_entries.TryGetValue(key, out var entry) == false)
                return;

            entry.Release();
        }
    }
}