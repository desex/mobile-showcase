using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.ChatMessages;
using VContainer.Unity;

namespace Sotbify.MobileApp.Chat.MessageSender
{
    public class MessageSenderPresenter : IInitializable, IDisposable
    {
        private readonly MessageSenderView _view;
        private readonly IChatMessages _chatMessages;
        private readonly ILogger _logger;

        private IDisposable _bindings;

        public MessageSenderPresenter
        (
            MessageSenderView view,
            IChatMessages chatMessages,
            ILogger logger
        )
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _chatMessages = chatMessages ?? throw new ArgumentNullException(nameof(chatMessages));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initialize()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void OnMessageChanged()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private async UniTask OnSendMessageAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void Dispose()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}