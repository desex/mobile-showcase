using ObservableCollections;
using Sotbify.MobileApp.ChatMessages.Models;


namespace Stab.StabChatWindow
{
    public class StabMessagesRepository : IStabMessagesRepository
    {
        private readonly ObservableList<UserMessageModel> _messages = new();
        public IReadOnlyObservableList<UserMessageModel> Messages => _messages;
        
        public void AddToEnd(UserMessageModel userMessageModel)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void Clear()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}