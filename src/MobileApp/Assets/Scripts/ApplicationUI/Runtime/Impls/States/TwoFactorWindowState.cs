using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.ApplicationUI.States.Stab;
using Sotbify.MobileApp.TwoFactorWindow;
using Sotbify.MobileApp.UI.Common.Providers;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils.Extensions;
using UState;

namespace Sotbify.MobileApp.ApplicationUI.States
{
    public class TwoFactorWindowState : State<IApplicationUI>
    {
        public override bool ShouldBeAddedToHistory => false;

        private readonly IWindowService _windowService;
        private readonly IRootsRectProvider _rootsRectProvider;
        private readonly IApplicationUI _applicationUI;
        private readonly ILogger _logger;

        public TwoFactorWindowState(IWindowService windowService, IRootsRectProvider rootsRectProvider,
            IApplicationUI applicationUI, ILogger logger)
        {
            _windowService = windowService;
            _rootsRectProvider = rootsRectProvider;
            _applicationUI = applicationUI;
            _logger = logger;
        }

        protected override async UniTask ExecuteAsync(CancellationToken cancellationToken)
        {
            var window = await _windowService.OpenAsync<ITwoFactorWindow>(_rootsRectProvider.NexusRoot,
                cancellationToken: cancellationToken);
            
            var closeWindowTask = window.CloseWindowObservable
                .FirstAsync(cancellationToken)
                .AsUniTask();
            
            var changePinTask = window.ChangePinObservable
                .FirstAsync(cancellationToken)
                .AsUniTask();
            
            var (winTask, _, _) = await UniTask.WhenAny(closeWindowTask, changePinTask);

            switch (winTask)
            {
                case 0:
                    _applicationUI.Undo();
                    break;
                case 1:
                    _applicationUI.RequestTransition<PinCreationWindowState, PinCreationWindowState.Payload>(
                        new PinCreationWindowState.Payload
                        {
                            IsChangingPin = true,
                        });
                    break;
            }
        }

        protected override UniTask ExitAsync(CancellationToken cancellationToken)
        {
            _windowService.CloseAsync<ITwoFactorWindow>(cancellationToken)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to close TwoFactorWindow", this);
            return UniTask.CompletedTask;
        }
    }
}
