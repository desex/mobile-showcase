using System.Threading;
using Cysharp.Threading.Tasks;
using R3;

namespace Sotbify.MobileApp.TutorialWindows.RelicsTutorialOverlay
{
    public interface IRelicsTutorialOverlay
    {
        public Observable<Unit> CloseMainVisual { get; }
        public void Close();
        public UniTask OpenMoreInfoAsync(CancellationToken cancellationToken = default);
    }
}