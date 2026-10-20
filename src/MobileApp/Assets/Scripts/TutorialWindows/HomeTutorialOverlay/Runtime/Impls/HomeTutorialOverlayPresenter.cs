using System;
using R3;
using Sotbify.MobileApp.HomeWindow;
using Sotbify.MobileApp.UI.Window.Presenter;

namespace Sotbify.MobileApp.TutorialWindows.HomeTutorialOverlay
{
    public class HomeTutorialOverlayPresenter : WindowPresenter<IHomeTutorialOverlay, HomeTutorialOverlayView>
    {
        private readonly IHome _home;
        
        private IDisposable _disposable;

        public HomeTutorialOverlayPresenter(
            IHome home,
            IDisposable disposable
        )
        {
            _home = home;
            _disposable = disposable;
        }

        public override void Initialize()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override void OnDispose()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}