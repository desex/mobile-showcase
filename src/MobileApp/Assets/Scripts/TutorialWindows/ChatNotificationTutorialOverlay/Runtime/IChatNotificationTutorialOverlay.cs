using System.Threading;
using Cysharp.Threading.Tasks;

namespace Sotbify.MobileApp.TutorialWindows.ChatNotificationTutorialOverlay
{
    public interface IChatNotificationTutorialOverlay
    {
        public UniTask Close(CancellationToken cancellationToken = default);
        public UniTask RequestNotifications(CancellationToken cancellationToken = default);
    }
}