using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.BlackjackWindow;
using Sotbify.MobileApp.ChatMessages;
using Sotbify.MobileApp.ChatMessages.Models;
using Sotbify.MobileApp.UI.Common.Providers;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils.Enums.Blackjack;
using UState;

namespace Sotbify.MobileApp.ApplicationUI.States
{
    public class BlackjackWindowState : State<IApplicationUI, BlackjackWindowState.Payload>
    {
        public struct Payload
        {
            public BlackjackModel BlackjackModel;
            public bool ShouldContinue;
        }
        
        private readonly IWindowService _windowService;
        private readonly IRootsRectProvider _rootsRectProvider;
        private readonly IApplicationUI _applicationUI;
        private readonly IChatMessages _chatMessages;
        private IBlackjack _blackjack;

        public BlackjackWindowState(
            IWindowService windowService,
            IRootsRectProvider rootsRectProvider,
            IApplicationUI applicationUI,
            IChatMessages chatMessages
        )
        {
            _windowService = windowService;
            _rootsRectProvider = rootsRectProvider;
            _applicationUI = applicationUI;
            _chatMessages = chatMessages;
        }

        protected override async UniTask ExecuteAsync(Payload payload, CancellationToken cancellationToken)
        {
            if (_windowService.IsOpen<IBlackjack>() == false)
                _blackjack = await _windowService.OpenAsync<IBlackjack>(_rootsRectProvider.NexusRoot,
                    cancellationToken: cancellationToken);
            
            if (payload.ShouldContinue == false)
            {
                await CloseBlackjack(payload, EGameResult.Cancelled, cancellationToken);
                return;
            }
            
            var resultTask = _blackjack.CloseAfterResult
                .FirstAsync(cancellationToken)
                .AsUniTask();

            var exitTask = _blackjack.ShowClosePopupObservable
                .FirstAsync(cancellationToken)
                .AsUniTask();
            
            var cancelledExplicitlyTask = _blackjack.GameCancelledExplicitly
                .FirstAsync(cancellationToken)
                .AsUniTask();

            var (winTaskIndex, result, _, _) = await UniTask.WhenAny(resultTask, exitTask, cancelledExplicitlyTask);

            switch (winTaskIndex)
            {
                case 0:
                    await CloseBlackjack(payload, result, cancellationToken);
                    return;
                case 1:
                    _applicationUI.RequestTransition<CloseBlackjackPopupState,CloseBlackjackPopupState.Payload>( new CloseBlackjackPopupState.Payload()
                    {
                        BlackjackModel = payload.BlackjackModel,
                    });
                    return;
                case 2:
                    await CloseBlackjack(payload, EGameResult.Cancelled, cancellationToken);
                    return;
            }
        }

        protected async UniTask CloseBlackjack(Payload payload, EGameResult result, CancellationToken cancellationToken)
        {
            _chatMessages.ChangeBlackjackResult(payload.BlackjackModel, result);
            if (result == EGameResult.Cancelled)
                await _blackjack.CancelGame(cancellationToken);
            
            _blackjack.PrepareForNextGame();
            
            await _windowService.CloseAsync<IBlackjack>(cancellationToken);
            _applicationUI.RequestTransition<ChatWindowState, ChatWindowState.Payload>(new ChatWindowState.Payload
            {
                IsFromOverlay = true,
            });
        }
    }
}