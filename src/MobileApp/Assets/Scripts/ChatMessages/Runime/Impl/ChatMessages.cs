using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.ChatMessages.Models;
using Sotbify.MobileApp.Server;
using Sotbify.MobileApp.Utils.Enums.Blackjack;

namespace Sotbify.MobileApp.ChatMessages
{
    public class ChatMessages : IChatMessages, IDisposable
    {
        private readonly SemaphoreSlim _sendMessageSemaphore = new(1, 1);
        private readonly Subject<(BlackjackModel, EGameResult)> _blackjackResult = new();
        private readonly ReactiveProperty<int> _additionalContentPaddingBottom = new(0);
        
        private readonly Client _client;
        private readonly IChatItemsRepository _chatItemsRepository;
        private readonly ILogger _logger;
        
        public ChatMessages
        (
            Client client,
            IChatItemsRepository chatItemsRepository,
            ILogger logger
        )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _chatItemsRepository = chatItemsRepository ?? throw new ArgumentNullException(nameof(chatItemsRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public Observable<int> UserSentMessage => _chatItemsRepository.ScrollToObservable;
        public Observable<Unit> CancelActiveBlackjack => _chatItemsRepository.CancelActiveBlackjack;
        public IReadOnlyObservableList<IChatItemModel> Messages => _chatItemsRepository.LoadedItems;
        public Observable<BlackjackModel> PlayBlackjackRequested { get; set; }
        public Observable<(BlackjackModel, EGameResult)> BlackjackResult => _blackjackResult;
        public ReadOnlyReactiveProperty<int> AdditionalContentPaddingBottom => _additionalContentPaddingBottom;

        public void ChangeBlackjackResult(BlackjackModel model, EGameResult gameResult)
        {
            _blackjackResult.OnNext((model, gameResult));
        }
        
        public async UniTask<bool> TryLoadHistoryAsync(CancellationToken cancellationToken = default)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.Debug("Loading history", context: this);
            
            var isHistoryLoaded = await _chatItemsRepository.TryLoadMoreAsync(cancellationToken);

            if (isHistoryLoaded == false) 
                return false;
            
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.Debug("Reading messages", context: this);
                
            await _chatItemsRepository.ReadMessagesAsync(cancellationToken);

            return true;
        }

        public async UniTask SendMessageAsync(string viewMessage, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(viewMessage))
                throw new ArgumentException("Can not be null or empty", nameof(viewMessage));
            
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.Debug($"Pending message: {viewMessage}", context: this);

            await _sendMessageSemaphore.WaitAsync(cancellationToken);
            try
            {
                await SendMessageInternal(viewMessage, cancellationToken);
            }
            finally
            {
                _sendMessageSemaphore.Release();
            }
        }

        public void LastVisibleItemUpdated(int index) => 
            _chatItemsRepository.LastSeenMessageUpdated(index);

        public void SetAdditionalContentPaddingBottom(int paddingBottom) => 
            _additionalContentPaddingBottom.Value = paddingBottom;

        public void Dispose()
        {
            _blackjackResult.Dispose();
            _additionalContentPaddingBottom.Dispose();
        }

        private async UniTask SendMessageInternal(string message, CancellationToken cancellationToken)
        {
            var response = await _client.SendChatMessageAsync(message, cancellationToken);
            
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.Debug($"Message sent: {message}", context: this);
            
            await _chatItemsRepository.AddToEnd(response, cancellationToken, true);
        }
    }
}