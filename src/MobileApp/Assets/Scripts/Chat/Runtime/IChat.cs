using R3;
using Sotbify.MobileApp.Utils.Data;

namespace Sotbify.MobileApp.Chat
{
    public interface IChat
    {
        public ReadOnlyReactiveProperty<ManagerData> ManagerData { get; }
        public ReadOnlyReactiveProperty<bool> ShouldShowLoading { get; }

        public Observable<bool> ChangeQuickMessagePanelVisibilityObservable { get; }

        public void Start();
        public void Close();
    }
}