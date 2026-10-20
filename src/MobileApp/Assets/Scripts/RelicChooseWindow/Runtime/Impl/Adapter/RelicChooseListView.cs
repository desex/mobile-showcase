using System;
using System.Collections.Generic;
using System.Threading;
using Com.ForbiddenByte.OSA.CustomAdapters.GridView;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using R3;
using Sotbify.MobileApp.Common.OSA;
using Sotbify.MobileApp.Relic;
using Sotbify.MobileApp.Utils.Extensions;
using VContainer;

namespace Sotbify.MobileApp.RelicChooseWindow.Adapter
{
    public class RelicChooseListView : GridAdapter<GridParams, RelicViewsHolder>
    {
        private readonly HashSet<int> _equippedRelicIds = new();
        private readonly Subject<(int, IRelicInstance)> _openDescription = new();
        
        private Func<IRelicItem, IRelicInstance> _relicInstanceFactory;

        private ListDataHelper<RelicModel> _data;
        private int? _selectedRelicId;
        private RelicChooseView _selectedView;

        [Inject]
        public void Constructor(Func<IRelicItem, IRelicInstance> relicInstanceFactory)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public Observable<(int, IRelicInstance)> OpenDescriptionObservable => _openDescription;

        public ListDataHelper<RelicModel> Data => _data ??= new ListDataHelper<RelicModel>(this, false, false);

        public void SetEquippedRelics([CanBeNull] IEnumerable<int> equippedRelicIds)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public async UniTask DeselectCurrentAsync(CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override void UpdateCellViewsHolder(RelicViewsHolder viewsHolder)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override void OnCellViewsHolderCreated(RelicViewsHolder cellVH, CellGroupViewsHolder<RelicViewsHolder> cellGroup)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override void OnBeforeRecycleOrDisableCellViewsHolder(RelicViewsHolder viewsHolder, int newItemIndex)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private async UniTask SelectViewAsync
        (
            RelicChooseView selectedView,
            int relicId,
            CancellationToken cancellationToken
        )
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        private void UpdateIsEquipped(RelicViewsHolder holder)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}