using Cysharp.Threading.Tasks;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.UI.Window.Presenter;

namespace Sotbify.MobileApp.GlassesWindow
{
    public class GlassesPresenter : WindowPresenter<IGlassesWindow, GlassesView>
    {
        private readonly CompositeDisposable _disposables = new ();
        private readonly ILogger _logger;
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