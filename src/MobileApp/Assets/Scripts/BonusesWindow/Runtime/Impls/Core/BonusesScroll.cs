using System;
using System.IO;
using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;
using Cysharp.Threading.Tasks;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Analytics;
using Sotbify.MobileApp.BonusesWindow.Core.Models;
using Sotbify.MobileApp.BonusesWindow.Core.ViewHolders;
using Sotbify.MobileApp.Common.OSA;
using Sotbify.MobileApp.Utils.Extensions;
using VContainer;

namespace Sotbify.MobileApp.BonusesWindow.Core
{
    public class BonusesScroll : OSA<BaseParamsWithPrefab, BaseBonusItemsViewsHolder>
    {
        private readonly Subject<Uri> _openLink = new ();
        private readonly Subject<string> _copyBonus = new ();
        private Func<BonusViewsHolder> _bonusViewsHolderFactory;
        private ILogger _logger;
        
        private ListDataHelper<IBonusModel> _data;
        private AnalyticsEvents _analyticsEvents;


        public Observable<string> CopyBonus => _copyBonus;
        public Observable<Uri> OpenLink => _openLink;
        
        public ListDataHelper<IBonusModel> Data => _data ??= new ListDataHelper<IBonusModel>(this, false, false);
        
        public int LastVisibleItemIndex { get; private set; }
        
        [Inject]
        public void Construct(
            ILogger logger,
            Func<BonusViewsHolder> bonusViewsHolderFactory,
            AnalyticsEvents analyticsEvent
        )
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override BaseBonusItemsViewsHolder CreateViewsHolder(int itemIndex)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private BaseBonusItemsViewsHolder HandleBonusModel(int itemIndex)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override void UpdateViewsHolder(BaseBonusItemsViewsHolder holder)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        protected override float UpdateItemSizeOnTwinPass(BaseBonusItemsViewsHolder viewsHolder)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override bool IsRecyclable(BaseBonusItemsViewsHolder potentiallyRecyclable, int indexOfItemThatWillBecomeVisible, double sizeOfItemThatWillBecomeVisible)
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
        
        protected override void OnScrollPositionChanged(double normalizedPosition)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        protected override void Dispose()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        private float GetItemSize(Type modelType)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        private void RequestChangeItemSize(BaseBonusItemsViewsHolder holder, float size)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}