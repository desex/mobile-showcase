using System;
using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;
using Sotbify.MobileApp.Common.OSA;
using VContainer;

namespace Sotbify.MobileApp.NotificationCenterWindow
{
    public class NotificationsListView : OSA<BaseParamsWithPrefab, NotificationViewsHolder>
    {
        private Func<NotificationViewsHolder> _notificationViewsHolderFactory;
        
        private ListDataHelper<NotificationModel> _data;

        public ListDataHelper<NotificationModel> Data => _data ??= new ListDataHelper<NotificationModel>(this);
        
        public int LastVisibleItemIndex { get; private set; }

        [Inject]
        public void Construct(Func<NotificationViewsHolder> notificationViewsHolderFactory)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override NotificationViewsHolder CreateViewsHolder(int itemIndex)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        protected override void UpdateViewsHolder(NotificationViewsHolder holder)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override void OnScrollPositionChanged(double normalizedPosition)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public override void ScrollTo(int itemIndex, float normalizedOffsetFromViewportStart = 0, float normalizedPositionOfItemPivotToUse = 0)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}