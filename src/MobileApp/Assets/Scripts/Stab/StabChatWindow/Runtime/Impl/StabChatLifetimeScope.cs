using Stab.StabChatWindow.MessageSender;
using UnityEngine;
using VContainer;
using VContainer.Unity;


namespace Stab.StabChatWindow
{
    public class StabChatLifetimeScope : LifetimeScope
    {
        [SerializeField] private StabMessageSenderView _stabMessagesSenderView;

        protected override void Configure(IContainerBuilder builder)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}