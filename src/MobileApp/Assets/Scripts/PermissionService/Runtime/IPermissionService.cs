using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.Utils;
using Sotbify.MobileApp.Utils.Enums;


namespace Sotbify.MobileApp.PermissionService
{
    public interface IPermissionService
    {
        public UniTask<EPermissionStates> RequestPermissionAndroidAsync(string permission, CancellationToken cancellationToken = default);
        public UniTask<EPermissionStates> RequestPermissionIOSAsync(string permission, CancellationToken cancellationToken = default);
        public UniTask RequestNotificationPermission();
        public bool HasPermission(string permission);
        public bool HasNotificationPermission();
        public UniTask<ENotificationPermissionState> GetNotificationPermissionStateAsync(CancellationToken cancellationToken = default);
        public UniTask<bool> WaitNotificationsPermissionAction (CancellationToken cancellationToken = default);
    }
}