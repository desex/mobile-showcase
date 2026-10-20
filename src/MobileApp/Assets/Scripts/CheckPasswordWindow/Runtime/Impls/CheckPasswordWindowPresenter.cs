using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.PinPassword;
using Sotbify.MobileApp.UI.Window.Presenter;

namespace Sotbify.MobileApp.CheckPasswordWindow
{
    public class CheckPasswordWindowPresenter : WindowPresenter<ICheckPasswordWindow, CheckPasswordWindowView>
    {
        private readonly CompositeDisposable _disposables = new();
        private readonly IPinPasswordService _pinPasswordService;
        private bool _isPasswordComplete;

        public CheckPasswordWindowPresenter(IPinPasswordService pinPasswordService)
        {
            _pinPasswordService = pinPasswordService;
        }

        public override void Initialize()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override UniTask AfterShowAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override void OnDispose() {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private async UniTask OnPinChanged(string pin, CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}