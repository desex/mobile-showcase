using System.Threading;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using Sotbify.MobileApp.ChatMessages.Models;
using Sotbify.MobileApp.Utils.Schemas.Chat;

namespace Sotbify.MobileApp.ChatMessages
{
    public interface IChatItemsRepository
    {
        ReadOnlyReactiveProperty<int> UnreadCount { get; }
        IReadOnlyObservableList<IChatItemModel> LoadedItems { get; }
        Observable<int> ScrollToObservable { get; }
        Observable<Unit> CancelActiveBlackjack { get; }
        ReadOnlyReactiveProperty<bool> ShouldShowLoading { get; }
        
        UniTask StartAsync(CancellationToken cancellationToken = default);
        UniTask<bool> TryLoadMoreAsync(CancellationToken cancellationToken = default);
        UniTask LoadMoreAfterReconnectAsync(CancellationToken cancellationToken = default);
        UniTask ReadMessagesAsync(CancellationToken cancellationToken = default);
        UniTask AddToEnd(ChatMessageResponseSchema schema, CancellationToken cancellationToken = default, bool isUser = false);
        void LastSeenMessageUpdated(int itemIndex);
    }
}