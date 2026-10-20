using System.Threading;
using Cysharp.Threading.Tasks;

namespace Sotbify.MobileApp.TutorialWindows.EventsTutorialOverlay
{
    public interface IEventsTutorialOverlay
    {
        public UniTask Close(CancellationToken cancellationToken = default);
    }
}