using System.Threading;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.UI.Window.Presenter;
using Sotbify.MobileApp.Utils.Extensions;

namespace Sotbify.MobileApp.PlayWindow.Core
{
    public class GameWindowPresenter : WindowPresenter<IGame, GameWindowView>
    {
        private readonly CompositeDisposable _disposables = new ();
        private readonly ILogger _logger;

        public GameWindowPresenter(ILogger logger)
        {
            _logger = logger;
        }
        
        protected override UniTask AfterShowAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        private async UniTask LoadPartners(CancellationToken cancellationToken)
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