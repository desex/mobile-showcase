using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.ChatMessages.Models;
using Sotbify.MobileApp.CloseBlackjackPopup;
using Sotbify.MobileApp.UI.Common.Providers;
using Sotbify.MobileApp.UI.Window;
using UState;

namespace Sotbify.MobileApp.ApplicationUI.States
{
    public class CloseBlackjackPopupState : State<IApplicationUI, CloseBlackjackPopupState.Payload>
    {
        public struct Payload
        {
            public BlackjackModel BlackjackModel;
        }
        
        private readonly IWindowService _windowService;
        private readonly IApplicationUI _applicationUI;
        private readonly IRootsRectProvider _rootsRectProvider;

        public CloseBlackjackPopupState(
            IWindowService windowService,
            IApplicationUI applicationUI,
            IRootsRectProvider rootsRectProvider
        )
        {
            _windowService = windowService;
            _applicationUI = applicationUI;
            _rootsRectProvider = rootsRectProvider;
        }

        protected override async UniTask ExecuteAsync(Payload payload, CancellationToken cancellationToken)
        {
            var closeBlackjackPopup = await _windowService.
                OpenAsync<ICloseBlackjackPopup>(_rootsRectProvider.NexusRoot, cancellationToken: cancellationToken);
            
            var continueTask = closeBlackjackPopup.OnContinuePressed
                .FirstAsync(cancellationToken)
                .AsUniTask();
            
            var cancelTask = closeBlackjackPopup.OnCancelPressed
                .FirstAsync(cancellationToken)
                .AsUniTask();
            
            var (winTaskIndex, _, _) = await UniTask.WhenAny(continueTask, cancelTask);
            
            _applicationUI.RequestTransition<BlackjackWindowState, BlackjackWindowState.Payload>(
                new BlackjackWindowState.Payload()
                {
                    BlackjackModel = payload.BlackjackModel,
                    ShouldContinue = winTaskIndex == 0
                });
        }

        protected override async UniTask ExitAsync(CancellationToken cancellationToken)
        {
            await _windowService.CloseAsync<ICloseBlackjackPopup>(cancellationToken);
        }
    }
}