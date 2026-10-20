using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Common.Preferences;
using Sotbify.MobileApp.DeletePopupWindow;
using Sotbify.MobileApp.NexusWindow;
using Sotbify.MobileApp.Stab.StabNexus;
using Sotbify.MobileApp.UI.Common.Providers;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils;
using Sotbify.MobileApp.Utils.Data;
using Sotbify.MobileApp.Utils.Extensions;
using Stab.StabChatWindow;
using Stab.StabProfileWindow;
using UState;

namespace Sotbify.MobileApp.ApplicationUI.States.Stab
{
    public class DeleteAccountState : State<IApplicationUI>
    {
        private readonly IWindowService _windowService;
        private readonly IApplicationUI _applicationUI;
        private readonly IRootsRectProvider _rootsRectProvider;
        private readonly IPreferences _preferences;
        private readonly IStabChat _stabChat;
        private readonly ILogger _logger;

        public DeleteAccountState(
            IWindowService windowService,
            IApplicationUI applicationUI,
            IRootsRectProvider rootsRectProvider,
            IPreferences preferences,
            IStabChat stabChat,
            ILogger logger
        )
        {
            _windowService = windowService;
            _applicationUI = applicationUI;
            _rootsRectProvider = rootsRectProvider;
            _preferences = preferences;
            _stabChat = stabChat;
            _logger = logger;
        }

        protected override async UniTask ExecuteAsync(CancellationToken cancellationToken)
        {
            var popup = await _windowService.OpenAsync<IDeletePopup>(_rootsRectProvider.OverlayRoot ,cancellationToken: cancellationToken);

            var deleteTask = popup.OnDeletePressed
                .FirstAsync(cancellationToken)
                .AsUniTask();

            var undoTask = popup.OnClosePressed
                .FirstAsync(cancellationToken)
                .AsUniTask();

            var (winTaskIndex, _, _) = await UniTask.WhenAny(deleteTask, undoTask);

            switch (winTaskIndex)
            {
                case 0:
                    DeleteAccountData();
                    _windowService.CloseAsync<IStabProfile>(cancellationToken)
                        .ForgetIgnoreOperationCancelledException(_logger, "Failed to close policy window", this);
                    
                    _windowService.CloseAsync<IStabNexus>(cancellationToken)
                        .ForgetIgnoreOperationCancelledException(_logger, "Failed to close dock window", this);
                    
                    _applicationUI.RequestTransition<GreetingsWindowState>();
                    return;
                case 1:
                    _applicationUI.RequestTransition<StabProfileWindowState>();
                    return;
            }
        }

        protected override UniTask ExitAsync(CancellationToken cancellationToken)
        {
            _windowService.CloseAsync<IDeletePopup>(cancellationToken)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to close delete popup", this);
            return base.ExitAsync(cancellationToken);
        }

        private void DeleteAccountData()
        {
            _stabChat.Clear();
            DeleteToken();
        }

        private void DeleteToken()
        {
            var currentToken = _preferences.GetString(SavePath.QRSavePath);
            var stabTokens = _preferences.Get<StabQRTokens>(SavePath.StabQRs);

            var newStabTokens = new List<string>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var stabToken in stabTokens.Tokens)
            {
                if (stabToken != currentToken)
                    newStabTokens.Add(stabToken);
            }
            
            _preferences.Remove(SavePath.QRSavePath);
            
            var newTokensArray = new StabQRTokens
            {
                Tokens = newStabTokens.ToArray()
            };
            
            _preferences.Set(SavePath.StabQRs, newTokensArray);
        }
    }
}