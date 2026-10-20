using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.Analytics;
using Sotbify.MobileApp.PlayWindow;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils.Extensions;
using UState;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.ApplicationUI.States
{
    public class GameWindowState : State<IApplicationUI>
    {
        private readonly IWindowService _windowService;
        private readonly AnalyticsEvents _analyticsEvents;
        private readonly ILogger _logger;
        private Guid _guid;

        public GameWindowState(
            IWindowService windowService,
            AnalyticsEvents analyticsEvents,
            ILogger logger
        )
        {
            _windowService = windowService;
            _analyticsEvents = analyticsEvents;
            _logger = logger;
        }

        protected override async UniTask ExecuteAsync(CancellationToken cancellationToken)
        {
            _guid = Guid.NewGuid();
            _analyticsEvents.OpenCategory("game", _guid);
            await _windowService
                .OpenAsync<IGame>(cancellationToken);
        }

        protected override UniTask ExitAsync(CancellationToken cancellationToken)
        {
            _analyticsEvents.CloseCategory("game", _guid);
            _windowService.CloseAsync<IGame>(cancellationToken).
                ForgetIgnoreOperationCancelledException(_logger, "Failed to close game", this);
            return UniTask.CompletedTask;
        }
    }
}