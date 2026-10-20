using System.Threading;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using Sotbify.MobileApp.ChatMessages.Models;
using Sotbify.MobileApp.Utils.Enums.Blackjack;

namespace Sotbify.MobileApp.ChatMessages
{
    public interface IChatMessages
    {
        IReadOnlyObservableList<IChatItemModel> Messages { get; }
        Observable<int> UserSentMessage { get; }
        Observable<Unit> CancelActiveBlackjack { get; }
        Observable<BlackjackModel> PlayBlackjackRequested { get; set; }
        Observable<(BlackjackModel, EGameResult)> BlackjackResult { get; }
        ReadOnlyReactiveProperty<int> AdditionalContentPaddingBottom { get; }
        
        void ChangeBlackjackResult(BlackjackModel model, EGameResult gameResult);
        UniTask<bool> TryLoadHistoryAsync(CancellationToken cancellationToken = default);
        UniTask SendMessageAsync(string viewMessage, CancellationToken cancellationToken = default);
        void LastVisibleItemUpdated(int index);
        void SetAdditionalContentPaddingBottom(int paddingBottom);
    }
}