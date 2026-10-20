using System;
using R3;
using Sotbify.MobileApp.ApplicationUI;
using Sotbify.MobileApp.ApplicationUI.States;
using Sotbify.MobileApp.ChatMessages;
using VContainer.Unity;

namespace Sotbify.MobileApp.Badge.Chat
{
    public sealed class ChatBadgePresenter : IInitializable, IDisposable
    {
        private readonly IApplicationUI _applicationUI;
        private readonly IChatItemsRepository _chatItemsRepository;
        private readonly IBadgeView _badgeView;

        private IDisposable _bindings;

        public ChatBadgePresenter
        (
            IApplicationUI applicationUI,
            IChatItemsRepository chatItemsRepository,
            IBadgeView badgeView
        )
        {
            _applicationUI = applicationUI ?? throw new ArgumentNullException(nameof(applicationUI));
            _chatItemsRepository = chatItemsRepository ?? throw new ArgumentNullException(nameof(chatItemsRepository));
            _badgeView = badgeView ?? throw new ArgumentNullException(nameof(badgeView));
        }

        public void Initialize()
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