using System;
using Com.ForbiddenByte.OSA.CustomAdapters.GridView;
using R3;
using Sotbify.MobileApp.Common.OSA;
using Sotbify.MobileApp.Relic;
using VContainer;

namespace Sotbify.MobileApp.RelicListWindow.Adapter
{
    public class RelicsListView : GridAdapter<GridParams, RelicsViewsHolder>
    {
        private readonly Subject<int> _openDescription = new();
       
        private Func<IRelicItem, IRelicInstance> _relicInstanceFactory;

        private ListDataHelper<RelicModel> _data;

        [Inject]
        public void Construct(Func<IRelicItem, IRelicInstance> relicInstanceFactory)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        public Observable<int> OpenDescriptionObservable => _openDescription;
        
        public ListDataHelper<RelicModel> Data => _data ??= new ListDataHelper<RelicModel>(this, false, false);

        protected override void UpdateCellViewsHolder(RelicsViewsHolder viewsHolder)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override void OnCellViewsHolderCreated(RelicsViewsHolder cellVH, CellGroupViewsHolder<RelicsViewsHolder> cellGroup)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}