using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.Analytics;
using Sotbify.MobileApp.Chat;
using Sotbify.MobileApp.ChatMessages;
using Sotbify.MobileApp.Services.TutorialService;
using Sotbify.MobileApp.TutorialWindows.ChatNotificationTutorialOverlay;
using Sotbify.MobileApp.TutorialWindows.ChatTutorialOverlay;
using Sotbify.MobileApp.UI.Common.Providers;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils.Enums;
using Sotbify.MobileApp.Utils.Extensions;
using UnityEngine;
using UState;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.ApplicationUI.States
{
    public class ChatWindowState : State<IApplicationUI, ChatWindowState.Payload>
    {
        public struct Payload
        {
            public bool IsFromOverlay;
        }

        private static readonly TimeSpan AdditionalPaddingUpdateInterval = TimeSpan.FromSeconds(0.1f);
        
        private readonly IWindowService _windowService;
        private readonly IChat _chat;
        private readonly IApplicationUI _applicationUI;
        private readonly IChatMessagesStream _chatMessagesStream;
        private readonly IRootsRectProvider _rootsRectProvider;
        private readonly ITutorialStepService _tutorialStepService;
        private readonly AnalyticsEvents _analyticsEvents;
        private readonly IChatMessages _chatMessages;
        private readonly ILogger _logger;

        private IDisposable _disposable;

        private Guid _guid;

        private bool _shouldCloseOnExit = true;

        public ChatWindowState
        (
            IWindowService windowService,
            IChat chat,
            IChatMessages chatMessages,
            IApplicationUI applicationUI,
            IChatMessagesStream chatMessagesStream,
            IRootsRectProvider rootsRectProvider,
            ITutorialStepService tutorialStepService,
            AnalyticsEvents analyticsEvents,
            ILogger logger
        )
        {
            _windowService = windowService ?? throw new ArgumentNullException(nameof(windowService));
            _chat = chat ?? throw new ArgumentNullException(nameof(chat));
            _chatMessages = chatMessages ?? throw new ArgumentNullException(nameof(chatMessages));
            _applicationUI = applicationUI ?? throw new ArgumentNullException(nameof(applicationUI));
            _chatMessagesStream = chatMessagesStream ?? throw new ArgumentNullException(nameof(chatMessagesStream));
            _rootsRectProvider = rootsRectProvider ?? throw new ArgumentNullException(nameof(rootsRectProvider));
            _tutorialStepService = tutorialStepService ?? throw new ArgumentNullException(nameof(tutorialStepService));
            _analyticsEvents = analyticsEvents ?? throw new ArgumentNullException(nameof(analyticsEvents));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async UniTask ExecuteAsync(Payload payload, CancellationToken cancellationToken)
        {
            _shouldCloseOnExit = true;

            if (payload.IsFromOverlay == false)
            {
                _guid = Guid.NewGuid();
                _analyticsEvents.OpenCategory("chat", _guid);
            }

            _chatMessagesStream.Start();

            var openChatTask = _windowService.SafeOpenAsync<IChat>(cancellationToken);
            var openChatMessagesTask = _windowService.SafeOpenAsync<IChatMessages>
            (
                _rootsRectProvider.BackgroundBlurRoot,
                cancellationToken: cancellationToken
            );
            
            await openChatTask;

            var chatGameObject = _windowService.GetView<IChat>();
            if (chatGameObject.TryGetComponent<ChatView>(out var chatView))
            {
                Observable.Interval(AdditionalPaddingUpdateInterval)
                    .Select(chatView, static (_, chatView) => chatView.MessageInputSize)
                    .Prepend(chatView.MessageInputSize)
                    .Subscribe(_chatMessages, static (size, chatMessages) =>
                    {
                        var paddingBottom = Mathf.RoundToInt(size.y);
                        chatMessages.SetAdditionalContentPaddingBottom(paddingBottom);
                    })
                    .AddTo(cancellationToken);
            }

            await openChatMessagesTask;

            var isHasChatTutorialStep = _tutorialStepService.HasStep(ETutorialStep.Chat);
            var isHasChatNotificationTutorialStep = _tutorialStepService.HasStep(ETutorialStep.ChatNotification);
            
            if (isHasChatTutorialStep)
            {
                await _windowService.SafeOpenAsync<IChatTutorialOverlay>(_rootsRectProvider.TutorialRoot, cancellationToken:cancellationToken);
                _tutorialStepService.TutorialStepPassed(ETutorialStep.Chat);
            }
            
            if (isHasChatTutorialStep == false && isHasChatNotificationTutorialStep)
            {
                await _windowService.SafeOpenAsync<IChatNotificationTutorialOverlay>(_rootsRectProvider.TutorialRoot, cancellationToken:cancellationToken);
                _tutorialStepService.TutorialStepPassed(ETutorialStep.ChatNotification);
            }

            var blackjackModel = await _chatMessages.PlayBlackjackRequested
                .FirstAsync(cancellationToken)
                .AsUniTask();

            if (blackjackModel != null)
            {
                _shouldCloseOnExit = false;
                _applicationUI.RequestTransition<BlackjackWindowState, BlackjackWindowState.Payload>(new BlackjackWindowState.Payload
                {
                    BlackjackModel = blackjackModel,
                    ShouldContinue = true
                });
            }
        }

        protected override UniTask ExitAsync(CancellationToken cancellationToken)
        {
            _chat.Close();

            if (_shouldCloseOnExit == false)
                return UniTask.CompletedTask;

            _analyticsEvents.CloseCategory("chat", _guid);

            _windowService.SafeCloseAsync<IChat>(CancellationToken.None)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to close chat window", this);
            
            _windowService.SafeCloseAsync<IChatTutorialOverlay>(CancellationToken.None)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to close chat tutorial overlay", this);

            _windowService.SafeCloseAsync<IChatMessages>(CancellationToken.None)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to close chat messages window", this);

            return UniTask.CompletedTask;
        }
    }
}