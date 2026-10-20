using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using Sotbify.MobileApp.Server;
using Sotbify.MobileApp.Server.HttpClient.Core;
using Sotbify.MobileApp.Services.ServerConstConverter;
using Sotbify.MobileApp.Utils;
using Sotbify.MobileApp.Utils.Data.Blackjack;
using Sotbify.MobileApp.Utils.Enums;
using Sotbify.MobileApp.Utils.Enums.Blackjack;
using Sotbify.MobileApp.Utils.Extensions;
using Sotbify.MobileApp.Utils.Schemas.Blackjack;

namespace Sotbify.MobileApp.BlackjackWindow
{
    public class Blackjack : IBlackjack, IDisposable
    {
        private readonly IBlackjackRoundStateController _roundStateController;
        private readonly Client _client;
        private readonly IServerConstConverter _serverConstConverter;
        private readonly ObservableList<Card> _playerHand = new();
        private readonly ObservableList<Card> _dealerHand = new();
        private readonly ReactiveProperty<int> _playerCardCount = new();
        private readonly ReactiveProperty<int> _dealerCardCount = new();
        private readonly ReactiveProperty<int> _currentRound = new(-1);
        private readonly Subject<Card> _openCard = new();
        private readonly Subject<EGameResult> _roundEnded = new();
        private readonly Subject<EGameResult> _gameEnded = new();
        private readonly Subject<EGameResult> _closeAfterResult = new();
        private readonly Subject<Unit> _gameCancelledExplicitly = new();
        private readonly Subject<Unit> _showClosePopup = new();

        private EGameResult _gameResult;
        private bool _isActionInProgress;

        public Blackjack(
            IBlackjackRoundStateController roundStateController,
            Client client,
            IServerConstConverter serverConstConverter
        )
        {
            _roundStateController = roundStateController;
            _client = client;
            _serverConstConverter = serverConstConverter;
        }
        
        public ObservableList<Card> PlayerHand => _playerHand;
        public ObservableList<Card> DealerHand => _dealerHand;
        public Observable<Card> OpenDealerCardObservable => _openCard;
        public Observable<Unit> GameCancelledExplicitly => _gameCancelledExplicitly;
        public Observable<EGameResult> RoundEnded => _roundEnded;
        public Observable<EGameResult> GameEnded => _gameEnded;
        public Observable<EGameResult> CloseAfterResult => _closeAfterResult;
        public Observable<Unit> ShowClosePopupObservable => _showClosePopup;

        public ReadOnlyReactiveProperty<int> CurrentRound => _currentRound;
        public ReadOnlyReactiveProperty<int> PlayerCardCount => _playerCardCount;
        public ReadOnlyReactiveProperty<int> DealerCardCount => _dealerCardCount;
        
        public async UniTask StartGame(CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public UniTask Hit(CancellationToken cancellationToken = default)
            {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public UniTask Stand(CancellationToken cancellationToken = default)
            {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private async UniTask TryExecutePlayerAction(
            Func<CancellationToken, UniTask> action,
            CancellationToken cancellationToken = default
        )
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private async UniTask HitInternal(CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private async UniTask StandInternal(CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        public void ShowClosePopup()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public async UniTask<HttpResponse> CancelGame(CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private async UniTask AnimateFirstCards(BlackjackData roundData, CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private UniTask WaitForAnimationsEnded(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void RevealCard(Card hiddenCard, Card revealedCard)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private async UniTask DealCard(EDrawer drawer, Card targetCard, BlackjackData blackjackData, CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private async UniTask EndRound(BlackjackData roundData, CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void OnCloseAfterResult()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void PrepareForNextGame()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }


        private void PrepareForNextRound()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        private static List<Card> ConvertCardList(string[] cards)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private static Card CreateHiddenPlaceholderCard()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private BlackjackData ConvertToData(BlackjackResponseSchema schema)
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