using System;
using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;

namespace Sotbify.MobileApp.Common.OSA
{
    public abstract class BaseOsaListView<TViewsHolder> : BaseOsaListView<BaseParamsWithPrefab, TViewsHolder>
        where TViewsHolder : BaseItemViewsHolder
    {
        protected sealed override void InitializeViewHolder(TViewsHolder viewsHolder, int itemIndex)
        {
            viewsHolder.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
        }
    }
    
    public abstract class BaseOsaListView<TParams, TViewsHolder> : OSA<TParams, TViewsHolder>, IViewsHolderLifecycle<TViewsHolder>
        where TParams : BaseParams
        where TViewsHolder : BaseItemViewsHolder
    {
        public event Action<TViewsHolder> OnViewHolderCreated;
        public event Action<TViewsHolder> OnViewHolderUpdateRequested;
        public event Action<TViewsHolder> OnViewHolderDisableRequested;

        protected sealed override TViewsHolder CreateViewsHolder(int itemIndex)
        {
            var viewsHolder = CreateViewsHolder();
            InitializeViewHolder(viewsHolder, itemIndex);
            AfterInitializeViewsHolder(viewsHolder);

            OnViewHolderCreated?.Invoke(viewsHolder);
            return viewsHolder;
        }

        protected abstract void InitializeViewHolder(TViewsHolder viewsHolder, int itemIndex);

        protected sealed override void UpdateViewsHolder(TViewsHolder newOrRecycled)
        {
            OnViewHolderUpdateRequested?.Invoke(newOrRecycled);
            //ScheduleComputeVisibilityTwinPass(true);
        }

        protected sealed override void OnBeforeRecycleOrDisableViewsHolder(TViewsHolder inRecycleBinOrVisible, int newItemIndex)
        {
            OnViewHolderDisableRequested?.Invoke(inRecycleBinOrVisible);
            base.OnBeforeRecycleOrDisableViewsHolder(inRecycleBinOrVisible, newItemIndex);
        }

        protected abstract TViewsHolder CreateViewsHolder();

        protected virtual void AfterInitializeViewsHolder(TViewsHolder viewsHolder)
        {
        }
    }
}