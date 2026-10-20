using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.TutorialWindows.BonusesMoreTutorialInfo;
using Sotbify.MobileApp.UI.Common.Providers;
using Sotbify.MobileApp.UI.Window;

namespace Sotbify.MobileApp.TutorialWindows.BonusTutorialOverlay
{
    public class BonusTutorialOverlay : IBonusTutorialOverlay
    {
        private readonly Subject<Unit> _showRequestBonusVisual = new();
        private readonly IRootsRectProvider _rootProvider;
        private readonly IWindowService _windowService;

        public BonusTutorialOverlay(
            IRootsRectProvider rootProvider,
            IWindowService windowService
        )
        {
            _rootProvider = rootProvider;
            _windowService = windowService;
        }

        public Observable<Unit> ShowRequestBonusVisualObservable => _showRequestBonusVisual;

        public void ShowRequestBonusVisual() 
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