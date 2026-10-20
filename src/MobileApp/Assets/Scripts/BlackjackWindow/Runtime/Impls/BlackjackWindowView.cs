using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.BlackjackWindow.Core;
using Sotbify.MobileApp.Db.BlackjackWindowConfig;
using Sotbify.MobileApp.UI.Window.View;
using Sotbify.MobileApp.Utils;
using Sotbify.MobileApp.Utils.Data.Blackjack;
using Sotbify.MobileApp.Utils.Enums;
using Sotbify.MobileApp.Utils.Enums.Blackjack;
using Sotbify.MobileApp.Utils.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Pool;
using UnityEngine.UI;
using VContainer;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.BlackjackWindow
{
    public class BlackjackWindowView : FadeWindowView, IDisposable
    {
        [SerializeField] private Button _takeButton;
        [SerializeField] private Button _stopButton;
        [SerializeField] private Button _closeAfterResultButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Image _fieldImage;
        [SerializeField] private List<ProgressStrip> _progressStrips;
        [SerializeField] private TMP_Text _roundCounter;
        [SerializeField] private TMP_Text _resultText;
        [SerializeField] private TMP_Text _playerCardCount;
        [SerializeField] private TMP_Text _dealerCardCount;
        [SerializeField] private TMP_Text _resultBoxText;
        [SerializeField] private GameObject _dealerCounterGameObject;
        [SerializeField] private RectTransform _cardParent;
        [SerializeField] private RectTransform _resultBox;
        [SerializeField] private CanvasGroup _closeButtonCanvasGroup;
        [SerializeField] private List<LocalizedString> _counterTexts;

        [SerializeField] private RectTransform _startDealerCardLocalPosition;
        [SerializeField] private RectTransform _startPlayerCardLocalPosition;
        [SerializeField] private RectTransform _endDealerCardLocalPosition;
        [SerializeField] private RectTransform _endPlayerCardLocalPosition;

        [SerializeField] private MotionParameters _moveMotionParameters;
        [SerializeField] private MotionParameters _turnMotionParameters;
        [SerializeField] private MotionParameters _resultMotionParameters;
        
        private readonly Subject<Unit> _playAgain = new ();

        private Vector3 _currentDealerDisplace;
        private Vector3 _currentPlayerDisplace;
        private int _currentRound;
        private string _currentActiveText;
        private ILogger _logger;
        private IBlackjackWindowConfig _blackjackWindowConfig;
        private CancellationTokenSource _visibilityChangeTokenSource;

        public Observable<Unit> TakeObservable => _takeButton.OnClickAsObservable();
        public Observable<Unit> StopObservable => _stopButton.OnClickAsObservable();
        public Observable<Unit> PlayAgainObservable => _playAgain;
        public Observable<Unit> CloseAfterResultObservable => _closeAfterResultButton.OnClickAsObservable();
        public Observable<Unit> CloseObservable => _closeButton.OnClickAsObservable();

        [Inject]
        public void Construct(
            ILogger logger,
            IBlackjackWindowConfig blackjackWindowConfig
        )
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override UniTask ShowInternalAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public async UniTask TakeCardAsync(Card card, EDrawer drawer, CardView cardView, CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void Dispose()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public async UniTask OpenDealerCard(CardView card, CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void TryBlockButtons(bool canPlayerAct)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void ResetRoundVisualState()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public async UniTask ShowRoundResultAnimation(EGameResult roundResult, CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public async UniTask ShowGameResultAnimation(EGameResult gameResult, CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private async UniTask AnimateField(Color fieldColor, Color textColor, float textAlpha, CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void ApplyButtonsState(bool canPlayerAct)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void RoundNumberChanged(int number)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void ChangePlayerCounter(int count)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        public void ChangeDealerCounter(int count)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}