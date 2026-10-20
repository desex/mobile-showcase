using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using R3;
using Sotbify.MobileApp.UI.Window.View;
using Sotbify.MobileApp.Utils;
using Sotbify.MobileApp.Utils.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.TutorialWindows.BonusTutorialOverlay
{
    public class BonusTutorialOverlayView : FadeWindowView
    {
        [SerializeField] private CanvasGroup _mainVisual;
        [SerializeField] private CanvasGroup _requestBonusVisual;
        [SerializeField] private Button _okayButton;
        [SerializeField] private Button _moreButton;

        [SerializeField] private MotionParameters _motionParameters;

        public Observable<Unit> OnOkayClickObservable => _okayButton.OnClickAsObservable();
        public Observable<Unit> OnMoreClickObservable => _moreButton.OnClickAsObservable();

        public async UniTask ShowRequestBonusVisualAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}