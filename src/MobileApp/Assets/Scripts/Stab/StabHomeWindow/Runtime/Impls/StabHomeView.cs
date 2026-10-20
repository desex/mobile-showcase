using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Animation;
using R3;
using Sotbify.MobileApp.UI.Window.View;
using Sotbify.MobileApp.Utils;
using Sotbify.MobileApp.Utils.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace Stab.StabHomeWindow
{
    public class StabHomeView : FadeWindowView
    {
        [SerializeField] private ItemToButton _itemToButton;

        [SerializeField] private TMP_Text _statusText;
        [SerializeField] private Graphic _failedStatusImage;
        [SerializeField] private LitMotionAnimation _loadingAnimation;
        [SerializeField] private Button _connectButton;
        [SerializeField] private Button _supportButton;
        [SerializeField] private CanvasGroup _supportButtonCanvasGroup;

        [SerializeField] private Color _loadingColor;
        [SerializeField] private float _targetButtonWidth;
        [SerializeField] private float _delay;
        [SerializeField] private MotionParameters _buttonMotionParameters;

        public Observable<Unit> ConnectButtonPressed => _connectButton.OnClickAsObservable();
        public Observable<Unit> SupportButtonPressed => _supportButton.OnClickAsObservable();
        
        public IReadOnlyDictionary<EHomeItem, Button> ItemToButton => _itemToButton;

        public async UniTask PlayConnectAnimation(CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private async UniTask AnimateButtons(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void ChangeConnectingStatus(bool isConnecting, Color targetColor, string targetText)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}