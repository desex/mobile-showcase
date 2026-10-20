using System;
using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;
using R3;
using Sotbify.MobileApp.Common.OSA;
using Sotbify.MobileApp.UI.Common.Components.Filter;
using Sotbify.MobileApp.Utils.Enums;
using VContainer;

namespace Sotbify.MobileApp.HomeWindow.Filter
{
    public class FilterListView<T> : OSA<BaseParamsWithPrefab, FilterViewsHolder<T>>
        where T : Enum
    {
        private readonly Subject<FilterItem<T>> _filterChosen = new();
        private readonly Subject<T> _activatedFilter = new();

        private Func<FilterViewsHolder<T>> _factory;
        private ListDataHelper<FilterData<T>> _data;

        [Inject]
        public void Construct(Func<FilterViewsHolder<T>> factory)
        {
            _factory = factory;
        }

        public ListDataHelper<FilterData<T>> Data => _data ??= new ListDataHelper<FilterData<T>>(this, false, false);
        public Observable<FilterItem<T>> FilterChosen => _filterChosen;
        public Observable<T> ActivatedFilter => _activatedFilter;

        protected override FilterViewsHolder<T> CreateViewsHolder(int itemIndex)
        {
            var holder = _factory.Invoke();
            holder.Init(_Params.ItemPrefab, _Params.Content, itemIndex);

            holder.View.FilterChosen
                .Subscribe((_filterChosen, Filter: holder.View), (_, state) =>
                {
                    var (subject, filter) = state;
                    subject.OnNext(filter);
                })
                .AddTo(this);

            holder.View.ActivatedFilter
                .Subscribe(_activatedFilter, (type, subject) => subject.OnNext(type))
                .AddTo(this);

            return holder;
        }

        protected override void UpdateViewsHolder(FilterViewsHolder<T> holder)
        {
            var model = Data[holder.ItemIndex];
            holder.Bind(model);
            ScheduleComputeVisibilityTwinPass(true);
        }
    }
}