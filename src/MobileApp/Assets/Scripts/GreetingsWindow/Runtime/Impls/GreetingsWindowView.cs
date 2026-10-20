using R3;
using Sotbify.MobileApp.UI.Window.View;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.GreetingsWindow
{
    public sealed class GreetingsWindowView : FadeWindowView
    {
        [SerializeField] private Button _scanButton;
        [SerializeField] private Button _openPolicyButton;
        
        public Observable<Unit> ScanButtonPressed => _scanButton.OnClickAsObservable();
        public Observable<Unit> OpenPolicyPressed => _openPolicyButton.OnClickAsObservable();
    }
}