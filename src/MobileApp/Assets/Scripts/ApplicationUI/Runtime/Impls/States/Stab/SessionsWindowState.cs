using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils.Extensions;
using Stab.SessionsWindow;
using UState;

namespace Sotbify.MobileApp.ApplicationUI.States.Stab
{
    public class SessionsWindowState : State<IApplicationUI>
    {
        private readonly IWindowService _windowService;
        private readonly ILogger _logger;

        public SessionsWindowState(
            IWindowService windowService,
            ILogger logger
        )
        {
            _windowService = windowService;
            _logger = logger;
        }

        protected override async UniTask ExecuteAsync(CancellationToken cancellationToken)
        {
            await _windowService.OpenAsync<ISessions>(cancellationToken);
        }

        protected override UniTask ExitAsync(CancellationToken cancellationToken)
        {
            _windowService.CloseAsync<ISessions>(cancellationToken)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to close music window", this);
            return base.ExitAsync(cancellationToken);
        }
    }
}