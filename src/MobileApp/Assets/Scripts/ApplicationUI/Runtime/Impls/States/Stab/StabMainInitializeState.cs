using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.PermissionService;
using Sotbify.MobileApp.Services.FirebaseCM.Runtime;
using Sotbify.MobileApp.Stab.StabNexus;
using Sotbify.MobileApp.UI.Common.Providers;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils.Extensions;
using UState;

namespace Sotbify.MobileApp.ApplicationUI.States.Stab
{
    public class StabMainInitializeState : State<IApplicationUI>
    {
        private readonly IWindowService _windowService;
        private readonly IRootsRectProvider _rootsRectProvider;
        private readonly IApplicationUI _applicationUI;
        private readonly IPermissionService _permissionService;
        private readonly IFirebaseCMService _firebaseCMService;
        private readonly ILogger _logger;

        public StabMainInitializeState(
            IWindowService windowService,
            IRootsRectProvider rootsRectProvider,
            IApplicationUI applicationUI, 
            IPermissionService permissionService,
            IFirebaseCMService firebaseCMService,
            ILogger logger
        )
        {
            _windowService = windowService;
            _rootsRectProvider = rootsRectProvider;
            _applicationUI = applicationUI;
            _permissionService = permissionService;
            _firebaseCMService = firebaseCMService;
            _logger = logger;
        }

        protected override async UniTask ExecuteAsync(CancellationToken cancellationToken)
        {
            await RequestPermissions(cancellationToken);
            await _windowService.OpenAsync<IStabNexus>(_rootsRectProvider.NexusRoot, cancellationToken: cancellationToken);
            _applicationUI.RequestTransition<StabHomeState>();
        }

        private UniTask RequestPermissions(CancellationToken cancellationToken)
        {
            _firebaseCMService.Initialize(cancellationToken)
                .ForgetIgnoreOperationCancelledException(_logger, "Error during FirebaseCM initialization", this);
            _permissionService.RequestNotificationPermission()
                .ForgetIgnoreOperationCancelledException(_logger, "Exception while requesting notification permission", this);
            return UniTask.CompletedTask;
        }
    }
}