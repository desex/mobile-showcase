using System;
using System.Collections.Generic;
using Com.ForbiddenByte.OSA.Core;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Common.OSA;
using Sotbify.MobileApp.NewsWindow.Core.ViewHolders;
using Sotbify.MobileApp.NewsWindow.Models;
using Sotbify.MobileApp.TextureProvider;
using Sotbify.MobileApp.Utils.Extensions;
using Mathf = UnityEngine.Mathf;
using VContainer;

namespace Sotbify.MobileApp.NewsWindow.Core
{
    public class NewsContentListView : OSA<NewsContentParams, NewsContentItemViewsHolder>
    {
        private const float EndThreshold = 0.95f;

        private ListDataHelper<INewsContentItemModel> _data;
        private ITextureProvider _textureProvider;
        private ILogger _logger;
        private readonly Dictionary<int, NewsContentImageModel> _pendingImageSizeChanges = new();
        private readonly ReactiveProperty<float> _readingProgress = new(0f);
        private ReadOnlyReactiveProperty<bool> _scrolledToEnd;

        public ListDataHelper<INewsContentItemModel> Data => _data ??= new ListDataHelper<INewsContentItemModel>(this, false, false);
        public ReadOnlyReactiveProperty<float> ReadingProgress => _readingProgress;

        // Debounce гасит транзиентные флапы bool при смене контента (Clear → AddRange → SetNormalizedPosition
        // происходят в одном кадре) — наружу уходит только устаканившееся значение
        public ReadOnlyReactiveProperty<bool> ScrolledToEnd => _scrolledToEnd ??= _readingProgress
            .Select(progress => progress >= EndThreshold)
            .Debounce(TimeSpan.FromMilliseconds(100))
            .ToReadOnlyReactiveProperty(false);

        protected override void OnScrollPositionChanged(double normalizedPosition)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override void OnItemsRefreshed(int prevCount, int newCount)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void UpdateReadingProgress(double normalizedPosition)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override void Dispose()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        [Inject]
        public void Construct(
            ITextureProvider textureProvider,
            ILogger logger
        )
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override NewsContentItemViewsHolder CreateViewsHolder(int itemIndex)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override void UpdateViewsHolder(NewsContentItemViewsHolder holder)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override float UpdateItemSizeOnTwinPass(NewsContentItemViewsHolder holder)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override void LateUpdate()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override bool IsRecyclable(NewsContentItemViewsHolder holder, int itemIndex, double itemSize)
            {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override void CollectItemsSizes(ItemCountChangeMode changeMode, int count, int indexIfInsertingOrRemoving, ItemsDescriptor itemsDescriptor)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void OnImageSizeResolved(NewsContentImageViewsHolder holder)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}
