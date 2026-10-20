using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.NexusWindow;
using Sotbify.MobileApp.UI.Window.Presenter;
using Sotbify.MobileApp.Utils.Enums.Nexus;

namespace Sotbify.MobileApp.TutorialWindows.RelicsTutorialOverlay
{
    public class RelicsTutorialOverlayPresenter : WindowPresenter<IRelicsTutorialOverlay, RelicsTutorialOverlayView>
    {
        private readonly INexus _nexus;
        
        private IDisposable _disposables;

        public RelicsTutorialOverlayPresenter(INexus nexus)
        {
            _nexus = nexus;
        }

        public override void Initialize()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override UniTask BeforeHideAsync(CancellationToken cancellationToken)
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