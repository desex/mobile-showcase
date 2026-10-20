using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.ApplicationUI.States.Stab;
using Sotbify.MobileApp.GlassesWindow;
using Sotbify.MobileApp.ScanWindow;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils.Extensions;
using UState;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.ApplicationUI.States
{
    public class ScanWindowState : State<IApplicationUI>
    {
        public override bool ShouldBeAddedToHistory => false;
        
        private readonly IWindowService _windowService;
        private readonly IApplicationUI _applicationUI;
        private readonly ILogger _logger;

        private IScanWindow _scanWindow;

        private IGlassesWindow _glassesWindow;
        
        public ScanWindowState(
            IWindowService windowService,
            ILogger logger, 
            IApplicationUI applicationUI
        )
        {
            _windowService = windowService;
            _logger = logger;
            _applicationUI = applicationUI;
        }

        protected override async UniTask ExecuteAsync(CancellationToken cancellationToken)
        {
            _scanWindow = await _windowService.OpenAsync<IScanWindow>(cancellationToken);
            
            WaitForCloseWindow(cancellationToken)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to wait for scan window close", this);
            
            var goToStabTask = _scanWindow.GoToStabApp
                .FirstAsync(cancellationToken)
                .AsUniTask();
            
            var goToAppTask = _scanWindow.GoToApp
                .FirstAsync(cancellationToken)
                .AsUniTask();
            
            var (winTask, _, _) = await UniTask.WhenAny(goToStabTask, goToAppTask);

            switch (winTask)
            {
                case 0:
                    _applicationUI.RequestTransition<StabMainInitializeState>();
                    break;
                case 1:
                    _applicationUI.RequestTransition<LoadingScreenState>();
                    break;
            }
        }

        protected override UniTask ExitAsync(CancellationToken cancellationToken)
        {
            _windowService.SafeCloseAsync<IScanWindow>(cancellationToken)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed close scan window", context: this);
                
            return UniTask.CompletedTask;
        }

        private async UniTask WaitForCloseWindow(CancellationToken cancellationToken)
        {
            await _scanWindow.CloseWindow
                .FirstAsync(cancellationToken)
                .AsUniTask();

            _applicationUI.RequestTransition<GreetingsWindowState>(true);
        }
    }   
}