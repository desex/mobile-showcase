using R3;
using Sotbify.MobileApp.UI.Window.View;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.CloseBlackjackPopup
{
    public class CloseBlackjackPopupView : FadeWindowView
    {
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _cancelButton;

        public Observable<Unit> OnContinuePressed => _continueButton.OnClickAsObservable();
        public Observable<Unit> OnCancelPressed => _cancelButton.OnClickAsObservable();
    }
}