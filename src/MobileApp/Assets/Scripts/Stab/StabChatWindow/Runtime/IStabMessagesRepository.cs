using ObservableCollections;
using Sotbify.MobileApp.ChatMessages.Models;

namespace Stab.StabChatWindow
{
    public interface IStabMessagesRepository
    {
        public IReadOnlyObservableList<UserMessageModel> Messages { get; }
        
        public void AddToEnd(UserMessageModel userMessageModel);
        public void Clear();
    }
}