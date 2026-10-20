using System.Threading;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using Sotbify.MobileApp.Server.HttpClient.Core;
using Sotbify.MobileApp.Utils.Data.Blackjack;
using Sotbify.MobileApp.Utils.Enums.Blackjack;

namespace Sotbify.MobileApp.BlackjackWindow
{
    public interface IBlackjack
    {
        public ObservableList<Card> PlayerHand { get; }
        public ObservableList<Card> DealerHand { get; }
        public Observable<EGameResult> RoundEnded { get; }
        public Observable<Unit> GameCancelledExplicitly { get; }
        public Observable<EGameResult> GameEnded { get; }
        public Observable<EGameResult> CloseAfterResult { get; }
        public Observable<Unit> ShowClosePopupObservable { get; }
        public ReadOnlyReactiveProperty<int> CurrentRound { get; }
        public ReadOnlyReactiveProperty<int> PlayerCardCount { get; }
        public ReadOnlyReactiveProperty<int> DealerCardCount { get; }
        public Observable<Card> OpenDealerCardObservable { get; }

        public UniTask StartGame(CancellationToken cancellationToken = default);
        public UniTask Hit(CancellationToken cancellationToken = default);
        public UniTask Stand(CancellationToken cancellationToken = default);
        public void OnCloseAfterResult();
        public void PrepareForNextGame();
        public UniTask<HttpResponse> CancelGame(CancellationToken cancellationToken = default);
        public void ShowClosePopup();
    }
}