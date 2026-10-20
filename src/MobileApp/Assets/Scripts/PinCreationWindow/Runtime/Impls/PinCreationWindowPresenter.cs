using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.PinPassword;
using Sotbify.MobileApp.UI.Window.Presenter;

namespace Sotbify.MobileApp.PinCreationWindow
{
    public class PinCreationWindowPresenter : WindowPresenter<IPinCreationWindow, PinCreationWindowView>
    {
        private readonly CompositeDisposable _disposables = new();
        private readonly IPinPasswordService _pinPasswordService;

        public PinCreationWindowPresenter(IPinPasswordService pinPasswordService)
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
    }
}
