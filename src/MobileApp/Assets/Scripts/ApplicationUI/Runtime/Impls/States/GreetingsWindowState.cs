using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.GreetingsWindow;
using Sotbify.MobileApp.PermissionService;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils.Extensions;
using UState;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

#if UNITY_ANDROID || UNITY_IOS
using UnityEngine.Android;
#endif

namespace Sotbify.MobileApp.ApplicationUI.States
{
    public class GreetingsWindowState : State<IApplicationUI>
    {
        public override bool ShouldBeAddedToHistory => false;
        
        private readonly IApplicationUI _applicationUI;
        private readonly IPermissionService _permissionService;
        private readonly ILogger _logger;
        private readonly IWindowService _windowService;

        public GreetingsWindowState(
            IWindowService windowService, 
            IApplicationUI applicationUI,
            IPermissionService permissionService,
            ILogger logger
        )
        {
            _windowService = windowService;
            _applicationUI = applicationUI;
            _permissionService = permissionService;
            _logger = logger;
        }

        protected override async UniTask ExecuteAsync(CancellationToken cancellationToken)
        {
            var greetingsWindow = await _windowService.OpenAsync<IGreetingsWindow>(cancellationToken);
            
            var policyTask = greetingsWindow.OpenPolicyPressed
                .FirstAsync(cancellationToken)
                .AsUniTask();
            
            var scanTask = greetingsWindow.ScanButtonPressed
                .FirstAsync(cancellationToken)
                .AsUniTask();

            var (winTask, _, _) = await UniTask.WhenAny(policyTask, scanTask);

            switch (winTask)
            {
                case 0:
                    _applicationUI.RequestTransition<MainPolicyState>();
                    break;
                case 1:
                    _applicationUI.RequestTransition<ScanWindowState>();
                    break;
            }
        }
        
        protected override UniTask ExitAsync(CancellationToken cancellationToken)
        {
            _windowService.CloseAsync<IGreetingsWindow>(cancellationToken).ForgetIgnoreOperationCancelledException(_logger,"Cannot close greeting window", this);
            return UniTask.CompletedTask;
        }
    }
}