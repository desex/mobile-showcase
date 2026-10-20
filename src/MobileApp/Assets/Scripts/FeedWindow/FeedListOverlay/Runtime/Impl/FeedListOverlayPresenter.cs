using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Common.OSA;
using Sotbify.MobileApp.UI.Window.Presenter;
using Sotbify.MobileApp.Utils.Extensions;

namespace Sotbify.MobileApp.FeedWindow.FeedListOverlay
{
    public class FeedListOverlayPresenter : WindowPresenter<IFeedListOverlay, FeedListOverlayView>
    {
        private readonly CompositeDisposable _disposables = new();
        private readonly CompositeDisposable _modelSubscriptions = new();
        private readonly ILogger _logger;

        private CancellationTokenSource _cancellationTokenSource;

        public FeedListOverlayPresenter(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

        private void SubscribeModels<T>(IReadOnlyObservableList<T> models, ListDataHelper<T> targetListView)
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

        private void ClearCTS()
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