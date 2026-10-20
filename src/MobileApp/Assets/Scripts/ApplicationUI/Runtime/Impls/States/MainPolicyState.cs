using System.Threading;
using Cysharp.Threading.Tasks;
using MainPolicyWindow;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils.Extensions;
using UState;

namespace Sotbify.MobileApp.ApplicationUI.States
{
    public class MainPolicyState : State<IApplicationUI>
    {
        public override bool ShouldBeAddedToHistory => false;
        
        private readonly IWindowService _windowService;
        private readonly IApplicationUI _applicationUI;
        private readonly ILogger _logger;

        public MainPolicyState
        (
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
            var policyWindow = await _windowService.OpenAsync<IMainPolicy>(cancellationToken);

            await policyWindow.CloseButtonPressed
                .AnyAsync(cancellationToken)
                .AsUniTask();

            _applicationUI.RequestTransition<GreetingsWindowState>();
        }

        protected override UniTask ExitAsync(CancellationToken cancellationToken)
        {
            _windowService.CloseAsync<IMainPolicy>(cancellationToken).ForgetIgnoreOperationCancelledException(_logger, "Failed to close main policy window", this);
            return base.ExitAsync(cancellationToken);
        }
    }
}