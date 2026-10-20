using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.PinCreationWindow;
using Sotbify.MobileApp.UI.Common.Providers;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils.Extensions;
using UState;

namespace Sotbify.MobileApp.ApplicationUI.States
{
    public class PinCreationWindowState : State<IApplicationUI, PinCreationWindowState.Payload>
    {
        public struct Payload
        {
            public bool IsChangingPin;
        }

        private readonly IWindowService _windowService;
        private readonly IRootsRectProvider _rootsRectProvider;
        private readonly IPinCreationWindow _pinCreationWindow;
        private readonly IApplicationUI _applicationUI;
        private readonly ILogger _logger;

        public PinCreationWindowState
        (
            IWindowService windowService,
            IRootsRectProvider rootsRectProvider,
            IPinCreationWindow pinCreationWindow,
            IApplicationUI applicationUI,
            ILogger logger
        )
        {
            _windowService = windowService;
            _rootsRectProvider = rootsRectProvider;
            _pinCreationWindow = pinCreationWindow;
            _applicationUI = applicationUI;
            _logger = logger;
        }

        public override bool ShouldBeAddedToHistory => false;

        protected override async UniTask ExecuteAsync(Payload payload, CancellationToken cancellationToken)
        {
            _pinCreationWindow.SetChangingMode(payload.IsChangingPin);

            var window = await _windowService.OpenAsync<IPinCreationWindow>(_rootsRectProvider.NexusRoot,
                cancellationToken: cancellationToken);

            await window.ActionObservable
                .FirstAsync(cancellationToken)
                .AsUniTask();

            _applicationUI.Undo();
        }

        protected override UniTask ExitAsync(CancellationToken cancellationToken)
        {
            _windowService.CloseAsync<IPinCreationWindow>(cancellationToken)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to close PinCreationWindow", this);
            return UniTask.CompletedTask;
        }
    }
}