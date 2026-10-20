using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using Sotbify.MobileApp.BlackjackWindow.Core;
using Sotbify.MobileApp.UI.Window.Presenter;
using Sotbify.MobileApp.Utils;
using Sotbify.MobileApp.Utils.Data.Blackjack;
using Sotbify.MobileApp.Utils.Enums;
using Sotbify.MobileApp.Utils.Enums.Blackjack;
using Sotbify.MobileApp.Utils.Extensions.Blackjack;

namespace Sotbify.MobileApp.BlackjackWindow
{
    public class BlackjackWindowPresenter : WindowPresenter<IBlackjack, BlackjackWindowView>
    {
        private readonly CompositeDisposable _disposables = new ();
        private ISynchronizedView<Card, CardView> _dealerCardViews;
        private ISynchronizedView<Card, CardView> _playerCardViews;
        private readonly List<CardView> _playerViewsCache = new();
        private readonly List<CardView> _dealerViewsCache = new();
        private readonly Func<Card, CardView> _cardFactory;
        private readonly IBlackjackRoundStateController _blackjackRoundStateController;
        
        public BlackjackWindowPresenter(
            Func<Card, CardView> cardFactory,
            IBlackjackRoundStateController blackjackRoundStateController
        )
        {
            _cardFactory = cardFactory;
            _blackjackRoundStateController = blackjackRoundStateController;
        }

        public override void Initialize()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override UniTask AfterShowAsync(CancellationToken cancellationToken)
            {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private CardView FindCardView(Card card)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        private static void DestroyView(CardView view)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override void OnDispose()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}