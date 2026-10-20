using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.NexusWindow;
using Sotbify.MobileApp.UI.Window.Presenter;
using Sotbify.MobileApp.Utils.Enums.Nexus;

namespace Sotbify.MobileApp.TutorialWindows.WelcomeTutorialOverlay
{
    public class WelcomeTutorialOverlayPresenter : WindowPresenter<IWelcomeTutorialOverlay, WelcomeTutorialOverlayView>
    {
        private readonly INexus _nexus;

        public WelcomeTutorialOverlayPresenter(INexus nexus)
        {
            _nexus = nexus;
        }

        protected override UniTask AfterShowAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override UniTask BeforeHideAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}