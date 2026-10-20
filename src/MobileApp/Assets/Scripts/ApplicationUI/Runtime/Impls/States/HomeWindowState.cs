using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Analytics;
using Sotbify.MobileApp.BlurredBackgroundWindow;
using Sotbify.MobileApp.HomeWindow;
using Sotbify.MobileApp.LoadingScreen;
using Sotbify.MobileApp.NexusWindow;
using Sotbify.MobileApp.Services.TutorialService;
using Sotbify.MobileApp.TutorialWindows.HomeTutorialOverlay;
using Sotbify.MobileApp.TutorialWindows.WelcomeTutorialOverlay;
using Sotbify.MobileApp.UI.Common.Providers;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils.Enums;
using Sotbify.MobileApp.Utils.Enums.Nexus;
using Sotbify.MobileApp.Utils.Extensions;
using UState;

namespace Sotbify.MobileApp.ApplicationUI.States
{
    public class HomeWindowState : State<IApplicationUI>
    {
        private readonly IHome _home;
        private readonly IWindowService _windowService;
        private readonly INexus _nexus;
        private readonly AnalyticsEvents _analyticsEvents;
        private readonly ITutorialStepService _tutorialStepService;
        private readonly IRootsRectProvider _rootsRectProvider;
        private readonly ILogger _logger;

        private CancellationTokenSource _categoryAwaitTokenSource;
        private Guid _guid;

        public HomeWindowState 
        (
            IHome home,
            IWindowService windowService,
            INexus nexus,
            AnalyticsEvents analyticsEvents,
            ITutorialStepService tutorialStepService,
            IRootsRectProvider rootsRectProvider, 
            ILogger logger
        )
        {
            _home = home ?? throw new ArgumentNullException(nameof(home));
            _windowService = windowService ?? throw new ArgumentNullException(nameof(windowService));
            _nexus = nexus ?? throw new ArgumentNullException(nameof(nexus));
            _analyticsEvents = analyticsEvents ?? throw new ArgumentNullException(nameof(analyticsEvents));
            _tutorialStepService = tutorialStepService;
            _rootsRectProvider = rootsRectProvider ?? throw new ArgumentNullException(nameof(rootsRectProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async UniTask ExecuteAsync(CancellationToken cancellationToken)
        {
            _guid = Guid.NewGuid();
            _analyticsEvents.OpenCategory("home", _guid);

            if (_windowService.IsOpen<IHome>())
                return;
            
            _home.LoadItemsAsync(cancellationToken)
                .Forget();
            
            var homeOpenTask = _windowService.OpenAsync<IHome>(cancellationToken);
            var backgroundBlurOpenTask = _windowService.IsOpen<IBlurredBackground>() == false
                ? _windowService.OpenAsync<IBlurredBackground>(_rootsRectProvider.BackgroundBlurRoot, cancellationToken: cancellationToken)
                : UniTask.CompletedTask;
                
            await backgroundBlurOpenTask;
            await homeOpenTask;

            await _windowService.SafeCloseAsync<ILoadingScreen>(cancellationToken);

            var hasWelcomeStep = _tutorialStepService.HasStep(ETutorialStep.Welcome);
            var hasHomeStep = _tutorialStepService.HasStep(ETutorialStep.Home);
            if (hasWelcomeStep)
            {
                await _windowService.SafeOpenAsync<IWelcomeTutorialOverlay>(_rootsRectProvider.TutorialRoot, cancellationToken:cancellationToken);
                _tutorialStepService.TutorialStepPassed(ETutorialStep.Welcome);
            }

            if (hasHomeStep && hasWelcomeStep == false)
            {
                await _windowService.SafeOpenAsync<IHomeTutorialOverlay>(_rootsRectProvider.TutorialRoot, cancellationToken:cancellationToken);
                _tutorialStepService.TutorialStepPassed(ETutorialStep.Home);
            }

            await _home.EditActiveSetObservable
                .AnyAsync(cancellationToken: cancellationToken);
                
            if(_logger.IsEnabled(LogLevel.Information))
                _logger.Information("Edit active set", context: this);

            _nexus.ChangeDockScreen(ENexusCategory.ActiveSet);
        }

        protected override UniTask ExitAsync(CancellationToken cancellationToken)
        {
            _analyticsEvents.CloseCategory("home", _guid);
            
            _windowService.SafeCloseAsync<IWelcomeTutorialOverlay>(cancellationToken)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to close welcome overlay", this);
            
            _windowService.SafeCloseAsync<IHomeTutorialOverlay>(cancellationToken)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to close home overlay", this);
            
            _windowService.CloseAsync<IHome>(cancellationToken)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to close home", this);
            
            _windowService.CloseAsync<IBlurredBackground>(cancellationToken)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to close blurred background", this);
            
            return UniTask.CompletedTask;
        }
    }    
}