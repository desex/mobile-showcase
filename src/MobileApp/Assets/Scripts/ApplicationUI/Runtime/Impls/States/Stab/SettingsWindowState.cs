using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.Stab.SettingsWindow;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils.Extensions;
using UState;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.ApplicationUI.States.Stab
{
    public class SettingsWindowState : State<IApplicationUI>
    {
        private readonly IWindowService _windowService;
        private readonly IApplicationUI _applicationUI;
        private readonly ILogger _logger;

        public SettingsWindowState(
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
            var settings = await _windowService
                .OpenAsync<ISettings>(cancellationToken);

            await settings.CloseObservable
                .FirstAsync(cancellationToken)
                .AsUniTask();

            _applicationUI.RequestTransition<StabProfileWindowState>();
        }

        protected override UniTask ExitAsync(CancellationToken cancellationToken)
        {
            _windowService.CloseAsync<ISettings>(cancellationToken)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to close settings window", this);
            return base.ExitAsync(cancellationToken);
        }
    }
}