using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Sotbifay.Images
{
    internal sealed class ImagePoolEntry : IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly SemaphoreSlim _loadSemaphore = new(1, 1);
        private readonly object _syncRoot = new();
        private readonly Progress _progress = new();

        private readonly Func<IProgress<float>, CancellationToken, UniTask<Texture2D>> _loadImageTaskFactory;

        private int _isDisposed = 0;
        private int _referenceCount = 0;
        private Texture2D _texture = null;

        public ImagePoolEntry
        (
            string key,
            Func<IProgress<float>, CancellationToken, UniTask<Texture2D>> loadImageTaskFactory
        )
        {
            Key = string.IsNullOrEmpty(key) == false ? key : throw new ArgumentException("Cannot be null or empty", nameof(key));
            _loadImageTaskFactory = loadImageTaskFactory ?? throw new ArgumentNullException(nameof(loadImageTaskFactory));
        }
        
        public string Key { get; }

        public bool HasReferences
        {
            get
            {
                lock (_syncRoot)
                    return _referenceCount > 0;
            }
        }
        
        public async UniTask<Texture2D> LoadAsync(IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            ThrowExceptionIfDisposed();
            try
            {
                ReportProgress(_progress.Value);
                _progress.OnReport += ReportProgress;
                return await LoadImageInternalAsync(cancellationToken);
            }
            finally
            {
                _progress.OnReport -= ReportProgress;
            }

            void ReportProgress(float value) =>
                progress?.Report(value);
        }

        public void Release()
        {
            ThrowExceptionIfDisposed();
            
            Texture2D texture = null;
            lock (_syncRoot)
            {
                if (_referenceCount <= 0)
                    throw new InvalidOperationException($"Over-release detected. Key: {Key};");
                
                _referenceCount--;

                if (_referenceCount > 0)
                    return;
                
                texture = _texture;
                _texture = null;
            }
            
            if (texture != null)
                UnityEngine.Object.Destroy(texture);
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _isDisposed, 1) == 1)
                return;

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();

            Texture2D texture = null;
            lock (_syncRoot)
            {
                _referenceCount = 0;
                
                texture = _texture;
                _texture = null;
            }
            
            if (texture != null)
                UnityEngine.Object.Destroy(texture);
        }

        private void ThrowExceptionIfDisposed()
        {
            if (Volatile.Read(ref _isDisposed) == 1)
                throw new ObjectDisposedException($"{nameof(ImagePoolEntry)}. Key: {Key};");
        }

        private async UniTask<Texture2D> LoadImageInternalAsync(CancellationToken cancellationToken)
        {
            lock (_syncRoot) 
                _referenceCount++;

            try
            {
                lock (_syncRoot)
                {
                    if(_texture != null)
                        return _texture;
                }
                
                using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);
                await _loadSemaphore.WaitAsync(cancellationTokenSource.Token);
                try
                {
                    lock (_syncRoot)
                    {
                        if(_texture != null)
                            return _texture;
                    }

                    var texture = await _loadImageTaskFactory.Invoke(_progress, cancellationTokenSource.Token);

                    Texture2D toDestroy = null;
                    lock (_syncRoot)
                    {
                        if (_referenceCount > 0 && Volatile.Read(ref _isDisposed) == 0)
                        {
                            _texture = texture;
                        }
                        else
                        {
                            toDestroy = texture;
                        }
                    }

                    if (toDestroy != null)
                        UnityEngine.Object.Destroy(toDestroy);

                    return texture;
                }
                finally
                {
                    _loadSemaphore.Release();
                }
            }
            catch
            {
                lock (_syncRoot)
                {
                    if (_referenceCount > 0)
                        _referenceCount--;
                }
                
                throw;
            }
        }

        #region Progress

        private sealed class Progress : IProgress<float>
        {
            public float Value { get; private set; } = 0;
            public event Action<float> OnReport;

            public void Report(float value)
            {
                Value = value;
                OnReport?.Invoke(value);
            }
        }

        #endregion
    }
}