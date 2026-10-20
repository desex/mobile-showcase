using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Messaging;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Analytics;
using Sotbify.MobileApp.Common.Preferences;
using Sotbify.MobileApp.NexusWindow;
using Sotbify.MobileApp.Server;
using Sotbify.MobileApp.Services.ServerConstConverter;
using Sotbify.MobileApp.Utils;
using Sotbify.MobileApp.Utils.Enums.Nexus;
using Sotbify.MobileApp.Utils.Extensions;
using UnityEngine;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;
using LogLevel = Sotbifay.MobileApp.Logger.LogLevel;

namespace Sotbify.MobileApp.Services.FirebaseCM.Runtime.Impls
{
    public class FirebaseCMService : IFirebaseCMService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IPreferences _preferences;
        private readonly Client _client;
        private readonly AnalyticsEvents _analyticsEvents;
        private readonly IServerConstConverter _serverConstConverter;
        private readonly INexus _nexus;
        private bool _isInitialized;
        private bool _isInitializationStarted;
        private bool _isSubscribedToEvents;
        private bool _isSubscribedToTopic;
        private bool _isTopicSubscriptionStarted;
        private bool _isNotificationHandlingReady;
        private bool _hasPendingNotificationOpen;
        private string _pendingNotificationUuid;
        private ENexusCategory _pendingNexusCategory;
        private UniTask _initializationTask;
        private const string OpenActionKey = "open_action";
        private const string NotificationUuidKey = "notification_uuid";
        private const int IosNotificationOpenBufferSize = 512;

        private bool _hasPushReceivedTime;
        private DateTime _pushReceivedTime;

#if UNITY_ANDROID && !UNITY_EDITOR
        private const string AndroidNotificationActivityClassName = "com.sotbifay.mobileapp.push.SotbifayMessagingUnityPlayerActivity";
        private AndroidNotificationOpenListener _androidNotificationOpenListener;
        private AndroidPushDismissListener _androidPushDismissListener;
        private AndroidPushReceivedListener _androidPushReceivedListener;
#endif

#if UNITY_IOS && !UNITY_EDITOR
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void IosNotificationOpenCallback(IntPtr openAction, IntPtr notificationUuid);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void IosNotificationDismissCallback(IntPtr openAction, IntPtr notificationUuid);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void IosNotificationPresentedCallback();

        private static readonly IosNotificationOpenCallback IosOpenCallback = OnIosNotificationOpenFromNative;
        private static readonly IosNotificationDismissCallback IosDismissCallback = OnIosNotificationDismissFromNative;
        private static readonly IosNotificationPresentedCallback IosPresentedCallback = OnIosNotificationPresentedFromNative;
        private static FirebaseCMService _iosNotificationActionService;

        [DllImport("__Internal")]
        private static extern void EnableForegroundNotificationPresentation();

        [DllImport("__Internal")]
        private static extern void LogCurrentNotificationCenterDelegate(string source);

        [DllImport("__Internal")]
        private static extern void RegisterNotificationDismissCallback(IosNotificationDismissCallback callback);

        [DllImport("__Internal")]
        private static extern void RegisterNotificationOpenCallback(IosNotificationOpenCallback callback);

        [DllImport("__Internal")]
        private static extern void RegisterNotificationPresentedCallback(IosNotificationPresentedCallback callback);

        [DllImport("__Internal")]
        private static extern int TryConsumePendingNotificationOpen(
            StringBuilder openActionBuffer,
            int openActionBufferSize,
            StringBuilder notificationUuidBuffer,
            int notificationUuidBufferSize
        );

        [DllImport("__Internal")]
        private static extern void ClearPendingNotificationOpen();

        [DllImport("__Internal")]
        private static extern int TryConsumePendingNotificationDismiss(
            StringBuilder openActionBuffer,
            int openActionBufferSize,
            StringBuilder notificationUuidBuffer,
            int notificationUuidBufferSize
        );
#endif

        public FirebaseCMService(
            ILogger logger,
            IPreferences preferences,
            Client client,
            AnalyticsEvents analyticsEvents,
            IServerConstConverter serverConstConverter,
            INexus nexus
        )
        {
            _logger = logger;
            _preferences = preferences;
            _client = client;
            _analyticsEvents = analyticsEvents;
            _serverConstConverter = serverConstConverter;
            _nexus = nexus;
        }


        public async UniTask Initialize(CancellationToken cancellationToken = default)
        {
            if (_isInitialized)
                return;

            if (_isInitializationStarted)
            {
                await _initializationTask.AttachExternalCancellation(cancellationToken);
                return;
            }

            _isInitializationStarted = true;
            _initializationTask = InitializeInternal(cancellationToken).Preserve();
            await _initializationTask;
        }

        private async UniTask InitializeInternal(CancellationToken cancellationToken)
        {
            if (_isInitialized)
                return;

            var dependencyStatus = await FirebaseApp
                .CheckAndFixDependenciesAsync()
                .AsUniTask()
                .AttachExternalCancellation(cancellationToken);

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.Information("Firebase Initialized", context: this);

            if (dependencyStatus != DependencyStatus.Available)
            {
                _logger.Error($"Firebase dependencies are not available: {dependencyStatus}");
                return;
            }

            _ = FirebaseApp.DefaultInstance;
            EnsureTokenRegistrationOnInitEnabled();

            SubscribeFirebaseEvents();
#if UNITY_ANDROID && !UNITY_EDITOR
            RegisterAndroidNotificationOpenListener();
            RegisterAndroidNotificationReceivedListener();
            RegisterAndroidNotificationDismissedListener();
#endif
#if UNITY_IOS && !UNITY_EDITOR
            EnableForegroundNotificationPresentation();
            LogCurrentNotificationCenterDelegate("FirebaseCMService.InitializeInternal after EnableForegroundNotificationPresentation");
            RegisterIosNotificationDismissCallback();
            ReinstallIosForegroundNotificationPresentationAsync(cancellationToken)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to reinstall iOS foreground notification presentation", this);
#endif
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            Application.focusChanged += OnApplicationFocusChanged;
#endif

            _isInitialized = true;
        }

        public async UniTask Subscribe()
        {
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.Information("Firebase CM Subscribe requested", context: this);

            await Initialize();

            if (_isSubscribedToTopic)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                    _logger.Information("Firebase CM topic already subscribed", context: this);

                return;
            }

            if (_isTopicSubscriptionStarted)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                    _logger.Information("Firebase CM topic subscription is already in progress", context: this);

                return;
            }

            _isTopicSubscriptionStarted = true;

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.Information("Firebase CM topic subscription started in background", context: this);

            SubscribeToTopicAsync()
                .ForgetIgnoreOperationCancelledException(_logger, "Firebase CM topic subscription failed", this);
        }

        private async UniTask SubscribeToTopicAsync()
        {
            const string topic = "all_users_notification";

            try
            {
                EnsureTokenRegistrationOnInitEnabled();

                if (_logger.IsEnabled(LogLevel.Information))
                    _logger.Information("Firebase CM SubscribeAsync started", context: this);

                await FirebaseMessaging.SubscribeAsync(topic);

                _isSubscribedToTopic = true;

                if (_logger.IsEnabled(LogLevel.Information))
                    _logger.Information("Firebase CM SubscribeAsync completed", context: this);
            }
            catch (Exception exception)
            {
                _isTopicSubscriptionStarted = false;
                _logger.Error($"Firebase CM SubscribeAsync failed. Topic: {topic}", exception, this);
            }
        }

        private void EnsureTokenRegistrationOnInitEnabled()
        {
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.Information("Firebase CM token registration on init check started", context: this);

            if (FirebaseMessaging.TokenRegistrationOnInitEnabled == false)
            {
                FirebaseMessaging.TokenRegistrationOnInitEnabled = true;

                _logger.Warning("Firebase CM TokenRegistrationOnInitEnabled was disabled and has been enabled", context: this);
            }

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.Information("Firebase CM token registration on init check completed", context: this);
        }

        public bool TryHandlePendingNotificationOpen()
        {
            _isNotificationHandlingReady = true;
            CachePendingAndroidActivityNotificationOpen();
            CachePendingIosNotificationOpen();
            CachePendingIosNotificationDismiss();
            return TryApplyPendingNotificationOpen();
        }

        public void Dispose()
        {
            if (_isSubscribedToEvents == false)
                return;

            FirebaseMessaging.TokenReceived -= TokenReceived;
            FirebaseMessaging.MessageReceived -= MessageReceived;
#if UNITY_ANDROID && !UNITY_EDITOR
            UnregisterAndroidNotificationOpenListener();
            UnregisterAndroidNotificationReceivedListener();
            UnregisterAndroidNotificationDismissedListener();
            Application.focusChanged -= OnApplicationFocusChanged;
#elif UNITY_IOS && !UNITY_EDITOR
            UnregisterIosNotificationDismissCallback();
            Application.focusChanged -= OnApplicationFocusChanged;
#endif
            _isSubscribedToEvents = false;
        }

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        private void OnApplicationFocusChanged(bool hasFocus)
        {
            if (hasFocus == false)
                return;

            CachePendingAndroidActivityNotificationOpen();
            CachePendingIosNotificationOpen();
            CachePendingIosNotificationDismiss();
            TryApplyPendingNotificationOpen();
        }
#endif

        private void TokenReceived(object sender, TokenReceivedEventArgs eventArgs)
        {
            var token = eventArgs.Token;
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.Information("Token received");

            _preferences.SetString(SavePath.FirebaseTokenPath, token);
            _client.UpdateToken(token)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to update token", this);
        }

        private void MessageReceived(object sender, MessageReceivedEventArgs eventArgs)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.Debug("Firebase notification received", context: this);

#if UNITY_IOS && !UNITY_EDITOR
            LogCurrentNotificationCenterDelegate("FirebaseCMService.MessageReceived before re-enable");
            EnableForegroundNotificationPresentation();
            LogCurrentNotificationCenterDelegate("FirebaseCMService.MessageReceived after re-enable");
            LogIosMessageReceived(eventArgs);
            return;
#elif UNITY_ANDROID && !UNITY_EDITOR
            // The custom activity handles taps; receiving a push must not navigate.
            return;
#else
            if (eventArgs.Message.NotificationOpened == false)
                return;

            CachePendingNotification(eventArgs);
            TryApplyPendingNotificationOpen();
#endif
        }

        private void SubscribeFirebaseEvents()
        {
            if (_isSubscribedToEvents)
                return;

            FirebaseMessaging.TokenReceived += TokenReceived;
            FirebaseMessaging.MessageReceived += MessageReceived;
            _isSubscribedToEvents = true;
        }

        private void OnPushReceived()
        {
            _pushReceivedTime = DateTime.Now;
            _hasPushReceivedTime = true;
            _analyticsEvents.InAppPushReceived();
        }

        private void OnInAppPushOpened()
        {
            if (_hasPushReceivedTime == false)
                return;

            var timeToTapInSec = DateTime.Now.Subtract(_pushReceivedTime).TotalSeconds;
            _analyticsEvents.InAppPushOpened(timeToTapInSec);
            _hasPushReceivedTime = false;
        }

        private void OnPushDismissed()
        {
            _analyticsEvents.InAppPushDismiss();
            _hasPushReceivedTime = false;
        }

        private void CachePendingNotification(MessageReceivedEventArgs eventArgs)
        {
            eventArgs.Message.Data.TryGetValue(OpenActionKey, out var openAction);
            eventArgs.Message.Data.TryGetValue(NotificationUuidKey, out var uuid);

            CachePendingNotificationOpen(openAction, uuid);
        }

        private void CachePendingAndroidActivityNotificationOpen()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (_hasPendingNotificationOpen)
                return;

            try
            {
                using var activityClass = new AndroidJavaClass(AndroidNotificationActivityClassName);
                var openAction = activityClass.CallStatic<string>("consumePendingOpenAction");
                var uuid = activityClass.CallStatic<string>("consumePendingNotificationUuid");

                if (_logger.IsEnabled(LogLevel.Debug))
                    _logger.Debug($"Android notification activity pending. open_action: {openAction}, notification_uuid: {uuid}", context: this);

                CachePendingNotificationOpen(openAction, uuid);
            }
            catch (Exception exception)
            {
                _logger.Warning($"Failed to read Android notification activity pending data: {exception.Message}", context: this);
            }
#endif
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        private void RegisterAndroidNotificationOpenListener()
        {
            try
            {
                _androidNotificationOpenListener ??= new AndroidNotificationOpenListener(this);

                using var activityClass = new AndroidJavaClass(AndroidNotificationActivityClassName);
                activityClass.CallStatic("registerListener", _androidNotificationOpenListener);
            }
            catch (Exception exception)
            {
                _logger.Warning($"Failed to register Android notification open listener: {exception.Message}", context: this);
            }
        }

        private void RegisterAndroidNotificationReceivedListener()
        {
            try
            {
                _androidPushReceivedListener ??= new AndroidPushReceivedListener(this);


                using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                using var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                using var receiver = new AndroidJavaClass(
                    "com.sotbifay.resources.push.PushReceiver");

                receiver.CallStatic(
                    "register",
                    activity,
                    _androidPushReceivedListener);
            }
            catch (Exception exception)
            {
                _logger.Warning($"Failed to register Android notification received listener: {exception.Message}", context: this);
            }
        }

        private void RegisterAndroidNotificationDismissedListener()
        {
            try
            {
                _androidPushDismissListener ??= new AndroidPushDismissListener(this);


                using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                using var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                using var receiver = new AndroidJavaClass(
                    "com.sotbifay.resources.push.PushDismissReceiver");

                receiver.CallStatic(
                    "register",
                    activity,
                    _androidPushDismissListener);
            }
            catch (Exception exception)
            {
                _logger.Warning($"Failed to register Android notification received listener: {exception.Message}", context: this);
            }
        }

        private void UnregisterAndroidNotificationDismissedListener()
        {
            try
            {
                using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                using var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                using var receiver = new AndroidJavaClass(
                    "com.sotbifay.resources.push.PushDismissReceiver");

                receiver.CallStatic(
                    "unregister",
                    activity);
            }
            catch (Exception exception)
            {
                _logger.Warning($"Failed to unregister Android notification received listener: {exception.Message}", context: this);
            }

            _androidPushDismissListener = null;
        }

        private void UnregisterAndroidNotificationReceivedListener()
        {
            try
            {
                using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                using var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                using var receiver = new AndroidJavaClass(
                    "com.sotbifay.resources.push.PushReceiver");

                receiver.CallStatic(
                    "unregister",
                    activity);
            }
            catch (Exception exception)
            {
                _logger.Warning($"Failed to unregister Android notification received listener: {exception.Message}", context: this);
            }

            _androidPushReceivedListener = null;
        }


        private void UnregisterAndroidNotificationOpenListener()
        {
            try
            {
                using var activityClass = new AndroidJavaClass(AndroidNotificationActivityClassName);
                activityClass.CallStatic("unregisterListener");
            }
            catch (Exception exception)
            {
                _logger.Warning($"Failed to unregister Android notification open listener: {exception.Message}", context: this);
            }

            _androidNotificationOpenListener = null;
        }

        private void HandleAndroidNotificationOpenFromNative(string openAction, string notificationUuid)
        {
            HandleAndroidNotificationOpenFromNativeAsync(openAction, notificationUuid)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to handle Android notification open callback", this);
        }

        private async UniTask HandleAndroidNotificationOpenFromNativeAsync(string openAction, string notificationUuid)
        {
            await UniTask.SwitchToMainThread();

            OnInAppPushOpened();

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.Information($"Android notification open callback. open_action: {openAction}, notification_uuid: {notificationUuid}", context: this);

            if (CachePendingNotificationOpen(openAction, notificationUuid) == false)
                return;

            if (TryApplyPendingNotificationOpen())
                ClearNativePendingNotificationOpen();
        }

        private sealed class AndroidNotificationOpenListener : AndroidJavaProxy
        {
            private readonly FirebaseCMService _service;

            public AndroidNotificationOpenListener(FirebaseCMService service)
                : base("com.sotbifay.mobileapp.push.SotbifayMessagingUnityPlayerActivity$NotificationOpenListener")
            {
                _service = service;
            }

            public void onNotificationOpen(string openAction, string notificationUuid)
            {
                _service.HandleAndroidNotificationOpenFromNative(openAction, notificationUuid);
            }
        }
        private sealed class AndroidPushReceivedListener : AndroidJavaProxy
        {
            private readonly FirebaseCMService _service;

            public AndroidPushReceivedListener(FirebaseCMService service)
                : base("com.sotbifay.resources.push.PushReceiver$Listener")
            {
                _service = service;
            }

            public void onPushReceived()
            {
               UniTask.Create(async () =>
                {
                    await UniTask.SwitchToMainThread();
                    _service.OnPushReceived();
                });
            }
        }

        private sealed class AndroidPushDismissListener : AndroidJavaProxy
        {
            private readonly FirebaseCMService _service;

            public AndroidPushDismissListener(FirebaseCMService service)
                : base("com.sotbifay.resources.push.PushDismissReceiver$Listener")
            {
                _service = service;
            }

            public void onPushDismissed()
            {
                UniTask.Create(async () =>
                {
                    await UniTask.SwitchToMainThread();
                    _service.OnPushDismissed();
                });
            }
        }
#endif

#if UNITY_IOS && !UNITY_EDITOR
        private async UniTask ReinstallIosForegroundNotificationPresentationAsync(CancellationToken cancellationToken)
        {
            await UniTask.Yield(cancellationToken);
            EnableForegroundNotificationPresentation();
            LogCurrentNotificationCenterDelegate("FirebaseCMService delayed reinstall after 1 yield");

            await UniTask.DelayFrame(1, cancellationToken: cancellationToken);
            EnableForegroundNotificationPresentation();
            LogCurrentNotificationCenterDelegate("FirebaseCMService delayed reinstall after 1 frame");

            await UniTask.DelayFrame(5, cancellationToken: cancellationToken);
            EnableForegroundNotificationPresentation();
            LogCurrentNotificationCenterDelegate("FirebaseCMService delayed reinstall after 5 frames");
        }

        private void RegisterIosNotificationDismissCallback()
        {
            _iosNotificationActionService = this;
            RegisterNotificationOpenCallback(IosOpenCallback);
            RegisterNotificationDismissCallback(IosDismissCallback);
            RegisterNotificationPresentedCallback(IosPresentedCallback);
        }

        private void UnregisterIosNotificationDismissCallback()
        {
            RegisterNotificationOpenCallback(null);
            RegisterNotificationDismissCallback(null);
            RegisterNotificationPresentedCallback(null);

            if (_iosNotificationActionService == this)
                _iosNotificationActionService = null;
        }

        [AOT.MonoPInvokeCallback(typeof(IosNotificationOpenCallback))]
        private static void OnIosNotificationOpenFromNative(IntPtr openAction, IntPtr notificationUuid)
        {
            var service = _iosNotificationActionService;
            if (service == null)
                return;

            service.HandleIosNotificationOpenFromNative(
                PtrToStringUtf8(openAction),
                PtrToStringUtf8(notificationUuid)
            );
        }

        [AOT.MonoPInvokeCallback(typeof(IosNotificationDismissCallback))]
        private static void OnIosNotificationDismissFromNative(IntPtr openAction, IntPtr notificationUuid)
        {
            var service = _iosNotificationActionService;
            if (service == null)
                return;

            service.HandleIosNotificationDismissFromNative(
                PtrToStringUtf8(openAction),
                PtrToStringUtf8(notificationUuid)
            );
        }

        [AOT.MonoPInvokeCallback(typeof(IosNotificationPresentedCallback))]
        private static void OnIosNotificationPresentedFromNative()
        {
            var service = _iosNotificationActionService;
            if (service == null)
                return;

            service.HandleIosNotificationPresentedFromNative();
        }

        private static string PtrToStringUtf8(IntPtr value)
        {
            if (value == IntPtr.Zero)
                return null;

            var length = 0;
            while (Marshal.ReadByte(value, length) != 0)
                length++;

            if (length == 0)
                return string.Empty;

            var buffer = new byte[length];
            Marshal.Copy(value, buffer, 0, length);
            return Encoding.UTF8.GetString(buffer);
        }

        private void HandleIosNotificationOpenFromNative(string openAction, string notificationUuid)
        {
            HandleIosNotificationOpenFromNativeAsync(openAction, notificationUuid)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to handle iOS notification open callback", this);
        }

        private void HandleIosNotificationPresentedFromNative()
        {
            HandleIosNotificationPresentedFromNativeAsync()
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to handle iOS notification presented callback", this);
        }

        private async UniTask HandleIosNotificationPresentedFromNativeAsync()
        {
            await UniTask.SwitchToMainThread();

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.Information("iOS notification presented callback", context: this);

            OnPushReceived();
        }

        private async UniTask HandleIosNotificationOpenFromNativeAsync(string openAction, string notificationUuid)
        {
            await UniTask.SwitchToMainThread();

            OnInAppPushOpened();

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.Information($"iOS notification open callback. open_action: {openAction}, notification_uuid: {notificationUuid}", context: this);

            if (CachePendingNotificationOpen(openAction, notificationUuid) == false)
                return;

            if (TryApplyPendingNotificationOpen())
                ClearNativePendingNotificationOpen();
        }

        private void HandleIosNotificationDismissFromNative(string openAction, string notificationUuid)
        {
            HandleIosNotificationDismissFromNativeAsync(openAction, notificationUuid)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to handle iOS notification dismiss callback", this);
        }

        private async UniTask HandleIosNotificationDismissFromNativeAsync(string openAction, string notificationUuid)
        {
            await UniTask.SwitchToMainThread();

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.Information($"iOS notification dismiss callback. open_action: {openAction}, notification_uuid: {notificationUuid}", context: this);

            OnPushDismissed();
        }

        private void LogIosMessageReceived(MessageReceivedEventArgs eventArgs)
        {
            if (_logger.IsEnabled(LogLevel.Information) == false)
                return;

            var message = eventArgs.Message;
            var notification = message.Notification;
            var dataBuilder = new StringBuilder();

            foreach (var pair in message.Data)
            {
                if (dataBuilder.Length > 0)
                    dataBuilder.Append(", ");

                dataBuilder.Append(pair.Key);
                dataBuilder.Append('=');
                dataBuilder.Append(pair.Value);
            }

            _logger.Information(
                $"iOS Firebase MessageReceived. This is receive callback, not visual show confirmation. Notification title: {notification?.Title}, body: {notification?.Body}, data: [{dataBuilder}]",
                context: this
            );
        }
#endif

        private void CachePendingIosNotificationOpen()
        {
#if UNITY_IOS && !UNITY_EDITOR
            try
            {
                var openActionBuffer = new StringBuilder(IosNotificationOpenBufferSize);
                var uuidBuffer = new StringBuilder(IosNotificationOpenBufferSize);

                if (TryConsumePendingNotificationOpen(
                        openActionBuffer,
                        openActionBuffer.Capacity,
                        uuidBuffer,
                        uuidBuffer.Capacity
                    ) == 0)
                    return;

                var openAction = openActionBuffer.ToString();
                var uuid = uuidBuffer.ToString();

                OnInAppPushOpened();
                CachePendingNotificationOpen(openAction, uuid);
            }
            catch (Exception exception)
            {
                _logger.Warning($"Failed to read iOS notification open data: {exception.Message}", context: this);
            }
#endif
        }

        private void CachePendingIosNotificationDismiss()
        {
#if UNITY_IOS && !UNITY_EDITOR
            try
            {
                var openActionBuffer = new StringBuilder(IosNotificationOpenBufferSize);
                var uuidBuffer = new StringBuilder(IosNotificationOpenBufferSize);

                if (TryConsumePendingNotificationDismiss(
                        openActionBuffer,
                        openActionBuffer.Capacity,
                        uuidBuffer,
                        uuidBuffer.Capacity
                    ) == 0)
                    return;

                if (_logger.IsEnabled(LogLevel.Information))
                    _logger.Information($"iOS notification dismissed. open_action: {openActionBuffer}, notification_uuid: {uuidBuffer}", context: this);

                OnPushDismissed();
            }
            catch (Exception exception)
            {
                _logger.Warning($"Failed to read iOS notification dismiss data: {exception.Message}", context: this);
            }
#endif
        }

        private bool CachePendingNotificationOpen(string openAction, string uuid)
        {
            var nexusCategory = _serverConstConverter.ToENexusCategory(openAction);

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.Debug($"Cache pending notification open. open_action: {openAction}, notification_uuid: {uuid}, nexusCategory: {nexusCategory}", context: this);

            if (nexusCategory == ENexusCategory.None)
                return false;

            _pendingNexusCategory = nexusCategory;
            _pendingNotificationUuid = uuid;
            _hasPendingNotificationOpen = true;

            return true;
        }

        private void ClearNativePendingNotificationOpen()
        {
#if UNITY_IOS && !UNITY_EDITOR
            ClearPendingNotificationOpen();
#endif
        }

        private bool TryApplyPendingNotificationOpen()
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.Debug($"Try apply pending notification open. ready: {_isNotificationHandlingReady}, hasPending: {_hasPendingNotificationOpen}, category: {_pendingNexusCategory}, uuid: {_pendingNotificationUuid}", context: this);

            if (_isNotificationHandlingReady == false || _hasPendingNotificationOpen == false)
                return false;

            SendMonitoring(_pendingNotificationUuid);
            OpenDeepLink(_pendingNotificationUuid, _pendingNexusCategory);

            ClearNativePendingNotificationOpen();

            _hasPendingNotificationOpen = false;
            _pendingNotificationUuid = null;
            _pendingNexusCategory = ENexusCategory.None;

            return true;
        }

        private void OpenDeepLink(string uuid, ENexusCategory nexusCategory)
        {
            if (nexusCategory == ENexusCategory.None)
                return;

            _analyticsEvents.NotificationUUID = uuid;
            _analyticsEvents.Destination = AnalyticsDestination.Everywhere;

            _nexus.ChangeDockScreen(nexusCategory);
        }

        private void SendMonitoring(string uuid)
        {
            if (uuid == null)
                return;

            _client.NotificationOpened(uuid)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to send open notif. analytics", this);
        }
    }
}
