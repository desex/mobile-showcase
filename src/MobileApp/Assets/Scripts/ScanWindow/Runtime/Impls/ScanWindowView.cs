using System.Threading;
using Coffee.UIEffects;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Animation;
using R3;
using Sirenix.OdinInspector;
using Sotbify.MobileApp.Db.ScanParameters;
using Sotbify.MobileApp.UI.Window.View;
using Sotbify.MobileApp.Utils;
using Sotbify.MobileApp.Utils.Enums;
using Sotbify.MobileApp.Utils.Extensions;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using VContainer;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.ScanWindow
{
    public class ScanWindowView : FadeWindowView
    {
        private const string ScanTab = "Scan";
        private const string CameraTab = "Camera";
        
        [SerializeField] private Graphic[] _frameGraphics;
        [SerializeField] private RawImage _cameraImage;
        [SerializeField] private AspectRatioFitter _cameraRatioFitter;
        [SerializeField, TabGroup(ScanTab)] private Graphic _background;
        [SerializeField, TabGroup(ScanTab)] private Graphic _scanZoneBackground;
        [SerializeField, TabGroup(ScanTab)] private Graphic _scanZoneOutline;
        [SerializeField, TabGroup(ScanTab)] private EScanStateToTextCanvasGroup _scanStateToTextCanvasGroup;
        [SerializeField, TabGroup(ScanTab)] private EScanStateToIcon _scanStateToIcon;
        [SerializeField, TabGroup(ScanTab)] private CanvasGroup _scanAgainButtonCanvasGroup;
        [SerializeField, TabGroup(ScanTab)] private LitMotionAnimation _pendingFrameAnimation;
        [SerializeField, TabGroup(ScanTab)] private LitMotionAnimation _pendingIconAnimation;
        [SerializeField, TabGroup(ScanTab)] private CanvasGroup _closeButtonCanvasGroup;
        [SerializeField, TabGroup(ScanTab)] private Button _scanAgainButton;
        [SerializeField, TabGroup(ScanTab)] private Button _closeWindowButton;
        [SerializeField, TabGroup(ScanTab)] private MotionParameters _scanMotionParameters;

        [SerializeField, TabGroup(CameraTab)] private GameObject _cameraGroup;
        [SerializeField, TabGroup(CameraTab)] private Graphic _cameraIcon;
        [SerializeField, TabGroup(CameraTab)] private Graphic _deniedCameraIcon;
        [SerializeField, TabGroup(CameraTab)] private UIEffect _linearGradient;
        [SerializeField, TabGroup(CameraTab)] private CanvasGroup _deniedCameraAccessRadialCanvasGroup;
        [SerializeField, TabGroup(CameraTab)] private CanvasGroup _cameraAccessDeniedTextCanvasGroup;
        [SerializeField, TabGroup(CameraTab)] private CanvasGroup _grantAccessButtonCanvasGroup;
        [SerializeField, TabGroup(CameraTab)] private CanvasGroup _openSettingsButtonCanvasGroup;
        [SerializeField, TabGroup(CameraTab)] private Button _grantAccessButton;
        [SerializeField, TabGroup(CameraTab)] private Button _notNowButton;
        [SerializeField, TabGroup(CameraTab)] private Button _openSettingsButton;
        [SerializeField, TabGroup(CameraTab)] private MotionParameters _cameraMotionParameters;
        
        private IScanWindowParameters _scanWindowParameters;
        private CanvasGroup _activeTextCanvasGroup;
        private Graphic _activeIcon;
        private CancellationTokenSource _cancellationTokenSource;
        private ILogger _logger;
        private bool _animatingPendingIcon;

        [Inject]
        public void Construct(
            IScanWindowParameters scanWindowParameters,
            ILogger logger
        )
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public Observable<Unit> CloseWindowButtonObservable => _closeWindowButton.OnClickAsObservable();
        public Observable<Unit> ScanAgainButtonObservable => _scanAgainButton.OnClickAsObservable();
        public Observable<Unit> GrantAccessButtonObservable => _grantAccessButton.OnClickAsObservable();
        public Observable<Unit> NotNowButtonObservable => _notNowButton.OnClickAsObservable();
        public Observable<Unit> OpenSettingsButtonObservable => _openSettingsButton.OnClickAsObservable();

        protected override UniTask ShowInternalAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public async UniTask ChangeScanState(
            EScanWindowState scanWindowState,
            CancellationToken cancellationToken = default
        )
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public async UniTask ChangeCameraState(
            EPermissionStates permissionState,
            CancellationToken cancellationToken = default
        )
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        public void AssignCameraTexture(WebCamTexture texture, CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        private async UniTask CorrectTextureOrientation(WebCamTexture texture, CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private async UniTask AnimateCameraDeny(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void ResetState()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        private async UniTask PlayPendingIconAnimationAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        private async UniTask PlayPendingFrameAnimationAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

    }
}