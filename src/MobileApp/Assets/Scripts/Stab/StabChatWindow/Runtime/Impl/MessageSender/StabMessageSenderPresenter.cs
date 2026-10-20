using System;
using R3;
using VContainer.Unity;

namespace Stab.StabChatWindow.MessageSender
{
    public class StabMessageSenderPresenter : IInitializable, IDisposable
    {
        private readonly CompositeDisposable _disposable = new();
        private readonly StabMessageSenderView _view;
        private readonly IStabChat _chat;

        public StabMessageSenderPresenter(
            StabMessageSenderView view,
            IStabChat chat
        )
        {
            _view = view;
            _chat = chat;
        }

        public void Initialize()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void OnDeselect()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void OnSelect()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void OnSendMessage()
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