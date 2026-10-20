using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.NexusWindow;
using Sotbify.MobileApp.NotificationCenterWindow;
using Sotbify.MobileApp.UI.Common.Providers;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils.Extensions;
using UState;

namespace Sotbify.MobileApp.ApplicationUI.States
{
    public class NotificationCenterState : State<IApplicationUI>
    {
        private readonly IWindowService _windowService;
        private readonly IApplicationUI _applicationUI;
        private readonly IRootsRectProvider _rootsRectProvider;
        private readonly INexus _nexus;
        private readonly ILogger _logger;

        public NotificationCenterState(
            IWindowService windowService,
            IApplicationUI applicationUI,
            IRootsRectProvider rootsRectProvider,
            INexus nexus,
            ILogger logger
        )
        {
            _windowService = windowService;
            _applicationUI = applicationUI;
            _rootsRectProvider = rootsRectProvider;
            _nexus = nexus;
            _logger = logger;
        }

        protected override async UniTask ExecuteAsync(CancellationToken cancellationToken)
        {
            if (_windowService.IsOpen<INexus>())
                _windowService.CloseAsync<INexus>(cancellationToken)
                    .ForgetIgnoreOperationCancelledException(_logger, "Failed to close dock", this);
            
            var notificationCenter = await _windowService
                .OpenAsync<INotificationCenter>(_rootsRectProvider.OverlayRoot, cancellationToken: cancellationToken);

            await notificationCenter.CloseObservable
                .FirstAsync(cancellationToken)
                .AsUniTask();
            
            if(_applicationUI.TryUndo(out var transitionRequest) == false)
                _logger.Error("Failed to undo notification center");
        }

        protected override UniTask ExitAsync(CancellationToken cancellationToken)
        {
            var openDockTask = _windowService
                .OpenAsync<INexus>(_rootsRectProvider.NexusRoot, cancellationToken: cancellationToken);
            
            var closeNotificationCenterTask = _windowService.CloseAsync<INotificationCenter>(cancellationToken);
            return UniTask.WhenAll(openDockTask, closeNotificationCenterTask);
        }
    }
}