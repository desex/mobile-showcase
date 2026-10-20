using R3;
using Sotbify.MobileApp.UI.Window.View;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.TwoFactorWindow
{
    public class TwoFactorWindowView : FadeWindowView
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _pinButton;
        [SerializeField] private Button _disableButton;

        public Observable<Unit> BackObservable => _backButton.OnClickAsObservable();
        public Observable<Unit> PinObservable => _pinButton.OnClickAsObservable();
        public Observable<Unit> DisableObservable => _disableButton.OnClickAsObservable();
    }
}
