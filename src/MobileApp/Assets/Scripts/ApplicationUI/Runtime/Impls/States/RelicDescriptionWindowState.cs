using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.NexusWindow;
using Sotbify.MobileApp.RelicsDescriptionWindow;
using Sotbify.MobileApp.UI.Common.Providers;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils.Extensions;
using UState;

namespace Sotbify.MobileApp.ApplicationUI.States
{
    public class RelicDescriptionWindowState : State<IApplicationUI, RelicDescriptionWindowState.Payload>
    {
        public struct Payload
        {
            public int RelicId;
        }

        private readonly IWindowService _windowService;
        private readonly IRootsRectProvider _rootsRectProvider;
        private readonly IRelicDescription _relicDescription;
        private readonly IApplicationUI _applicationUI;
        private readonly ILogger _logger;

        public RelicDescriptionWindowState(
            IWindowService windowService,
            IRootsRectProvider rootsRectProvider,
            IRelicDescription relicDescription,
            ILogger logger,
            IApplicationUI applicationUI
        )
        {
            _windowService = windowService;
            _rootsRectProvider = rootsRectProvider;
            _relicDescription = relicDescription;
            _logger = logger;
            _applicationUI = applicationUI;
        }

        protected override async UniTask ExecuteAsync(Payload payload, CancellationToken cancellationToken)
        {
            await _relicDescription.LoadDescription(payload.RelicId, cancellationToken);

            await _windowService.OpenAsync<IRelicDescription>
            (
                _rootsRectProvider.NexusRoot,
                cancellationToken: cancellationToken
            );
            
            await _relicDescription.CloseObservable
                .AnyAsync(cancellationToken)
                .AsUniTask();

            _applicationUI.Undo();
        }

        protected override UniTask ExitAsync(CancellationToken cancellationToken)
        {
            _windowService.CloseAsync<IRelicDescription>(cancellationToken)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to close relic description", this);

            return UniTask.CompletedTask;
        }
    }
}