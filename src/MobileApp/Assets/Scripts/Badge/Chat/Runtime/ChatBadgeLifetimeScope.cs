using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Sotbify.MobileApp.Badge.Chat
{
    public sealed class ChatBadgeLifetimeScope : LifetimeScope
    {
        [SerializeField] private ChatBadgeView _chatBadgeView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<ChatBadgePresenter>()
                .WithParameter<IBadgeView>(_chatBadgeView);
        }
    }
}