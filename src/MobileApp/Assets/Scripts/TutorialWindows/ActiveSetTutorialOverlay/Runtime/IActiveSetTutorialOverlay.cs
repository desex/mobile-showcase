using System.Threading;
using Cysharp.Threading.Tasks;
using R3;

namespace Sotbify.MobileApp.TutorialWindows.ActiveSetTutorialOverlay
{
    public interface IActiveSetTutorialOverlay
    {
        public Observable<Unit> ShowOpenHomeVisualObservable { get; }
        public void ShowOpenHomeVisual();
        public UniTask OpenMoreInfoAsync(CancellationToken cancellationToken = default);
    }
}