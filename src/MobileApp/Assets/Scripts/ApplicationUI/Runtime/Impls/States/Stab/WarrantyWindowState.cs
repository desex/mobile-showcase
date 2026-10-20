using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils.Extensions;
using Stab.WarrantyWindow;
using UState;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.ApplicationUI.States.Stab
{
    public class WarrantyWindowState : State<IApplicationUI>
    {
        private readonly IWindowService _windowService;
        private readonly IApplicationUI _applicationUI;
        private readonly ILogger _logger;

        public WarrantyWindowState(
            IWindowService windowService,
            IApplicationUI applicationUI,
            ILogger logger
        )
        {
            _windowService = windowService;
            _applicationUI = applicationUI;
            _logger = logger;
        }

        protected override async UniTask ExecuteAsync(CancellationToken cancellationToken)
        {
            var warrantyWindow = await _windowService
                .OpenAsync<IWarranty>(cancellationToken);
            
            await warrantyWindow.CloseObservable
                .FirstAsync(cancellationToken)
                .AsUniTask();
            
            _applicationUI.RequestTransition<StabProfileWindowState>();
        }

        protected override UniTask ExitAsync(CancellationToken cancellationToken)
        {
            _windowService.CloseAsync<IWarranty>(cancellationToken)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to close warranty window", this);
            return base.ExitAsync(cancellationToken);
        }
    }
}