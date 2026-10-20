using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.Chat;
using Sotbify.MobileApp.Db.MediaPathConfig;
using Sotbify.MobileApp.LoadingScreen;
using Sotbify.MobileApp.PermissionService;
using Sotbify.MobileApp.Services.FirebaseCM.Runtime;
using Sotbify.MobileApp.Services.PartnersService;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils.Enums;
using Sotbify.MobileApp.VideoPlayer;
using UnityEngine.Pool;
using UState;

namespace Sotbify.MobileApp.ApplicationUI.States
{
    public class LoadingScreenState : State<IApplicationUI>
    {
        public override bool ShouldBeAddedToHistory => false;
        
        private readonly IApplicationUI _applicationUI;
        private readonly IWindowService _windowService;
        private readonly IPartnersService _partnersService;
        private readonly IFirebaseCMService _firebaseCMService;
        private readonly IPermissionService _permissionService;
        private readonly ILoadingScreen _loadingScreen;
        private readonly IMediaPathConfig _mediaPathConfig;
        private readonly IVideoPlayer _videoPlayer;
        private readonly IChat _chat;
        private bool _handledPendingNotification;

        public LoadingScreenState
        (
            IApplicationUI applicationUI,
            IWindowService windowService,
            IPartnersService partnersService,
            IFirebaseCMService firebaseCMService,
            IPermissionService permissionService,
            ILoadingScreen loadingScreen,
            IMediaPathConfig mediaPathConfig,
            IVideoPlayer videoPlayer,
            IChat chat
        )
        {
            _applicationUI = applicationUI ?? throw new ArgumentNullException(nameof(applicationUI));
            _windowService = windowService ?? throw new ArgumentNullException(nameof(windowService));
            _partnersService = partnersService ?? throw new ArgumentNullException(nameof(partnersService));
            _firebaseCMService = firebaseCMService ?? throw new ArgumentNullException(nameof(firebaseCMService));
            _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
            _loadingScreen = loadingScreen ?? throw new ArgumentNullException(nameof(loadingScreen));
            _mediaPathConfig = mediaPathConfig;
            _videoPlayer = videoPlayer ?? throw new ArgumentNullException(nameof(videoPlayer));
            _chat = chat ?? throw new ArgumentNullException(nameof(chat));
        }

        protected override async UniTask ExecuteAsync(CancellationToken cancellationToken)
        {
            _chat.Start();
            _loadingScreen.AssignPlayer(null);
            
            await _windowService.SafeOpenAsync<ILoadingScreen>(cancellationToken);

            await PlayVideo(cancellationToken);
            
            await HandleNotifications(cancellationToken);

            using (ListPool<UniTask>.Get(out var taskList))
            {
                if (_partnersService.InitializingState == EInitializingState.None)
                    taskList.Add(_partnersService
                        .InitializeAsync(cancellationToken));

                await UniTask.WhenAll(taskList);
            }

            _handledPendingNotification = _firebaseCMService.TryHandlePendingNotificationOpen();
            
            if (_handledPendingNotification)
                return;

            _applicationUI.RequestTransition<DailyRelicsState>();
        }

        private async UniTask PlayVideo(CancellationToken cancellationToken)
        {
            var mediaPlayer = _videoPlayer.MediaPlayer;
            
            await _videoPlayer.PrepareVideo(_mediaPathConfig.MediaPaths["Loading"], cancellationToken);
            
            _videoPlayer.PlayCurrentVideo();
            await _videoPlayer.WaitForFirstFrame(cancellationToken);
            _loadingScreen.AssignPlayer(mediaPlayer);
        }

        private async UniTask HandleNotifications(CancellationToken cancellationToken)
        {
            var permissionState = await _permissionService.GetNotificationPermissionStateAsync(cancellationToken);
            if (permissionState is ENotificationPermissionState.Denied
                or ENotificationPermissionState.NotDetermined
                or ENotificationPermissionState.Unknown)
                return;

            await _firebaseCMService.Initialize(cancellationToken);
            await _firebaseCMService.Subscribe();
        }
    }
}