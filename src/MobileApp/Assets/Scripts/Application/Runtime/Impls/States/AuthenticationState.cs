using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.ApplicationUI.States;
using Sotbify.MobileApp.ApplicationUI.States.Stab;
using Sotbify.MobileApp.ApplicationUIScopeFacade;
using Sotbify.MobileApp.Common.Preferences;
using Sotbify.MobileApp.PinPassword;
using Sotbify.MobileApp.Server;
using Sotbify.MobileApp.Utils;
using Sotbify.MobileApp.Utils.Data;
using UState;

namespace Sotbify.MobileApp.Application.States
{
    public class AuthenticationState : State<IApplication>
    {
        private readonly IPreferences _preferences;
        private readonly Client _client;
        private readonly IPinPasswordService _pinPasswordService;
        private readonly IApplicationUIScopeFacade _applicationUIScopeFacade;

        public AuthenticationState(
            IPreferences preferences, 
            Client client,
            IPinPasswordService pinPasswordService,
            IApplicationUIScopeFacade applicationUIScopeFacade
        )
        {
            _preferences = preferences;
            _client = client;
            _pinPasswordService = pinPasswordService;
            _applicationUIScopeFacade = applicationUIScopeFacade;
        }

        protected override async UniTask ExecuteAsync(CancellationToken cancellationToken)
        {
            var applicationUI = _applicationUIScopeFacade.ApplicationUI;
            
            var qrToken = _preferences.Has(SavePath.QRSavePath)
                ? _preferences.GetString(SavePath.QRSavePath)
                : string.Empty;

            if (qrToken != string.Empty)
            {
                var isStab = IsTokenStab(qrToken);
                var isValid = isStab || await _client.TryAuthenticate(qrToken, cancellationToken);

                if (!isValid)
                {
                    applicationUI.RequestTransition<GreetingsWindowState>();
                    return;
                }

                if (_pinPasswordService.HasPassword)
                {
                    applicationUI.RequestTransition<CheckPasswordWindowState>();
                    return;
                }

                if (isStab)
                {
                    applicationUI.RequestTransition<StabMainInitializeState>();
                    return;
                }
                
                applicationUI.RequestTransition<LoadingScreenState>();

            }
            else
                applicationUI.RequestTransition<GreetingsWindowState>();
        }
        
        private bool IsTokenStab(string token)
        {
            var stabTokens = _preferences.Get<StabQRTokens>(SavePath.StabQRs);
            
            foreach (var stabToken in stabTokens.Tokens)
            {
                if (token != stabToken)
                    continue;

                return true;
            }

            return false;
        }
    }    
}