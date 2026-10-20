using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.UI.Window.Presenter;

namespace Sotbify.MobileApp.BlurredBackgroundWindow
{
    public class BlurredBackgroundPresenter : WindowPresenter<IBlurredBackground, BlurredBackgroundView>
    {
        private readonly CompositeDisposable _disposables = new ();
        protected override UniTask BeforeShowAsync(CancellationToken cancellationToken)
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