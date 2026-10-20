using R3;
using Sotbify.MobileApp.UI.Window.View;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.HiddenModeWindow
{
    public class HiddenModeWindowView : FadeWindowView
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _pinButton;
        [SerializeField] private Button _cancelButton;

        public Observable<Unit> BackObservable => _backButton.OnClickAsObservable();
        public Observable<Unit> PinObservable => _pinButton.OnClickAsObservable();
        public Observable<Unit> CancelObservable => _cancelButton.OnClickAsObservable();
    }
}
