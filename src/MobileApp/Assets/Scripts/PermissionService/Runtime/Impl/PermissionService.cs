using System.Threading;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using Firebase.Messaging;
using R3;
using Sotbify.MobileApp.Utils.Enums;
using UnityEngine;
using UnityEngine.Android;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.PermissionService
{
    public class PermissionService : IPermissionService
    {
        private const string NotificationPermission = "android.permission.POST_NOTIFICATIONS";

        private readonly ILogger _logger;
        private UniTaskCompletionSource<bool> _notificationPermissionTcs;
        private bool? _notificationPermissionResult;

#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void EnableForegroundNotificationPresentation();

        [DllImport("__Internal")]
        private static extern int GetNotificationAuthorizationStatus();
#endif

        public PermissionService(ILogger logger)
        {
            _logger = logger;
        }

        public async UniTask<EPermissionStates> RequestPermissionAndroidAsync(string permission, CancellationToken cancellationToken = default)
        {
            var winTask = int.MaxValue;
            var permissionCallbacks = new PermissionCallbacks();
            var deniedTask = Observable.FromEvent<string>(
                    h => permissionCallbacks.PermissionDenied += h,
                    h => permissionCallbacks.PermissionGranted -= h
                )
                .FirstAsync(cancellationToken)
                .AsUniTask();
            
            var grantedTask = Observable.FromEvent<string>(
                    h => permissionCallbacks.PermissionGranted += h,
                    h => permissionCallbacks.PermissionGranted -= h
                )
                .FirstAsync(cancellationToken)
                .AsUniTask();
            
            var permissionDismissed = Observable.FromEvent<string>(
                    h => permissionCallbacks.PermissionRequestDismissed += h,
                    h => permissionCallbacks.PermissionRequestDismissed -= h
                )
                .FirstAsync(cancellationToken)
                .AsUniTask();
            do
            {
                Permission.RequestUserPermission(permission, permissionCallbacks);
                
                (winTask, _, _, _) = await UniTask.WhenAny(deniedTask, grantedTask, permissionDismissed);

                switch (winTask)
                {
                    case 0:
                        return EPermissionStates.DontAskAgain;
                    case 1:
                        return EPermissionStates.Granted;
                }
                
            } while (cancellationToken.IsCancellationRequested == false && winTask == 3);
            
            return EPermissionStates.None;
        }

        public async UniTask<EPermissionStates> RequestPermissionIOSAsync(string permission, CancellationToken cancellationToken = default)
        {
            var userAuthorization = permission == Permission.Camera
                ? UserAuthorization.WebCam
                : UserAuthorization.Microphone;
            
            var request = Application
                .RequestUserAuthorization(userAuthorization)
                .ToUniTask(cancellationToken: cancellationToken);

            await request;

            return Application.HasUserAuthorization(userAuthorization) 
                ? EPermissionStates.Granted 
                : EPermissionStates.DontAskAgain;  
        }

        public async UniTask RequestNotificationPermission()
        {
#if UNITY_ANDROID
            if (Permission.HasUserAuthorizedPermission(NotificationPermission))
            {
                _notificationPermissionResult = true;
                _notificationPermissionTcs?.TrySetResult(true);
                return;
            }

            _notificationPermissionResult = null;
            _notificationPermissionTcs = new UniTaskCompletionSource<bool>();

            var permissionCallbacks = new PermissionCallbacks();
            permissionCallbacks.PermissionGranted += _ => ResolveNotificationPermission(true);
            permissionCallbacks.PermissionDenied += _ => ResolveNotificationPermission(false);
            permissionCallbacks.PermissionRequestDismissed += _ => { };

            Permission.RequestUserPermission(NotificationPermission, permissionCallbacks);
#elif UNITY_IOS
            await FirebaseMessaging.RequestPermissionAsync().AsUniTask();
#if !UNITY_EDITOR
            EnableForegroundNotificationPresentation();
#endif
            var notificationPermissionState = await GetNotificationPermissionStateAsync();
            _notificationPermissionResult = IsNotificationPermissionGranted(notificationPermissionState);
#endif
        }

        public bool HasPermission(string permission)    
        {
#if UNITY_EDITOR
            return true;
#elif UNITY_ANDROID
            return Permission.HasUserAuthorizedPermission(Permission.Camera);
#elif UNITY_IOS
            var userAuthorization = permission == Permission.Camera
                ? UserAuthorization.WebCam
                : UserAuthorization.Microphone;
            return Application.HasUserAuthorization(userAuthorization);
#endif
            return true;
        }

        public UniTask<bool> WaitNotificationsPermissionAction(CancellationToken cancellationToken = default)
        {
            #if UNITY_EDITOR
                return UniTask.FromResult(true);
            #elif UNITY_ANDROID
                if (Permission.HasUserAuthorizedPermission(NotificationPermission))
                    return UniTask.FromResult(true);

                if (_notificationPermissionResult.HasValue)
                    return UniTask.FromResult(_notificationPermissionResult.Value);

                return WaitAndroidNotificationsPermission(cancellationToken);
            #elif UNITY_IOS
                if (_notificationPermissionResult.HasValue)
                    return UniTask.FromResult(_notificationPermissionResult.Value);

                return WaitIosNotificationsPermission(cancellationToken);
            #else
                return UniTask.FromResult(true);
            #endif

            return UniTask.FromResult(false);
        }

        public bool HasNotificationPermission()
        {
#if UNITY_EDITOR
            return false;
#elif UNITY_ANDROID
            return Permission.HasUserAuthorizedPermission(NotificationPermission);
#elif UNITY_IOS
            return _notificationPermissionResult == true;
#else
            return true;
#endif
        }

        public async UniTask<ENotificationPermissionState> GetNotificationPermissionStateAsync(CancellationToken cancellationToken = default)
        {
#if UNITY_EDITOR
            return ENotificationPermissionState.Unknown;
#elif UNITY_ANDROID
            await UniTask.CompletedTask;
            return Permission.HasUserAuthorizedPermission(NotificationPermission)
                ? ENotificationPermissionState.Authorized
                : ENotificationPermissionState.Denied;
#elif UNITY_IOS
            await UniTask.SwitchToMainThread(cancellationToken);
            return ToNotificationPermissionState(GetNotificationAuthorizationStatus());
#else
            await UniTask.CompletedTask;
            return ENotificationPermissionState.Authorized;
#endif
        }

        private void ResolveNotificationPermission(bool isGranted)
        {
            _notificationPermissionResult = isGranted;
            _notificationPermissionTcs?.TrySetResult(isGranted);
        }

        private static ENotificationPermissionState ToNotificationPermissionState(int authorizationStatus)
        {
            return authorizationStatus switch
            {
                0 => ENotificationPermissionState.NotDetermined,
                1 => ENotificationPermissionState.Denied,
                2 => ENotificationPermissionState.Authorized,
                3 => ENotificationPermissionState.Provisional,
                4 => ENotificationPermissionState.Ephemeral,
                _ => ENotificationPermissionState.Unknown,
            };
        }

        private static bool IsNotificationPermissionGranted(ENotificationPermissionState notificationPermissionState)
        {
            return notificationPermissionState is ENotificationPermissionState.Authorized
                or ENotificationPermissionState.Provisional
                or ENotificationPermissionState.Ephemeral;
        }

        private async UniTask<bool> WaitAndroidNotificationsPermission(CancellationToken cancellationToken)
        {
            if (_notificationPermissionTcs != null)
                return await _notificationPermissionTcs.Task.AttachExternalCancellation(cancellationToken);

            while (cancellationToken.IsCancellationRequested == false)
            {
                if (Permission.HasUserAuthorizedPermission(NotificationPermission))
                    return true;

                if (_notificationPermissionResult.HasValue)
                    return _notificationPermissionResult.Value;

                await UniTask.Yield(cancellationToken);
            }

            cancellationToken.ThrowIfCancellationRequested();
            return false;
        }

        private async UniTask<bool> WaitIosNotificationsPermission(CancellationToken cancellationToken)
        {
            while (cancellationToken.IsCancellationRequested == false)
            {
                if (_notificationPermissionResult.HasValue)
                    return _notificationPermissionResult.Value;

                await UniTask.Yield(cancellationToken);
            }

            cancellationToken.ThrowIfCancellationRequested();
            return false;
        }
    }
}
