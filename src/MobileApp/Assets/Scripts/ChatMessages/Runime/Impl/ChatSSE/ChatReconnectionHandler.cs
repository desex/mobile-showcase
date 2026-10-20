using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbifay.MobileApp.Logger;
using VContainer.Unity;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.ChatMessages.ChatSSE
{
    public sealed class ChatReconnectionHandler : IInitializable, IDisposable
    {
        private readonly CompositeDisposable _disposable = new();
        
        private readonly IChatMessagesStream _chatMessagesStream;
        private readonly IChatItemsRepository _chatItemsRepository;
        private readonly ILogger _logger;
        private CancellationTokenSource _cancellationTokenSource = new();

        private bool _isReconnecting;
        
        public ChatReconnectionHandler(
            IChatMessagesStream chatMessagesStream,
            IChatItemsRepository chatItemsRepository,
            ILogger logger
        )
        {
            _chatMessagesStream = chatMessagesStream;
            _chatItemsRepository = chatItemsRepository;
            _logger = logger;
        }


        public void Initialize()
        {
            _chatMessagesStream.ReconnectObservable
                .SubscribeAwait(this, (_, @this, cancellationToken) => @this.OnReconnect(cancellationToken))
                .AddTo(_disposable);
        }

        private async UniTask OnReconnect(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            
            if (_isReconnecting)
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                    _logger.Warning("Messages sync process has already been started!", context: this);
                return;
            }
            
            _isReconnecting = true;
            
            await _chatItemsRepository.LoadMoreAfterReconnectAsync(_cancellationTokenSource.Token);
            await _chatItemsRepository.ReadMessagesAsync(cancellationToken: _cancellationTokenSource.Token);

            _isReconnecting = false;
        }
        
        public void Dispose()
        {
            _disposable?.Dispose();
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}