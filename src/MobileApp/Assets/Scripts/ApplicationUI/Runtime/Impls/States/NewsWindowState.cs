using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.Analytics;
using Sotbify.MobileApp.NewsWindow;
using Sotbify.MobileApp.UI.Common.Providers;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils.Extensions;
using UnityEngine;
using UState;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.ApplicationUI.States
{
    public class NewsWindowState : State<IApplicationUI, NewsWindowState.Payload>
    {
        public struct Payload
        {
            public int NewsId;
        }
        
        private readonly IWindowService _windowService;
        private readonly IApplicationUI _applicationUI;
        private readonly ILogger _logger;
        private readonly RectTransform _root;
        private readonly INewsContent _news;
        private readonly AnalyticsEvents _analyticsEvents;
        private Guid _guid;


        public NewsWindowState(
            IWindowService windowService,
            IApplicationUI applicationUI,
            IRootsRectProvider rootsRectProvider,
            INewsContent news,
            AnalyticsEvents analyticsEvents,
            ILogger logger
        )
        {
            _windowService = windowService;
            _applicationUI = applicationUI;
            _logger = logger;
            _news = news;
            _analyticsEvents = analyticsEvents;
            _root = rootsRectProvider.NexusRoot;
        }

        protected override async UniTask ExecuteAsync(Payload payload, CancellationToken cancellationToken)
        {
            _guid = Guid.NewGuid();
            _analyticsEvents.OpenNews(payload.NewsId, _guid);
            await _windowService.OpenAsync<INewsContent>(_root, cancellationToken: cancellationToken);
            await _news.LoadContent(payload.NewsId, cancellationToken);

            await _news.CloseObservable
                .FirstAsync(cancellationToken)
                .AsUniTask();

            _applicationUI.Undo();
        }
        
        protected override UniTask ExitAsync(CancellationToken cancellationToken)
        {
            _analyticsEvents.CloseNews(_guid);
            if (_windowService.IsOpen<INewsContent>())
                _windowService.CloseAsync<INewsContent>(cancellationToken).ForgetIgnoreOperationCancelledException(_logger,"Cannot close news content", this);
            
            return UniTask.CompletedTask;
        }
    }
}