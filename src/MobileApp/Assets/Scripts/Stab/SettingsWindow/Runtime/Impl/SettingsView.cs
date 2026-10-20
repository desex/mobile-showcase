using R3;
using Sotbify.MobileApp.UI.Window.View;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.Stab.SettingsWindow
{
    public class SettingsView : FadeWindowView
    {
        [SerializeField] private Button _closeButton;

        public Observable<Unit> CloseButtonObservable => _closeButton.OnClickAsObservable();
    }
}