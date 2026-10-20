using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.Stab.AchievementsWindow;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils.Extensions;
using UState;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.ApplicationUI.States.Stab
{
    public class AchievementsWindowState : State<IApplicationUI>
    {
        private readonly IWindowService _windowService;
        private readonly IApplicationUI _applicationUI;
        private readonly ILogger _logger;

        public AchievementsWindowState(
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
            var achievements = await _windowService
                .OpenAsync<IAchievements>(cancellationToken);
            
            await achievements.CloseObservable
                .FirstAsync(cancellationToken)
                .AsUniTask();

            _applicationUI.Undo();
        }

        protected override UniTask ExitAsync(CancellationToken cancellationToken)
        {
            _windowService.CloseAsync<IAchievements>(cancellationToken)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to close achievements window", this);
            return base.ExitAsync(cancellationToken);
        }
    }
}