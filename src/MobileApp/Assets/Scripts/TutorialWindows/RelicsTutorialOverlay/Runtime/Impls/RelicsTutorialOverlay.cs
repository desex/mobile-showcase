using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Db.TutorialConfig;
using Sotbify.MobileApp.NexusWindow;
using Sotbify.MobileApp.TutorialWindows.RelicsMoreTutorialInfo;
using Sotbify.MobileApp.UI.Common.Providers;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils.Enums.Nexus;
using Sotbify.MobileApp.Utils.Extensions;

namespace Sotbify.MobileApp.TutorialWindows.RelicsTutorialOverlay
{
    public class RelicsTutorialOverlay : IRelicsTutorialOverlay
    {
        private readonly Subject<Unit> _closeMainVisual = new();
        private readonly ITutorialConfig _tutorialConfig;
        private readonly IRootsRectProvider _rootProvider;
        private readonly IWindowService _windowService;
        private readonly INexus _nexus;

        public RelicsTutorialOverlay(
            ITutorialConfig tutorialConfig,
            IRootsRectProvider rootProvider,
            IWindowService windowService,
            INexus nexus
        )
        {
            _tutorialConfig = tutorialConfig;
            _rootProvider = rootProvider;
            _windowService = windowService;
            _nexus = nexus;
        }
        
        public Observable<Unit> CloseMainVisual => _closeMainVisual;
        
        public void Close()
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