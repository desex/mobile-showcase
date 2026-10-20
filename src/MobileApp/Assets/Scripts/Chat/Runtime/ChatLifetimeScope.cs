using System.Collections.Generic;
using Sotbify.MobileApp.Chat.MessageSender;
using Sotbify.MobileApp.ChatMessages;
using Sotbify.MobileApp.ChatMessages.Units;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.Chat
{
    public class ChatLifetimeScope : LifetimeScope
    {
        [SerializeField] private MessageSenderView _messagesSenderView;
        [SerializeField] private List<NavbarButton> _fastMessageButtons = new();
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint(resolver =>
            {
                var logger = resolver.Resolve<ILogger>();
                var chatMessages = resolver.Resolve<IChatMessages>();
                return new MessageSenderPresenter(_messagesSenderView, chatMessages, logger);
            }, Lifetime.Singleton);
            
            builder.RegisterInstance<IReadOnlyList<NavbarButton>>(_fastMessageButtons);
        }
    }
}