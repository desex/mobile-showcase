using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace Sotbifay.Images
{
    public abstract class BaseImageEntry : IImageEntry
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly SemaphoreSlim _loadSemaphore = new(1, 1);
        
        private readonly IImagePool _imagePool;

        private int _isDisposed;

        protected BaseImageEntry(IImagePool imagePool)
        {
            _imagePool = imagePool ?? throw new ArgumentNullException(nameof(imagePool));
        }

        public bool IsLoaded => Texture != null;
        public Texture2D Texture { get; private set; }

        public event Action Loaded;

        public async UniTask LoadAsync(IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            if (Volatile.Read(ref _isDisposed) == 1)
                return;
            
            using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);
            await _loadSemaphore.WaitAsync(cancellationTokenSource.Token);
            try
            {
                if(Texture != null)
                    return;
                
                Texture = await LoadTextureAsync(_imagePool, progress, cancellationTokenSource.Token);
                Loaded?.Invoke();
            }
            finally
            {
                _loadSemaphore.Release();
            }
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _isDisposed, 1) == 1)
                return;
            
            _cancellationTokenSource.Cancel();

            var texture = Texture;
            Texture = null;
            
            if(texture != null)
                Release(_imagePool);
            
            _cancellationTokenSource.Dispose();
        }

        protected abstract UniTask<Texture2D> LoadTextureAsync
        (
            IImagePool imagePool,
            [CanBeNull] IProgress<float> progress,
            CancellationToken cancellationToken
        );

        protected abstract void Release(IImagePool imagePool);
    }
}