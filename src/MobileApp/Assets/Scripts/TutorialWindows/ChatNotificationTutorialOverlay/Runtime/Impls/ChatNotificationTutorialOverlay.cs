using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.Analytics;
using Sotbify.MobileApp.PermissionService;
using Sotbify.MobileApp.Services.FirebaseCM.Runtime;
using Sotbify.MobileApp.UI.Window;

namespace Sotbify.MobileApp.TutorialWindows.ChatNotificationTutorialOverlay
{
    public class ChatNotificationTutorialOverlay : IChatNotificationTutorialOverlay
    {
        private readonly IWindowService _windowService;
        private readonly IFirebaseCMService _firebaseCMService;
        private readonly IPermissionService _permissionService;
        private readonly AnalyticsEvents _analyticsEvents;

        public ChatNotificationTutorialOverlay(
            IWindowService windowService,
            IFirebaseCMService firebaseCMService,
            IPermissionService permissionService,
            AnalyticsEvents analyticsEvents
        )
        {
            _windowService = windowService;
            _firebaseCMService = firebaseCMService;
            _permissionService = permissionService;
            _analyticsEvents = analyticsEvents;
        }
        
        public async UniTask RequestNotifications(CancellationToken cancellationToken = default)
        {
#if UNITY_EDITOR
            await Close(cancellationToken);
            return;
#endif
            await RequestNotificationsInternal(cancellationToken);
        }

        public UniTask Close(CancellationToken cancellationToken = default) 
            => _windowService.SafeCloseAsync<IChatNotificationTutorialOverlay>(cancellationToken);
        
        private async UniTask RequestNotificationsInternal(CancellationToken cancellationToken)
        {
            await _firebaseCMService.Initialize(cancellationToken);

            await _permissionService.RequestNotificationPermission();

            var isGranted = await _permissionService.WaitNotificationsPermissionAction(cancellationToken);

            if (isGranted)
                await _firebaseCMService.Subscribe();
            
            _analyticsEvents.NotificationPermissionRequest(isGranted ? "allow" : "deny");

            await Close(cancellationToken);
        }
    }
}
