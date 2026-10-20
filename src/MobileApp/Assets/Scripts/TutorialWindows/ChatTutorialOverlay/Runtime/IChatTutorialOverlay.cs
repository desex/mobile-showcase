using System.Threading;
using Cysharp.Threading.Tasks;

namespace Sotbify.MobileApp.TutorialWindows.ChatTutorialOverlay
{
    public interface IChatTutorialOverlay
    {
        public UniTask Close(CancellationToken cancellationToken = default);
    }
}