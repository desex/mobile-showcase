using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Stab.StabNexus;
using Sotbify.MobileApp.UI.Common.Providers;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils.Extensions;
using Stab.StabHomeWindow;
using UState;

namespace Sotbify.MobileApp.ApplicationUI.States.Stab
{
    public class StabHomeState : State<IApplicationUI>
    {
        private readonly IWindowService _windowService;
        private readonly IApplicationUI _applicationUI;
        private readonly ILogger _logger;
        private readonly IRootsRectProvider _rootsRectProvider;

        public StabHomeState(
            IWindowService windowService,
            IApplicationUI applicationUI,
            ILogger logger,
            IRootsRectProvider rootsRectProvider
        )
        {
            _windowService = windowService;
            _applicationUI = applicationUI;
            _logger = logger;
            _rootsRectProvider = rootsRectProvider;
        }

        protected override async UniTask ExecuteAsync(CancellationToken cancellationToken)
        {
            if (_windowService.IsOpen<IStabNexus>() == false)
                await _windowService.OpenAsync<IStabNexus>(_rootsRectProvider.NexusRoot, cancellationToken: cancellationToken);
            
            var homeScreen = await _windowService.OpenAsync<IStabHome>(cancellationToken);
            
            var homeCategory = await homeScreen.ChangeScreen
                .FirstAsync(cancellationToken)
                .AsUniTask();

            _windowService.CloseAsync<IStabNexus>(cancellationToken)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to close dock", this);
            
            switch (homeCategory)
            {
                case EHomeItem.Achievements:
                    _applicationUI
                        .RequestTransition<AchievementsWindowState>();
                    break;
                
                case EHomeItem.Guide:
                    _applicationUI.RequestTransition<VRGuideWindowState>();
                    break;
            }
        }

        protected override UniTask ExitAsync(CancellationToken cancellationToken)
        {
            _windowService.CloseAsync<IStabHome>(cancellationToken)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to close stab home", this);
            return UniTask.CompletedTask;
        }
    }
}