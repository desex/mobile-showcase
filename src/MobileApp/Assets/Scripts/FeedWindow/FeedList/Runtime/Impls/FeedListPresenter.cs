using System.Threading;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using Sotbify.MobileApp.Common.OSA;
using Sotbify.MobileApp.FeedWindow.FeedList.Core.ViewHolders;
using Sotbify.MobileApp.FeedWindow.FeedList.Models;
using Sotbify.MobileApp.FeedWindow.FeedListOverlay;
using Sotbify.MobileApp.UI.Window.Presenter;
using Sotbify.MobileApp.Utils.Extensions;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.FeedWindow.FeedList
{
    public class FeedListPresenter : WindowPresenter<IFeedList, FeedListView>
    {
        private readonly CompositeDisposable _disposables = new();
        private readonly CompositeDisposable _modelSubscriptions = new();
        private readonly ILogger _logger;
        private readonly IFeedListOverlay _feedListOverlay;
        private CancellationTokenSource _cancellationTokenSource;

        public FeedListPresenter(
            ILogger logger,
            IFeedListOverlay feedListOverlay
        )
        {
            _logger = logger;
            _feedListOverlay = feedListOverlay;
        }

        public override void Initialize()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void RefreshVisiblePostStatus(int id)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override UniTask AfterShowAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void SubscribeModels<T>(IReadOnlyObservableList<T> models, ListDataHelper<T> targetListView)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        private async UniTask LoadFeedAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override UniTask BeforeHideAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        protected override UniTask AfterHideAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override void OnDispose()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        private void ClearCTS()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}
