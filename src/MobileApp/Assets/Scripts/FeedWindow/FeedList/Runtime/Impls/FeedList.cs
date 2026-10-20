using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using Sotbify.MobileApp.FeedWindow.FeedList.Models;
using Sotbify.MobileApp.Services.Browser;
using Sotbify.MobileApp.Utils.Enums;
using UnityEngine;
using UnityEngine.Pool;

namespace Sotbify.MobileApp.FeedWindow.FeedList
{
    public class FeedList : IFeedList, IDisposable
    {
        private readonly Subject<Unit> _scrollToTop = new();
        private readonly Subject<int> _onOpenFeedContent = new();
        private readonly Subject<int> _newsReadMarked = new();
        private readonly Subject<Unit> _openNotificationCenter = new();
        
        private readonly ObservableList<IFeedModel> _homeModels = new();
        private readonly List<IFeedModel> _loadedItems = new();
        
        private readonly IFeedItemLoader _feedItemLoader;
        private readonly IBrowser _browser;
        
        private int _generation;
        private EHomeItemType _activeFilter;

        public FeedList(IFeedItemLoader feedItemLoader, IBrowser browser)
        {
            _feedItemLoader = feedItemLoader ?? throw new ArgumentNullException(nameof(feedItemLoader));
            _browser = browser ?? throw new ArgumentNullException(nameof(browser));
        }

        public bool IsAllHomeItemsLoaded => _feedItemLoader.IsAllLoaded;

        public IReadOnlyObservableList<IFeedModel> HomeModels => _homeModels;
        public Observable<Unit> ScrollToTopObservable => _scrollToTop;
        public Observable<int> NewsReadMarked => _newsReadMarked;
        public Observable<int> OpenContentObservable => _onOpenFeedContent;

        public bool IsNewsRead(int id)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void MarkNewsRead(int id)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void OpenFeedContent(int id)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public async UniTask LoadMoreHomeItems(CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void OpenNotificationCenter()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void Clear()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        public UniTask OpenBrowser(string url, CancellationToken cancellationToken = default)
            {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void CopyPromocode(string promocode)
            {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void FilterItems(EHomeItemType chosenType)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void Dispose()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void ClearHomeItems()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}
