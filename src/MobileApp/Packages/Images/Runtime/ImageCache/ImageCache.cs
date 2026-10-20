using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using SQLite;
using UnityEngine;

namespace Sotbifay.Images
{
    public sealed class ImageCache : IImageCache, IDisposable
    {
        private const string DatabaseFileName = nameof(ImageCache) + ".db";
        private const string TableName = "ImageCacheMetas";

        private static readonly DirectoryInfo RootDirectory = new(Path.Combine(Application.persistentDataPath, nameof(ImageCache)));
        private static readonly FileInfo DatabaseFile = new(Path.Combine(Application.persistentDataPath, DatabaseFileName));
        private static readonly TimeSpan MaxImageAge = TimeSpan.FromDays(10);

        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphores = new();
        
        private readonly SQLiteConnection _databaseConnection = new(DatabaseFile.FullName);
        private readonly object _databaseLock = new();
        
        private int _isDisposed;

        public ImageCache()
        {
            try
            {
                _databaseConnection.CreateTable<ImageCacheMeta>();
            }
            catch
            {
                _databaseConnection.Dispose();
                throw;
            }
        }
        
        public async UniTask<Texture2D> LoadTextureOrNullAsync<TState>
        (
            string key,
            TState state,
            Func<TState, FileStream, IProgress<float>, CancellationToken, UniTask> writeImageTaskFactory,
            IProgress<float> progress = null,
            CancellationToken cancellationToken = default
        )
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Cannot be null or empty", nameof(key));

            if (writeImageTaskFactory is null)
                throw new ArgumentNullException(nameof(writeImageTaskFactory));

            ThrowExceptionIfDisposed();

            using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);

            var hash = key.ComputeSha256Hash();
            var semaphore = GetSemaphore(hash);
            await semaphore.WaitAsync(cancellationTokenSource.Token);
            try
            {
                var imageFileInfo = GetImageFileInfo(hash);
                var imageMeta = GetImageMetaOrNull(hash);

                if (imageMeta is null && imageFileInfo.Exists)
                    DeleteImage(imageFileInfo);

                if (imageMeta is not null && ImageIsExpired(imageMeta))
                {
                    DeleteImageMeta(imageMeta);
                    DeleteImage(imageFileInfo);
                    imageMeta = null;
                }

                if (imageMeta is not null && imageFileInfo.Exists)
                {
                    var texture = await imageFileInfo.ReadAsTextureOrNullAsync(progress, cancellationTokenSource.Token);
                    if (texture != null)
                    {
                        UpdateImageMeta(imageMeta);
                        return texture;
                    }
                    else
                    {
                        DeleteImageMeta(imageMeta);
                        DeleteImage(imageFileInfo);
                        imageMeta = null;
                    }
                }

                if (imageFileInfo.Directory?.Exists == false)
                    imageFileInfo.Directory.Create();

                var writeProgress = new LerpProgress(progress, 0f, 0.5f);
                var writeResult = await WriteImageAsync(state, imageFileInfo, writeImageTaskFactory, writeProgress, cancellationTokenSource.Token);
                if (writeResult == false)
                    throw new IOException($"Failed to write image file. Path: {imageFileInfo.FullName};");

                var loadProgress = new LerpProgress(progress, 0.5f, 1f);
                var resultTexture = await imageFileInfo.ReadAsTextureOrNullAsync(loadProgress, cancellationTokenSource.Token);
                if (resultTexture == null)
                {
                    DeleteImage(imageFileInfo);
                    DeleteImageMeta(imageMeta);
                    throw new IOException($"Failed to read image file. Path: {imageFileInfo.FullName};");
                }

                if (imageMeta is null)
                {
                    AddImageMeta(hash);
                }
                else
                {
                    UpdateImageMeta(imageMeta);
                }

                return resultTexture;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ImageCacheException($"Failed to load image. Key: {key}; Hash: {hash};", e);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _isDisposed, 1) == 1)
                return;
            
            _cancellationTokenSource.Cancel();
            
            lock (_databaseLock) 
                _databaseConnection.Dispose();
            
            _semaphores.Clear();
            
            _cancellationTokenSource.Dispose();
        }
        
        private void ThrowExceptionIfDisposed()
        {
            if (Volatile.Read(ref _isDisposed) == 1)
                throw new ObjectDisposedException(nameof(ImageCache));
        }

        private SemaphoreSlim GetSemaphore(string hash) =>
            _semaphores.GetOrAdd(hash, _ => new SemaphoreSlim(1, 1));

        private static FileInfo GetImageFileInfo(string hash)
            => new(Path.Combine(RootDirectory.FullName, hash));

        private ImageCacheMeta GetImageMetaOrNull(string hash)
        {
            lock (_databaseLock)
            {
                return _databaseConnection.Table<ImageCacheMeta>()
                    .FirstOrDefault(x => x.Key == hash);
            }
        }

        private static bool ImageIsExpired(ImageCacheMeta imageMeta) =>
            imageMeta.LastAccessTimeUtc <= DateTime.UtcNow - MaxImageAge;

        private ImageCacheMeta AddImageMeta(string hash)
        {
            var imageCacheMeta = new ImageCacheMeta
            {
                Key = hash,
                LastAccessTimeUtc = DateTime.UtcNow
            };
            
            lock (_databaseLock) 
                _databaseConnection.Insert(imageCacheMeta);
            
            return imageCacheMeta;
        }

        private void UpdateImageMeta(ImageCacheMeta imageMeta)
        {
            imageMeta.LastAccessTimeUtc = DateTime.UtcNow;
            
            lock (_databaseLock) 
                _databaseConnection.Update(imageMeta);
        }

        private void DeleteImageMeta(ImageCacheMeta imageMeta)
        {
            if (imageMeta is null)
                return;
            
            lock (_databaseLock) 
                _databaseConnection.Delete(imageMeta);
        }

        private static async UniTask<bool> WriteImageAsync<TState>
        (
            TState state,
            FileInfo imageFileInfo,
            Func<TState, FileStream, IProgress<float>, CancellationToken, UniTask> saveImageTaskFactory,
            IProgress<float> progress,
            CancellationToken cancellationToken
        )
        {
            var tempImageFileInfo = new FileInfo($"{imageFileInfo.FullName}.temp");
            try
            {
                await using
                (
                    var imageFileStream = new FileStream
                    (
                        tempImageFileInfo.FullName,
                        FileMode.Create,
                        FileAccess.Write,
                        FileShare.None,
                        bufferSize: 64 * 1024,
                        useAsync: true
                    )
                )
                {
                    await saveImageTaskFactory.Invoke(state, imageFileStream, progress, cancellationToken);
                }

                return TryMoveFile(tempImageFileInfo, imageFileInfo);
            }
            catch (OperationCanceledException)
            {
                if (tempImageFileInfo.Exists)
                    tempImageFileInfo.Delete();
                
                throw;
            }
            catch (Exception)
            {
                if (tempImageFileInfo.Exists)
                    tempImageFileInfo.Delete();

                return false;
            }
        }

        private static void DeleteImage(FileInfo imageFileInfo)
        {
            if (imageFileInfo is null || imageFileInfo.Exists == false)
                return;

            imageFileInfo.Delete();
            imageFileInfo.Refresh();
        }

        private static bool TryMoveFile(FileInfo sourceFileInfo, FileInfo destinationFileInfo)
        {
            try
            {
                if (destinationFileInfo.Exists)
                    destinationFileInfo.Delete();

                File.Move(sourceFileInfo.FullName, destinationFileInfo.FullName);
                return true;
            }
            catch (Exception)
            {
                try
                {
                    sourceFileInfo.Refresh();
                    if (sourceFileInfo.Exists)
                        sourceFileInfo.Delete();
                }
                catch
                {
                    // Ignore 🤡
                }

                return false;
            }
        }

        #region SQL Table

        [Table(TableName)]
        private sealed class ImageCacheMeta
        {
            [PrimaryKey] public string Key { get; set; }
            public DateTime LastAccessTimeUtc { get; set; }
        }

        #endregion

        #region ProgressWithLerp
        
        private sealed class LerpProgress : IProgress<float>
        {
            private readonly IProgress<float> _progress;
            private readonly float _min;
            private readonly float _max;

            public LerpProgress(IProgress<float> progress, float min, float max)
            {
                _progress = progress;
                _min = Mathf.Max(0, min);
                _max = Mathf.Min(1f, max);
            }

            public void Report(float value) => 
                _progress?.Report(Mathf.Lerp(_min, _max, value));
        }

        #endregion
    }
}