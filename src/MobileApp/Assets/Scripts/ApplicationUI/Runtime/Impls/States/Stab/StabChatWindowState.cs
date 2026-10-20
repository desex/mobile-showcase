using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Stab.StabNexus;
using Sotbify.MobileApp.UI.Common.Providers;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils.Extensions;
using Stab.StabChatWindow;
using UState;

namespace Sotbify.MobileApp.ApplicationUI.States.Stab
{
    public class StabChatWindowState : State<IApplicationUI>
    {
        private readonly ILogger _logger;
        private readonly IWindowService _windowService;
        private readonly IRootsRectProvider _rootsRectProvider;

        public StabChatWindowState(
            ILogger logger,
            IWindowService windowService,
            IRootsRectProvider rootsRectProvider
        )
        {
            _logger = logger;
            _windowService = windowService;
            _rootsRectProvider = rootsRectProvider;
        }


        protected override async UniTask ExecuteAsync(CancellationToken cancellationToken)
        {
            if (_windowService.IsOpen<IStabNexus>() == false)
                _windowService.OpenAsync<IStabNexus>(_rootsRectProvider.NexusRoot, cancellationToken: cancellationToken)
                    .ForgetIgnoreOperationCancelledException(_logger, "Failed to open stab dock", this);
            
            await _windowService.OpenAsync<IStabChat>(cancellationToken);
        }

        protected override UniTask ExitAsync(CancellationToken cancellationToken)
        {
            _windowService.CloseAsync<IStabChat>(cancellationToken)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to close stab chat window", this);
            return base.ExitAsync(cancellationToken);
        }
    }
}