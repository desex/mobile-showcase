using R3;
using Sotbify.MobileApp.UI.Window.View;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.TutorialWindows.RelicsMoreTutorialInfo
{
    public class RelicsMoreTutorialInfoView : FadeWindowView
    {
        [SerializeField] private Button _closeButton;
        
        public Observable<Unit> CloseObservable => _closeButton.OnClickAsObservable();
    }
}