using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.UI.Window;

namespace Sotbify.MobileApp.TutorialWindows.EventsTutorialOverlay
{
    public class EventsTutorialOverlay : IEventsTutorialOverlay
    {
        private readonly IWindowService _windowService;

        public EventsTutorialOverlay(IWindowService windowService)
        {
            _windowService = windowService;
        }

        public UniTask Close(CancellationToken cancellationToken = default) 
            {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}