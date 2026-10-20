using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.Analytics;
using Sotbify.MobileApp.BonusesWindow;
using Sotbify.MobileApp.LoadingScreen;
using Sotbify.MobileApp.Services.TutorialService;
using Sotbify.MobileApp.TutorialWindows.BonusTutorialOverlay;
using Sotbify.MobileApp.TutorialWindows.EventsTutorialOverlay;
using Sotbify.MobileApp.UI.Common.Providers;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils.Enums;
using Sotbify.MobileApp.Utils.Extensions;
using UState;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.ApplicationUI.States
{
    public class BonusesWindowState : State<IApplicationUI>
    {
        private readonly IWindowService _windowService;
        private readonly ILogger _logger;

        private readonly IBonusesWindow _bonusesScreen;
        private readonly AnalyticsEvents _analyticsEvents;
        private readonly ITutorialStepService _tutorialStepService;
        private readonly IRootsRectProvider _rootsRectProvider;
        private Guid _guid;

        public BonusesWindowState(
            IWindowService windowService,
            IBonusesWindow bonusesScreen,
            AnalyticsEvents analyticsEvents,
            ITutorialStepService tutorialStepService,
            IRootsRectProvider rootsRectProvider,
            ILogger logger
        )
        {
            _windowService = windowService;
            _bonusesScreen = bonusesScreen;
            _analyticsEvents = analyticsEvents;
            _tutorialStepService = tutorialStepService;
            _rootsRectProvider = rootsRectProvider;
            _logger = logger;
        }

        protected override async UniTask ExecuteAsync(CancellationToken cancellationToken)
        {
            _guid = Guid.NewGuid();
            _analyticsEvents.OpenCategory("bonus", _guid);
            
            await _windowService.OpenAsync<IBonusesWindow>(cancellationToken);
            await _windowService.SafeCloseAsync<ILoadingScreen>(cancellationToken);
            
            if (_tutorialStepService.HasStep(ETutorialStep.Bonus))
            {
                await _windowService.SafeOpenAsync<IBonusTutorialOverlay>(_rootsRectProvider.TutorialRoot, cancellationToken:cancellationToken);
                _tutorialStepService.TutorialStepPassed(ETutorialStep.Bonus);
            }
        }

        protected override UniTask ExitAsync(CancellationToken cancellationToken)
        {
            _bonusesScreen.Clear();
            _analyticsEvents.CloseCategory("bonus", _guid);
            _windowService.CloseAsync<IBonusesWindow>(CancellationToken.None)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to close Bonuses window", this);
            
            _windowService.SafeCloseAsync<IBonusTutorialOverlay>(cancellationToken:cancellationToken)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to close Bonuses tutorial window", this);
                
            return UniTask.CompletedTask;
        }
    }   
}