using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.HiddenModeWindow;
using Sotbify.MobileApp.UI.Common.Providers;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils.Extensions;
using UState;

namespace Sotbify.MobileApp.ApplicationUI.States
{
    public class HiddenModeWindowState : State<IApplicationUI>
    {
        private readonly IWindowService _windowService;
        private readonly IRootsRectProvider _rootsRectProvider;
        private readonly IApplicationUI _applicationUI;
        private readonly ILogger _logger;

        public HiddenModeWindowState
        (
            IWindowService windowService,
            IRootsRectProvider rootsRectProvider,
            IApplicationUI applicationUI,
            ILogger logger
        )
        {
            _windowService = windowService;
            _rootsRectProvider = rootsRectProvider;
            _applicationUI = applicationUI;
            _logger = logger;
        }

        public override bool ShouldBeAddedToHistory => false;

        protected override async UniTask ExecuteAsync(CancellationToken cancellationToken)
        {
            var window = await _windowService.OpenAsync<IHiddenModeWindow>(_rootsRectProvider.NexusRoot,
                cancellationToken: cancellationToken);
            var proceed = await window.CreatePinObservable.FirstAsync(cancellationToken).AsUniTask();

            if (proceed)
                _applicationUI.RequestTransition<PinCreationWindowState, PinCreationWindowState.Payload>(
                    new PinCreationWindowState.Payload
                    {
                        IsChangingPin = false,
                    });
            else
                _applicationUI.Undo();
        }

        protected override UniTask ExitAsync(CancellationToken cancellationToken)
        {
            _windowService.CloseAsync<IHiddenModeWindow>(cancellationToken)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to close HiddenModeWindow", this);
            return UniTask.CompletedTask;
        }
    }
}