using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.Stab.VRGuideWindow;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils.Extensions;
using UState;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.ApplicationUI.States.Stab
{
    public class VRGuideWindowState : State<IApplicationUI>
    {
        private readonly IWindowService _windowService;
        private readonly IApplicationUI _applicationUI;
        private readonly ILogger _logger;

        public VRGuideWindowState(
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
            var vrGuide = await _windowService.
                OpenAsync<IVRGuide>(cancellationToken);
            
            var closeTask = vrGuide.CloseObservable
                .FirstAsync(cancellationToken)
                .AsUniTask();
            
            var openGuideContentTask = vrGuide.OpenGuideObservable
                .FirstAsync(cancellationToken)
                .AsUniTask();
            
            var (winTask, _, guide) = await UniTask.WhenAny(closeTask, openGuideContentTask);

            switch (winTask)
            {
                case 0:
                    _windowService.CloseAsync<IVRGuide>(cancellationToken)
                        .ForgetIgnoreOperationCancelledException(_logger, "Failed to close vr guide window", this);
                    _applicationUI.RequestTransition<StabHomeState>();
                    break;
                
                case 1:
                    _applicationUI.RequestTransition<GuideContentWindowState, GuideContentWindowState.Payload>(
                        new GuideContentWindowState.Payload()
                        {
                            Guide = guide
                        });
                    break;
            }
        }
    }
}