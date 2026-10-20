using Sotbify.MobileApp.HomeWindow.Filter;
using Sotbify.MobileApp.UI.Common.Components.Filter;
using Sotbify.MobileApp.UI.Window.View;
using Sotbify.MobileApp.Utils.Enums;
using UnityEngine;

namespace Sotbify.MobileApp.FeedWindow.FeedListOverlay
{
    public class FeedListOverlayView : FadeWindowView
    {
        [field: SerializeField] public FilterListView<EHomeItemType> Filters { get; private set; }

        private FilterItem<EHomeItemType> _activeFilterItem;

        public void SetActivateFilter(FilterItem<EHomeItemType> filterItem)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}