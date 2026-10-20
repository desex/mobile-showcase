using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.Services.Browser;

namespace MainPolicyWindow
{
    public class MainPolicy : IMainPolicy
    {
        private readonly Subject<Unit> _closeButtonPressed = new();
        private readonly IBrowser _browser;

        public MainPolicy(IBrowser browser)
        {
            _browser = browser;
        }

        public Observable<Unit> CloseButtonPressed => _closeButtonPressed;
        
        public UniTask OpenUrl(string url, CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void Close()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}