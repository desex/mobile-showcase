using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.AllRelicsWindow;
using Sotbify.MobileApp.Analytics;
using Sotbify.MobileApp.RelicListWindow;
using Sotbify.MobileApp.Services.TutorialService;
using Sotbify.MobileApp.TutorialWindows.RelicsTutorialOverlay;
using Sotbify.MobileApp.UI.Common.Providers;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils.Enums;
using Sotbify.MobileApp.Utils.Extensions;
using UState;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.ApplicationUI.States
{
    public class CollectionWindowState : State<IApplicationUI>
    {
        private readonly IRelicListWindow _relicListWindow;
        private readonly IAllRelicsWindow _allRelicsWindow;
        private readonly IWindowService _windowService;
        private readonly IRootsRectProvider _rootsRectProvider;
        private readonly ITutorialStepService _tutorialStepService;
        private readonly AnalyticsEvents _analyticsEvents;
        private readonly IApplicationUI _applicationUI;
        private readonly ILogger _logger;

        private Guid _guid;
        private bool _closeOnExit;

        public CollectionWindowState
        (
            IRelicListWindow relicListWindow,
            IAllRelicsWindow allRelicsWindow,
            IWindowService windowService,
            IRootsRectProvider rootsRectProvider,
            ITutorialStepService tutorialStepService,
            AnalyticsEvents analyticsEvents,
            IApplicationUI applicationUI,
            ILogger logger
        )
        {
            _relicListWindow = relicListWindow ?? throw new ArgumentNullException(nameof(relicListWindow));
            _allRelicsWindow = allRelicsWindow ?? throw new ArgumentNullException(nameof(allRelicsWindow));
            _windowService = windowService ?? throw new ArgumentNullException(nameof(windowService));
            _rootsRectProvider = rootsRectProvider ?? throw new ArgumentNullException(nameof(rootsRectProvider));
            _tutorialStepService = tutorialStepService ?? throw new ArgumentNullException(nameof(tutorialStepService));
            _analyticsEvents = analyticsEvents ?? throw new ArgumentNullException(nameof(analyticsEvents));
            _applicationUI = applicationUI ?? throw new ArgumentNullException(nameof(applicationUI));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async UniTask ExecuteAsync(CancellationToken cancellationToken)
        {
            _guid = Guid.NewGuid();
            _analyticsEvents.OpenCategory("collection", _guid);

            _closeOnExit = true;

            var loadRelicsTask = _relicListWindow.LoadRelics(cancellationToken);
            var openWindowTask = _windowService.OpenAsync<IRelicListWindow>
            (
                _rootsRectProvider.BackgroundBlurRoot,
                cancellationToken: cancellationToken
            );

            await UniTask.WhenAll(loadRelicsTask, openWindowTask);

            var relics = _relicListWindow.Relics.CurrentValue;
            var totalRelicsCount = _relicListWindow.TotalRelicsCount.CurrentValue;

            if (relics.Count >= totalRelicsCount && _allRelicsWindow.WasShowed == false)
            {
                var allRelicsWindow = await _windowService.OpenAsync<IAllRelicsWindow>
                (
                    _rootsRectProvider.NexusRoot,
                    cancellationToken: cancellationToken
                );

                await allRelicsWindow.CloseObservable
                    .FirstAsync(cancellationToken)
                    .AsUniTask();

                await _windowService.SafeCloseAsync<IAllRelicsWindow>(cancellationToken);
            }
            
            if (_tutorialStepService.HasStep(ETutorialStep.Relics))
            {
                await _windowService.SafeOpenAsync<IRelicsTutorialOverlay>(_rootsRectProvider.TutorialRoot, cancellationToken:cancellationToken);
                _tutorialStepService.TutorialStepPassed(ETutorialStep.Relics);
            }

            var modelId = await _relicListWindow.OpenDescriptionObservable
                .FirstAsync(cancellationToken)
                .AsUniTask();

            _closeOnExit = false;

            _applicationUI.RequestTransition<RelicDescriptionWindowState, RelicDescriptionWindowState.Payload>
            (
                new RelicDescriptionWindowState.Payload
                {
                    RelicId = modelId,
                }
            );
        }

        protected override UniTask ExitAsync(CancellationToken cancellationToken)
        {
            if (_closeOnExit == false)
                return UniTask.CompletedTask;

            _analyticsEvents.CloseCategory("collection", _guid);
            
            _windowService.SafeCloseAsync<IRelicsTutorialOverlay>(cancellationToken)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to close relic tutorial overlay", this);

            _windowService.SafeCloseAsync<IAllRelicsWindow>(cancellationToken)
                .SuppressCancellationThrow()
                .Forget(e => _logger.Error("Failed to close all relics window", e, this));

            _windowService.CloseAsync<IRelicListWindow>(cancellationToken)
                .SuppressCancellationThrow()
                .Forget(e => _logger.Error("Failed to close relic list window", e, this));

            return UniTask.CompletedTask;
        }
    }
}