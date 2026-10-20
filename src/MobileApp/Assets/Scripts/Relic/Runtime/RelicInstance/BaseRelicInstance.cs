using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbifay.MobileApp.Logger;
using UnityEngine;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.Relic
{
    public abstract class BaseRelicInstance : IRelicInstance
    {
        private static readonly Texture DefaultTexture = Texture2D.blackTexture;
        
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly SemaphoreSlim _loadSemaphore = new(1, 1);
        
        private readonly ILogger _logger;

        protected BaseRelicInstance(Color backgroundColor, ILogger logger)
        {
            BackgroundColor = backgroundColor;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public Texture Texture { get; private set; } = DefaultTexture;
        public Color BackgroundColor { get; }
        public event Action Loaded;
        
        public async UniTaskVoid LoadAsync(CancellationToken cancellationToken = default)
        {
            using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);

            await _loadSemaphore.WaitAsync(cancellationTokenSource.Token);
            try
            {
                while (cancellationTokenSource.IsCancellationRequested == false)
                {
                    try
                    {
                        Texture = await LoadInternalAsync(cancellationTokenSource.Token);
                        Loaded?.Invoke();
                        break;
                    }
                    catch (InvalidOperationException)
                    {
                        break;
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception e)
                    {
                        _logger.Error("Failed to load relic", e, this);
                        await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: cancellationTokenSource.Token);
                    }
                }
            }
            finally
            {
                _loadSemaphore.Release();
            }
        }
        
        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();

            OnDispose();
            Texture = DefaultTexture;
        }

        protected abstract UniTask<Texture> LoadInternalAsync(CancellationToken cancellationToken);

        protected virtual void OnDispose()
        {
        }
    }
}