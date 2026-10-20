using R3;
using Sotbify.MobileApp.UI.Window.View;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.TutorialWindows.EventsTutorialOverlay
{
    public class EventsTutorialOverlayView : FadeWindowView
    {
        [SerializeField] private Button _closeButton;

        public Observable<Unit> CloseObservable => _closeButton.OnClickAsObservable();
    }
}