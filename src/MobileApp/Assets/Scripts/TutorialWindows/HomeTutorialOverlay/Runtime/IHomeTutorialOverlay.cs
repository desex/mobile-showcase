using System.Threading;
using Cysharp.Threading.Tasks;

namespace Sotbify.MobileApp.TutorialWindows.HomeTutorialOverlay
{
    public interface IHomeTutorialOverlay
    {
        public UniTask Close(CancellationToken cancellationToken = default);
    }
}