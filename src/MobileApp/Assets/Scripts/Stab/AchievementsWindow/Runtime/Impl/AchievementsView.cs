using R3;
using Sotbify.MobileApp.UI.Window.View;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.Stab.AchievementsWindow
{
    public class AchievementsView : FadeWindowView
    {
        [SerializeField] private Button _closeButton;

        public Observable<Unit> CloseButtonObservable => _closeButton.OnClickAsObservable();
    }
}