using System.Threading;
using Cysharp.Threading.Tasks;
using R3;

namespace Sotbify.MobileApp.TutorialWindows.BonusTutorialOverlay
{
    public interface IBonusTutorialOverlay
    {
        public Observable<Unit> ShowRequestBonusVisualObservable { get; }
        public void ShowRequestBonusVisual();
        public UniTask OpenMoreInfoAsync(CancellationToken cancellationToken = default);
    }
}