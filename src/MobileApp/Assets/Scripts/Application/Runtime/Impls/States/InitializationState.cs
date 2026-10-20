using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbifay.Analytics;
using Sotbifay.MobileApp.Assets;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Analytics;
using Sotbify.MobileApp.ApplicationUI;
using Sotbify.MobileApp.ApplicationUIScopeFacade;
using Sotbify.MobileApp.Common.Preferences;
using Sotbify.MobileApp.Localization;
using Sotbify.MobileApp.PermissionService;
using Sotbify.MobileApp.Services.FirebaseCrashlyticsService;
using Sotbify.MobileApp.Services.TutorialService;
using Sotbify.MobileApp.Utils;
using Sotbify.MobileApp.Utils.Data;
using Sotbify.MobileApp.Utils.Extensions;
using UState;
using VContainer;
using VContainer.Unity;
using UnityEngine;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

#if UNITY_IOS
using Services.AppTransparencyService;
#endif

namespace Sotbify.MobileApp.Application.States
{
    public class InitializationState : State<IApplication>
    {
        private const string MainSceneName = "Application";
        private const string QRStabTokenFirst = "$2a$04$RcpOVIvHvFzXLC.UXMm.ceCkoaosz3SiMDAsIzhdUtPKPDdm7Nx2m";
        private const string QRStabTokenSecond = "$3a$04$RcpOVIvHvFzXLC.UXMm.ceCkoaosz3SiMDAsIzhdUtPKPDdm7Nx2m";

        
        private readonly IApplication _application;
        private readonly LifetimeScope _parentLifetimeScope;
        private readonly ISceneLoader _sceneLoader;
        private readonly ILocalizationService _localizationService;
        private readonly IApplicationUIScopeFacade _applicationUIScopeFacade;
        
#if UNITY_IOS
        private readonly IAppTransparencyService _appTransparencyService;
#endif
        
        private readonly AnalyticsEvents _analyticsEvents;
        private readonly IPermissionService _permissionService;
        private readonly ITutorialStepService _tutorialStepService;
        private readonly IFirebaseCrashlyticsService _firebaseCrashlyticsService;
        private readonly IPreferences _preferences;
        private readonly ILogger _logger;

        private SceneInstance _applicationUIScene;

        public InitializationState(
            IApplication application,
            LifetimeScope parentLifetimeScope,
            ISceneLoader sceneLoader,
            ILocalizationService localizationService,
            IApplicationUIScopeFacade applicationUIScopeFacade,
            
#if UNITY_IOS
            IAppTransparencyService appTransparencyService,
#endif
            
            AnalyticsEvents analyticsEvents,
            IPermissionService permissionService,
            ITutorialStepService tutorialStepService,
            IFirebaseCrashlyticsService firebaseCrashlyticsService,
            ILogger logger,
            IPreferences preferences
        )
        {
            _application = application;
            _parentLifetimeScope = parentLifetimeScope;
            _sceneLoader = sceneLoader;
            _localizationService = localizationService;
            _applicationUIScopeFacade = applicationUIScopeFacade;
            
#if UNITY_IOS
            _appTransparencyService = appTransparencyService;
#endif
            
            _analyticsEvents = analyticsEvents;
            _permissionService = permissionService;
            _tutorialStepService = tutorialStepService;
            _firebaseCrashlyticsService = firebaseCrashlyticsService;
            _logger = logger;
            _preferences = preferences;
        }

        protected override async UniTask ExecuteAsync(CancellationToken cancellationToken)
        {
#if UNITY_IOS
            _appTransparencyService.Track();
            _appTransparencyService.RequestPermission();
#endif

            Debug.developerConsoleVisible = false;

            QualitySettings.vSyncCount = 0;
            UnityEngine.Application.targetFrameRate = GetMaxFrameRate();
            
            if (_preferences.Has(SavePath.TutorialStepsInitialized) == false || _preferences.GetBool(SavePath.TutorialStepsInitialized) == false)
                _tutorialStepService.Initialize();
            else
                _tutorialStepService.Load();

            await _localizationService.InitializeAsync(cancellationToken);
            
            _firebaseCrashlyticsService.Initialize(cancellationToken)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to initialize firebase crashlytics", this);
            
            _analyticsEvents.ApplicationLaunch();
            _analyticsEvents.InitialNotificationPermission(_permissionService.HasNotificationPermission() ? "allow" : "deny");

            var stabTokens = new StabQRTokens
            {
                Tokens = new[] { QRStabTokenFirst, QRStabTokenSecond },
            };
            
            if (_preferences.Has(SavePath.StabQRs) == false)
                _preferences.Set(SavePath.StabQRs, stabTokens);
            
            using (LifetimeScope.EnqueueParent(_parentLifetimeScope))
            {
                _applicationUIScene =
                    await _sceneLoader.LoadSceneAsync(MainSceneName, cancellationToken: cancellationToken);

                var lifetimeScope = LifetimeScope.Find<LifetimeScope>(_applicationUIScene.Scene);
                var applicationMenuResolver = lifetimeScope != null ? lifetimeScope.Container : null;

                if (applicationMenuResolver == null)
                {
                    _logger.Error($"Container in scene '{MainSceneName}' not found", context: this);
                    return;
                }

                if (applicationMenuResolver.TryResolve<IApplicationUI>(out var applicationMenu) == false)
                {
                    _logger.Error($"Resolve '{typeof(IApplicationUI)}' failed", context: this);
                    return;
                }

                _applicationUIScopeFacade.ApplicationUI = applicationMenu;
                _applicationUIScopeFacade.Scope = lifetimeScope;

                _application.RequestTransition<AuthenticationState>();
            }
        }
        
        private static int GetMaxFrameRate()
        {
            var refreshRateRatio = Screen.currentResolution.refreshRateRatio;
            var targetFrameRate = Mathf.CeilToInt((float)refreshRateRatio.value);
            return Mathf.Max(targetFrameRate, 60);
        }
    }
}