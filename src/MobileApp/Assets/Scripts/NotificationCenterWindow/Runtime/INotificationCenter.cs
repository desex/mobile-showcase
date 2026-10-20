using System.Threading;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;

namespace Sotbify.MobileApp.NotificationCenterWindow
{
    public interface INotificationCenter
    {
        public ReadOnlyReactiveProperty<bool> ShouldShowLoading { get; }
        public ReadOnlyReactiveProperty<int> UnreadCount { get; }
        public Observable<Unit> CloseObservable { get; }
        public Observable<Unit> ScrollToTop { get; }
        public bool IsAllLoaded { get; }
        
        public IReadOnlyObservableList<NotificationModel> Notifications { get; }
        
        public UniTask LoadMoreAsync(CancellationToken cancellationToken = default);
        public void Close();
    }
}