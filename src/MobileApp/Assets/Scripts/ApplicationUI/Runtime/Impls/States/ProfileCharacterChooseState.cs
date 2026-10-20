using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.ProfileCharacterChoose;
using Sotbify.MobileApp.UI.Common.Providers;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils.Extensions;
using UState;

namespace Sotbify.MobileApp.ApplicationUI.States
{
    public class ProfileCharacterChooseState : State<IApplicationUI>
    {
        private readonly IApplicationUI _applicationUI;
        private readonly IProfileCharacterChoose _profileCharacterChoose;
        private readonly IRootsRectProvider _rootsRectProvider;
        private readonly ILogger _logger;
        private readonly IWindowService _windowService;

        public ProfileCharacterChooseState(
            IApplicationUI applicationUI,
            IProfileCharacterChoose profileCharacterChoose,
            IRootsRectProvider rootsRectProvider,
            ILogger logger,
            IWindowService windowService
        )
        {
            _applicationUI = applicationUI;
            _profileCharacterChoose = profileCharacterChoose;
            _rootsRectProvider = rootsRectProvider;
            _logger = logger;
            _windowService = windowService;
        }

        protected override async UniTask ExecuteAsync(CancellationToken cancellationToken)
        {
            await _windowService
                .OpenAsync<IProfileCharacterChoose>(_rootsRectProvider.NexusRoot, cancellationToken: cancellationToken);
            
            await _profileCharacterChoose.LoadCharacters(cancellationToken);

            await _profileCharacterChoose.CloseObservable
                .FirstAsync(cancellationToken)
                .AsUniTask();

            _applicationUI.Undo();
        }

        protected override UniTask ExitAsync(CancellationToken cancellationToken)
        {
            _windowService.CloseAsync<IProfileCharacterChoose>(cancellationToken)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to close profile character choose", this);
            return UniTask.CompletedTask;
        }
    }
}