using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.FeedWindow.FeedList;
using Sotbify.MobileApp.Localization;
using Sotbify.MobileApp.NewsWindow.Models;
using Sotbify.MobileApp.Server;
using Sotbify.MobileApp.Services.ServerConstConverter;
using Sotbify.MobileApp.TextureProvider;
using Sotbify.MobileApp.Utils.Enums.FeedContent;
using Sotbify.MobileApp.Utils.Extensions;
using Sotbify.MobileApp.Utils.Schemas.Feed;
using UnityEngine;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.NewsWindow
{
    public class NewsContent : INewsContent
    {
        private readonly ObservableList<INewsContentItemModel> _items = new();
        private readonly Subject<string> _title = new();
        private readonly Subject<Sprite> _image = new();
        private readonly Subject<Unit> _startLoadingTitleImage = new();
        private readonly Subject<Unit> _closeWindow = new();
        private readonly Subject<Unit> _stopLoadingTitleImage = new();
        private readonly Subject<IReadOnlyCollection<INewsContentItemModel>> _itemsLoaded = new();
        private readonly ReactiveProperty<bool> _canLoadNext = new(false);
        private readonly ReactiveProperty<bool> _canLoadPrevious = new(false);
        private readonly ReactiveProperty<bool> _isLoading = new(false);
        private readonly HashSet<int> _viewedToEnd = new();
        
        private readonly Client _client;
        private readonly IFeedList _feedList;
        private readonly ILocalizationService _localizationService;
        private readonly IServerConstConverter _serverConstConverter;
        private readonly ITextureProvider _textureProvider;
        private readonly ILogger _logger;

        private CancellationTokenSource _loadCancellation;
        private int? _nextId;
        private int? _previousId;
        private int? _currentId;

        public NewsContent
        (
            Client client,
            IServerConstConverter serverConstConverter,
            ITextureProvider textureProvider,
            IFeedList feedList,
            ILocalizationService localizationService,
            ILogger logger
        )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _serverConstConverter = serverConstConverter ?? throw new ArgumentNullException(nameof(serverConstConverter));
            _textureProvider = textureProvider ?? throw new ArgumentNullException(nameof(textureProvider));
            _feedList = feedList ?? throw new ArgumentNullException(nameof(feedList));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IReadOnlyObservableList<INewsContentItemModel> Items => _items;
        public Observable<IReadOnlyCollection<INewsContentItemModel>> ItemsLoaded => _itemsLoaded;
        public Observable<string> Title => _title;
        public Observable<Sprite> Image => _image;
        public Observable<Unit> StartLoadingTitleImage => _startLoadingTitleImage;
        public Observable<Unit> StopLoadingTitleImage => _stopLoadingTitleImage;
        public Observable<Unit> CloseObservable => _closeWindow;
        public ReadOnlyReactiveProperty<bool> CanLoadNext => _canLoadNext;
        public ReadOnlyReactiveProperty<bool> CanLoadPrevious => _canLoadPrevious;
        public ReadOnlyReactiveProperty<bool> IsLoading => _isLoading;

        public UniTask LoadNext(CancellationToken cancellationToken = default)
            {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public UniTask LoadPrevious(CancellationToken cancellationToken = default)
            {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void MarkViewedToEnd()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void CloseContent()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private async UniTask LoadAdjacentContent(int? id, CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public async UniTask LoadContent(int id, CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private async UniTask LoadMainInfo(FeedContentSchema feedContentSchema, CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void Clear()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void CancelLoad()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void UpdateNavigationAvailability()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private INewsContentItemModel ConvertToData(FeedContentSectionSchema section, string localeCode)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}
