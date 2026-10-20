using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.Stab.StabNexus;
using Sotbify.MobileApp.UI.Common.Providers;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils;
using Sotbify.MobileApp.Utils.Extensions;
using Stab.StabProfileWindow;
using UState;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.ApplicationUI.States.Stab
{
    public class StabProfileWindowState : State<IApplicationUI>
    {
        private readonly IStabNexus _stabNexus;
        private readonly ILogger _logger;
        private readonly IApplicationUI _applicationUI;
        private readonly IWindowService _windowService;
        private readonly IRootsRectProvider _rootsRectProvider;
        
        private bool _shouldCloseOnExit = true;

        public StabProfileWindowState(
            IStabNexus stabNexus,
            ILogger logger,
            IApplicationUI applicationUI,
            IWindowService windowService,
            IRootsRectProvider rootsRectProvider
        )
        {
            _stabNexus = stabNexus;
            _logger = logger;
            _applicationUI = applicationUI;
            _windowService = windowService;
            _rootsRectProvider = rootsRectProvider;
        }
        
        protected override async UniTask ExecuteAsync(CancellationToken cancellationToken)
        {
            if (_windowService.IsOpen<IStabNexus>() == false)
                await _windowService.OpenAsync<IStabNexus>(_rootsRectProvider.NexusRoot, cancellationToken: cancellationToken);
            
            var tasksWindow = await _windowService.OpenAsync<IStabProfile>(cancellationToken);

            var nextProfileCategoryTask = tasksWindow.ProfileCategoryObservable
                .FirstAsync(cancellationToken)
                .AsUniTask();

            var deleteAccountTask = tasksWindow.DeleteAccountObservable
                .FirstAsync(cancellationToken)
                .AsUniTask();
            
            var (winTask, nextProfileCategory, _) = await UniTask.WhenAny(nextProfileCategoryTask, deleteAccountTask);

            switch (winTask)
            {
                case 0:
                    _shouldCloseOnExit = true;
                    await HandleProfileCategory(nextProfileCategory, cancellationToken);
                    break;
                
                case 1:
                    _shouldCloseOnExit = false;
                    _applicationUI.RequestTransition<DeleteAccountState>();
                    break;
            }
        }
        
        private UniTask HandleProfileCategory(EProfileCategory nextProfileCategory, CancellationToken cancellationToken)
        {
            _windowService.CloseAsync<IStabNexus>(cancellationToken)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to close dock", this);
            
            switch (nextProfileCategory)
            {
                case EProfileCategory.Warranty:
                    _applicationUI.RequestTransition<WarrantyWindowState>();
                    break;
                
                case EProfileCategory.Achievements:
                    _applicationUI.RequestTransition<AchievementsWindowState>();
                    break;
                
                case EProfileCategory.Settings:
                    _applicationUI.RequestTransition<SettingsWindowState>();
                    break;
            }
            return UniTask.CompletedTask;
        }

        protected override UniTask ExitAsync(CancellationToken cancellationToken)
        {
            if (_shouldCloseOnExit)
                _windowService.CloseAsync<IStabProfile>(cancellationToken)
                    .ForgetIgnoreOperationCancelledException(_logger, "Failed to close stab profile", this);
            
            return UniTask.CompletedTask;
        }
    }
}