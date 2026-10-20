using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.Analytics;
using Sotbify.MobileApp.RelicActiveSetWindow;
using Sotbify.MobileApp.Services.TutorialService;
using Sotbify.MobileApp.TutorialWindows.ActiveSetTutorialOverlay;
using Sotbify.MobileApp.UI.Common.Providers;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils.Enums;
using Sotbify.MobileApp.Utils.Extensions;
using UState;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.ApplicationUI.States
{
    public class ActiveSetState : State<IApplicationUI>
    {
        private readonly IWindowService _windowService;
        private readonly IRootsRectProvider _rootsRectProvider;
        private readonly ITutorialStepService _tutorialStepService;
        private readonly AnalyticsEvents _analyticsEvents;
        private readonly ILogger _logger;
        private Guid _guid;

        public ActiveSetState(
            IWindowService windowService,
            IRootsRectProvider rootsRectProvider,
            ITutorialStepService tutorialStepService,
            AnalyticsEvents analyticsEvents,
            ILogger logger
        )
        {
            _windowService = windowService;
            _rootsRectProvider = rootsRectProvider;
            _tutorialStepService = tutorialStepService;
            _analyticsEvents = analyticsEvents;
            _logger = logger;
        }

        protected override async UniTask ExecuteAsync(CancellationToken cancellationToken)
        {
            _guid = Guid.NewGuid();
            _analyticsEvents.OpenCategory("activeSet", _guid);

            await _windowService
                .OpenAsync<IRelicActiveSet>(_rootsRectProvider.BackgroundBlurRoot, cancellationToken: cancellationToken);

            if (_tutorialStepService.HasStep(ETutorialStep.ActiveSet))
            {
                await _windowService.SafeOpenAsync<IActiveSetTutorialOverlay>(_rootsRectProvider.TutorialRoot, cancellationToken:cancellationToken);
                _tutorialStepService.TutorialStepPassed(ETutorialStep.ActiveSet);
            }
        }

        protected override UniTask ExitAsync(CancellationToken cancellationToken)
        {
            _analyticsEvents.CloseCategory("activeSet", _guid);
            
            _windowService.SafeCloseAsync<IActiveSetTutorialOverlay>(cancellationToken)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to close active set tutorial overlay", this);
            
            _windowService.CloseAsync<IRelicActiveSet>(cancellationToken)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to close relic list window", this);

            return UniTask.CompletedTask;
        }
    }
}