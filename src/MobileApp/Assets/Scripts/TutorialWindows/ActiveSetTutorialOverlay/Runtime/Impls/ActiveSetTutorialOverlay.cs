using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.NexusWindow;
using Sotbify.MobileApp.TutorialWindows.RelicsMoreTutorialInfo;
using Sotbify.MobileApp.UI.Common.Providers;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils.Enums.Nexus;

namespace Sotbify.MobileApp.TutorialWindows.ActiveSetTutorialOverlay
{
    public class ActiveSetTutorialOverlay : IActiveSetTutorialOverlay
    {
        private readonly Subject<Unit> _showOpenHomeVisual = new();
        private readonly IRootsRectProvider _rootProvider;
        private readonly IWindowService _windowService;
        private readonly INexus _nexus;

        public ActiveSetTutorialOverlay(
            IRootsRectProvider rootProvider,
            IWindowService windowService,
            INexus nexus
        )
        {
            _rootProvider = rootProvider;
            _windowService = windowService;
            _nexus = nexus;
        }
        
        public Observable<Unit> ShowOpenHomeVisualObservable => _showOpenHomeVisual;
        
        public void ShowOpenHomeVisual()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public UniTask OpenMoreInfoAsync(CancellationToken cancellationToken = default) {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}