using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using R3;
using Sotbify.MobileApp.Server;
using Sotbify.MobileApp.Server.SSE;
using Sotbify.MobileApp.Utils.Extensions;
using Sotbify.MobileApp.Utils.Schemas.Chat;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.ChatMessages.ChatSSE
{
    public class ChatMessagesStream : IChatMessagesStream, IDisposable
    {
        private readonly Client _client;
        private readonly IChatItemsRepository _chatItemsRepository;
        private readonly ILogger _logger;
        private readonly Subject<Unit> _reconnectCommand = new ();
        private readonly CompositeDisposable _disposable = new ();
        
        private ISSEFacade _sseFacade;
        private bool _streamStarted;

        public ChatMessagesStream
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

        public Observable<Unit> ReconnectObservable => _reconnectCommand;

        public void Start()
        {
            if (_streamStarted)
                return;

            _streamStarted = true;
            
            _sseFacade = _client.CreateChatSse();
            
            _sseFacade.MessageObservable
                .SubscribeAwait(this, static (wrapper, @this, cancellationToken) => @this.OnMessage(wrapper,cancellationToken))
                .AddTo(_disposable);
            
            _sseFacade.ReconnectObservable
                .Subscribe(this, static(_, @this) => @this.OnReconnect())
                .AddTo(_disposable);
            
            _sseFacade.Start();

            _chatItemsRepository.StartAsync()
                .ForgetIgnoreOperationCancelledException(_logger, "Exception while starting message repository", this);
        }

        private async UniTask OnMessage(SSEFacadeWrapper data, CancellationToken cancellationToken)
        {
            var message = JsonConvert.DeserializeObject<ChatMessageResponseSchema>(data.Message);
            await _chatItemsRepository.AddToEnd(message, cancellationToken);
        }

        private void OnReconnect()
        {
            _reconnectCommand?.OnNext(Unit.Default);
        }

        public void Dispose()
        {
            _disposable?.Dispose();
            _reconnectCommand?.Dispose();
            
            if (_sseFacade is IDisposable sseFacade)
                sseFacade.Dispose();
            
            _sseFacade = null;
        }
    }
}