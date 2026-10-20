using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using Sotbify.MobileApp.ChatMessages.Models;
using Sotbify.MobileApp.Common.OSA;
using Sotbify.MobileApp.Db.ChatParameters;
using Sotbify.MobileApp.UI.Window.Presenter;
using Sotbify.MobileApp.Utils.Enums.Blackjack;
using Sotbify.MobileApp.Utils.Extensions;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.ChatMessages
{
    public class ChatMessagesPresenter : WindowPresenter<IChatMessages, ChatMessagesView>
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        private readonly IChatParameters _chatParameters;
        private readonly ILogger _logger;

        private IDisposable _bindings;
        private IDisposable _messagesBindings;

        public ChatMessagesPresenter(IChatParameters chatParameters, ILogger logger)
        {
            _chatParameters = chatParameters ?? throw new ArgumentNullException(nameof(chatParameters));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override void Initialize()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override async UniTask AfterShowAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override void OnDispose()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private async UniTask LoadHistoryAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void ApplyBlackjackResult(BlackjackModel model, EGameResult gameResult)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        private void ScrollToBottom()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}