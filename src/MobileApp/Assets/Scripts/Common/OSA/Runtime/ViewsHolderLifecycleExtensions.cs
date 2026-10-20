using System;
using Com.ForbiddenByte.OSA.Core;
using R3;

namespace Sotbify.MobileApp.Common.OSA
{
    public static class ViewsHolderLifecycleExtensions
    {
        public static Observable<TViewsHolder> HolderCreatedObservable<TViewsHolder>(this IViewsHolderLifecycle<TViewsHolder> lifecycle) 
            where TViewsHolder : BaseItemViewsHolder
        {
            if (lifecycle is null)
                throw new ArgumentNullException(nameof(lifecycle));

            return Observable.FromEvent<TViewsHolder>
            (
                h => lifecycle.OnViewHolderCreated += h,
                h => lifecycle.OnViewHolderCreated -= h
            );
        }
        
        public static Observable<TViewsHolder> HolderUpdateRequestedObservable<TViewsHolder>(this IViewsHolderLifecycle<TViewsHolder> lifecycle) 
            where TViewsHolder : BaseItemViewsHolder
        {
            if (lifecycle is null)
                throw new ArgumentNullException(nameof(lifecycle));

            return Observable.FromEvent<TViewsHolder>
            (
                h => lifecycle.OnViewHolderUpdateRequested += h,
                h => lifecycle.OnViewHolderUpdateRequested -= h
            );
        }
        
        public static Observable<TViewsHolder> HolderDisableRequestedObservable<TViewsHolder>(this IViewsHolderLifecycle<TViewsHolder> lifecycle) 
            where TViewsHolder : BaseItemViewsHolder
        {
            if (lifecycle is null)
                throw new ArgumentNullException(nameof(lifecycle));

            return Observable.FromEvent<TViewsHolder>
            (
                h => lifecycle.OnViewHolderDisableRequested += h,
                h => lifecycle.OnViewHolderDisableRequested -= h
            );
        }
    }
}