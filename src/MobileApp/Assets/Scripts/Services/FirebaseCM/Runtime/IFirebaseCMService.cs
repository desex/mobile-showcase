using System.Threading;
using Cysharp.Threading.Tasks;

namespace Sotbify.MobileApp.Services.FirebaseCM.Runtime
{
    public interface IFirebaseCMService
    {
        public UniTask Initialize(CancellationToken cancellationToken = default);
        public UniTask Subscribe();
        public bool TryHandlePendingNotificationOpen();
    }
}