using R3;
using Sotbify.MobileApp.Services.RemoteConfig;
using Sotbify.MobileApp.UI.Window.Presenter;
using Sotbify.MobileApp.Utils;

namespace Stab.StabProfileWindow
{
    public class StabProfilePresenter : WindowPresenter<IStabProfile, StabProfileView>
    {
        private readonly CompositeDisposable _disposable = new();
        private readonly IRemoteConfigService _remoteConfigService;
        
        private string PolicyHref => _remoteConfigService.GetString("PolicyHref");
        private string TermsHref => _remoteConfigService.GetString("TermsHref");

        public StabProfilePresenter(IRemoteConfigService remoteConfigService)
        {
            _remoteConfigService = remoteConfigService;
        }

        public override void Initialize()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override void OnDispose()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}