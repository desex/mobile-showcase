using System;
using System.Globalization;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.ChatMessages;
using Sotbify.MobileApp.Db.ChatParameters;
using Sotbify.MobileApp.Server;
using Sotbify.MobileApp.Utils.Data;
using Sotbify.MobileApp.Utils.Extensions;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.Chat
{
    public sealed class Chat : IChat, IDisposable
    {
        private readonly Subject<bool> _changeQuickMessagePanelVisibilityCommand = new();
        private readonly ReactiveProperty<ManagerData> _managerData = new();
        
        private readonly ILogger _logger;
        private readonly Client _client;
        private readonly IChatMessagesStream _chatMessagesStream;
        private readonly IChatItemsRepository _chatItemsRepository;
        private readonly IChatParameters _chatParameters;

        private AsyncLazy<bool> _handleLoadHistoryQueueTask;
        private AsyncLazy _handleReadQueueTask;
        private CancellationTokenSource _cancellationTokenSource;

        public Chat
        (
            Client client,
            IChatMessagesStream chatMessagesStream,
            IChatItemsRepository chatItemsRepository,
            IChatParameters chatParameters,
            ILogger logger
        )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _chatMessagesStream = chatMessagesStream ?? throw new ArgumentNullException(nameof(chatMessagesStream));
            _chatItemsRepository = chatItemsRepository ?? throw new ArgumentNullException(nameof(chatItemsRepository));
            _chatParameters = chatParameters ?? throw new ArgumentNullException(nameof(chatParameters));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public Observable<bool> ChangeQuickMessagePanelVisibilityObservable =>
            _changeQuickMessagePanelVisibilityCommand;

        public ReadOnlyReactiveProperty<ManagerData> ManagerData => _managerData;
        public ReadOnlyReactiveProperty<bool> ShouldShowLoading => _chatItemsRepository.ShouldShowLoading;

        public void Start()
        {
            _chatMessagesStream.Start();

            _cancellationTokenSource = new CancellationTokenSource();
            UpdateManagerStatusAsync(_cancellationTokenSource.Token)
                .ForgetIgnoreOperationCancelledException(_logger, "Error while updating manager status", this);
        }

        public void Close()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
        
        public void Dispose()
        {
            Close();
            _changeQuickMessagePanelVisibilityCommand?.Dispose();
            _managerData?.Dispose();
        }
        
        private async UniTask UpdateManagerStatusAsync(CancellationToken cancellationToken)
        {
            while (cancellationToken.IsCancellationRequested == false)
            {
                var schema = await _client.GetManagerInfo(cancellationToken);
                
                var onlineDateTime = DateTime.Parse(schema.OnlineTime, null, DateTimeStyles.RoundtripKind);
                _managerData.Value =  new ManagerData
                {
                    Id = schema.Id,
                    Username = schema.Username,
                    Fullname = schema.Fullname,
                    ThumbnailUrl = schema.ThumbnailUrl,
                    OnlineDate = onlineDateTime.Ticks,
                    IsOnline = schema.IsOnline
                };
                
                await UniTask.WaitForSeconds(60 * _chatParameters.ManagerStatusRequestDelayMinutes, cancellationToken: cancellationToken);    
            }
        }
    }
}