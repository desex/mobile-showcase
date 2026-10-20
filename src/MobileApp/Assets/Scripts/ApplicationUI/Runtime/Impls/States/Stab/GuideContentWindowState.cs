using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.Stab.GuideContentWindow;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils;
using Sotbify.MobileApp.Utils.Extensions;
using UState;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.ApplicationUI.States.Stab
{
    public class GuideContentWindowState : State<IApplicationUI, GuideContentWindowState.Payload>
    {
        public struct Payload
        {
            public EGuide Guide;
        }
        
        private readonly IWindowService _windowService;
        private readonly IApplicationUI _applicationUI;
        private readonly ILogger _logger;

        public GuideContentWindowState(
            IWindowService windowService,
            IApplicationUI applicationUI,
            ILogger logger)
        {
            _windowService = windowService;
            _applicationUI = applicationUI;
            _logger = logger;
        }

        protected override async UniTask ExecuteAsync(Payload payload, CancellationToken cancellationToken)
        {
            var guideContent = await _windowService
                .OpenAsync<IGuideContent>(cancellationToken);
            
            guideContent.OpenContent(payload.Guide);

            await guideContent.CloseObservable
                .FirstAsync(cancellationToken)
                .AsUniTask();

            _applicationUI.RequestTransition<VRGuideWindowState>();
        }

        protected override UniTask ExitAsync(CancellationToken cancellationToken)
        {
            _windowService.CloseAsync<IGuideContent>(cancellationToken)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to close guide content window", this);
            return base.ExitAsync(cancellationToken);
        }
    }
}