using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Analytics;
using Sotbify.MobileApp.FeedWindow.FeedList;
using Sotbify.MobileApp.FeedWindow.FeedListOverlay;
using Sotbify.MobileApp.Services.TutorialService;
using Sotbify.MobileApp.TutorialWindows.ChatTutorialOverlay;
using Sotbify.MobileApp.TutorialWindows.EventsTutorialOverlay;
using Sotbify.MobileApp.UI.Common.Providers;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils.Enums;
using UState;

namespace Sotbify.MobileApp.ApplicationUI.States
{
    public class FeedWindowState : State<IApplicationUI>
    {
        private readonly IWindowService _windowService;
        private readonly IApplicationUI _applicationUI;
        private readonly ITutorialStepService _tutorialStepService;
        private readonly IRootsRectProvider _rootsRectProvider;
        private readonly ILogger _logger;
        private readonly IFeedList _feedList;

        private CancellationTokenSource _categoryAwaitTokenSource;
        private readonly AnalyticsEvents _analyticsEvents;
        private Guid _guid;
        private bool _closeOnExit;

        public FeedWindowState (
            IWindowService windowService,
            ILogger logger,
            IFeedList  feedList,
            AnalyticsEvents analyticsEvents, 
            IApplicationUI applicationUI,
            ITutorialStepService tutorialStepService,
            IRootsRectProvider rootsRectProvider
        )
        {
            _windowService = windowService;
            _analyticsEvents = analyticsEvents;
            _applicationUI = applicationUI;
            _tutorialStepService = tutorialStepService;
            _rootsRectProvider = rootsRectProvider;
            _logger = logger;
            _feedList = feedList;
        }

        protected override async UniTask ExecuteAsync(CancellationToken cancellationToken)
        {
            _closeOnExit = true;
            
            _guid = Guid.NewGuid();
            _analyticsEvents.OpenCategory("feed", _guid);

            if (_windowService.IsOpen<IFeedList>() == false)
            {
                var feedOpenTask = _windowService.OpenAsync<IFeedList>(_rootsRectProvider.BackgroundBlurRoot, cancellationToken: cancellationToken);
                var feedListOverlay = _windowService.OpenAsync<IFeedListOverlay>(cancellationToken);
                
                await UniTask.WhenAll(feedOpenTask, feedListOverlay);
            }
            
            if (_tutorialStepService.HasStep(ETutorialStep.Events))
            {
                await _windowService.SafeOpenAsync<IEventsTutorialOverlay>(_rootsRectProvider.TutorialRoot, cancellationToken:cancellationToken);
                _tutorialStepService.TutorialStepPassed(ETutorialStep.Events);
            }

            var id = await _feedList.OpenContentObservable
                .FirstAsync(cancellationToken)
                .AsUniTask();

            _closeOnExit = false;
            
            _applicationUI.RequestTransition<NewsWindowState, NewsWindowState.Payload>(new NewsWindowState.Payload()
            {
                NewsId = id,
            });
        }

        protected override async UniTask ExitAsync(CancellationToken cancellationToken)
        {
            _analyticsEvents.CloseCategory("feed", _guid);

            if (_closeOnExit == false)
                return;
            
            var closeFeed = _windowService.IsOpen<IFeedList>()
                ? _windowService.CloseAsync<IFeedList>(cancellationToken)
                : UniTask.CompletedTask;
            var closeOverlay = _windowService.IsOpen<IFeedListOverlay>()
                ? _windowService.CloseAsync<IFeedListOverlay>(cancellationToken)
                : UniTask.CompletedTask;
            await UniTask.WhenAll(closeFeed, closeOverlay);
        }
    }    
}
