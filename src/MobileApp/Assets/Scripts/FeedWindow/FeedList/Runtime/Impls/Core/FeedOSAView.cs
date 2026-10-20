using System;
using System.Collections.Generic;
using System.Threading;
using Com.ForbiddenByte.OSA.Core;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.Common.OSA;
using Sotbify.MobileApp.FeedWindow.FeedList.Core.ViewHolders;
using Sotbify.MobileApp.FeedWindow.FeedList.Models;
using Sotbify.MobileApp.Utils.Extensions;
using VContainer;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.FeedWindow.FeedList.Core
{
    public class FeedOSAView : OSA<FeedParams, FeedItemViewHolder>
    {
        private readonly Subject<int> _openFeedContentSubject = new ();
        private readonly Subject<string> _openHref = new ();
        private readonly Subject<string> _copyBonus = new ();
        private readonly HashSet<FeedItemViewHolder> _pendingSizeChanges = new();
        private readonly CancellationTokenSource _sizeChangeCancellation = new();
        private bool _sizeChangeScheduled;
        
        private Func<FeedBonusesViewHolder> _bonusesViewHolderFactory;
        private Func<PostViewHolder> _postViewHolderFactory;
        private ILogger _logger;

        private ListDataHelper<IFeedModel> _data;
        
        public ListDataHelper<IFeedModel> Data => _data ??= new ListDataHelper<IFeedModel>(this, false, true);
        
        public Observable<int> OpenFeedContentObservable => _openFeedContentSubject;
        public Observable<string> OpenHref => _openHref;
        public Observable<string> CopyBonusObservable => _copyBonus;
        public int LastVisibleItemIndex => VisibleItemsCount == 0
            ? -1
            : GetItemViewsHolder(VisibleItemsCount - 1).ItemIndex;

        [Inject]
        public void Construct(
            ILogger logger,
            Func<PostViewHolder> postViewHolderFactory,
            Func<FeedBonusesViewHolder> bonusesViewHolderFactory
        )
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override FeedItemViewHolder CreateViewsHolder(int itemIndex)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override void UpdateViewsHolder(FeedItemViewHolder holder)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override float UpdateItemSizeOnTwinPass(FeedItemViewHolder viewsHolder)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override bool IsRecyclable(FeedItemViewHolder potentiallyRecyclable, int indexOfItemThatWillBecomeVisible, double sizeOfItemThatWillBecomeVisible)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override void CollectItemsSizes(
            ItemCountChangeMode changeMode, 
            int count, 
            int indexIfInsertingOrRemoving, 
            ItemsDescriptor itemsDescriptor)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        private void OnHolderSizeChanged(FeedItemViewHolder holder)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private async UniTask ApplyPendingSizeChangesAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override void Dispose()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        private float GetItemSize(IFeedModel model)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}
