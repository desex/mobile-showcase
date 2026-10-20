using System.Threading;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using Sotbify.MobileApp.UI.Common.Components.Filter;
using Sotbify.MobileApp.Utils.Enums;

namespace Sotbify.MobileApp.FeedWindow.FeedListOverlay
{
    public interface IFeedListOverlay
    {
        public IReadOnlyObservableList<FilterData<EHomeItemType>> Filters { get; }
        public ReadOnlyReactiveProperty<EHomeItemType> ActiveFilter { get; }

        public void FilterItems(EHomeItemType chosenType);
        public UniTask LoadFilters(CancellationToken cancellationToken = default);
        public void Clear();
    }
}