using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using Sotbify.MobileApp.ChatMessages.Models;

namespace Stab.StabChatWindow
{
    public interface IStabChat
    {
        public IReadOnlyObservableList<UserMessageModel> Messages { get; }

        public Observable<int> MoveChatObservable { get; }
        public Observable<bool> ChangeQuickMessagePanelVisibilityObservable { get; }
        

        public void SendMessage(string viewMessage);
        public UniTask MoveChat(bool isMoveUpwards);
        public void Clear();
    }
}