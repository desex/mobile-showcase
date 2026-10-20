using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.CheckPasswordWindow;
using Sotbify.MobileApp.ApplicationUI.States.Stab;
using Sotbify.MobileApp.PinPassword;
using Sotbify.MobileApp.UI.Common.Providers;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils.Extensions;
using UState;

namespace Sotbify.MobileApp.ApplicationUI.States
{
    public class CheckPasswordWindowState: State<IApplicationUI>
    {
        public override bool ShouldBeAddedToHistory => false;
        
        private readonly IWindowService _windowService;
        private readonly IRootsRectProvider _rootsRectProvider;
        private readonly IPinPasswordService _pinPasswordService;
        private readonly IApplicationUI _applicationUI;
        private readonly ILogger _logger;

        public CheckPasswordWindowState(
            IWindowService windowService,
            IRootsRectProvider rootsRectProvider,
            IPinPasswordService pinPasswordService,
            IApplicationUI applicationUI,
            ILogger logger
        )
        {
            _windowService = windowService;
            _rootsRectProvider = rootsRectProvider;
            _pinPasswordService = pinPasswordService;
            _applicationUI = applicationUI;
            _logger = logger;
        }

        protected override async UniTask ExecuteAsync(CancellationToken cancellationToken)
        {
            var checkPasswordWindow = await _windowService.OpenAsync<ICheckPasswordWindow>(cancellationToken);

            var validPasswordTask =  checkPasswordWindow.IsPasswordValid
                .FirstAsync(isValid => isValid, cancellationToken)
                .AsUniTask();
            
            var forgotPasswordTask = checkPasswordWindow.ForgotPasswordObservable
                .FirstAsync(cancellationToken)
                .AsUniTask();
            
            var (winTask, _, _) = await UniTask.WhenAny(validPasswordTask, forgotPasswordTask);

            switch (winTask)
            {
                case 0:
                    _applicationUI.RequestTransition<LoadingScreenState>();
                    break;
                case 1:
                    _applicationUI.RequestTransition<ScanWindowState>();
                    break;
            }
        }

        protected override UniTask ExitAsync(CancellationToken cancellationToken)
        {
            _windowService.CloseAsync<ICheckPasswordWindow>(cancellationToken)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to close PIN check window", this);
            return UniTask.CompletedTask;
        }
    }
}