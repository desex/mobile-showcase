using System;
using Com.ForbiddenByte.OSA.Core;

namespace Sotbify.MobileApp.Common.OSA
{
    public interface IViewsHolderLifecycle<out TViewsHolder>
        where TViewsHolder : BaseItemViewsHolder
    {
        event Action<TViewsHolder> OnViewHolderCreated;
        event Action<TViewsHolder> OnViewHolderUpdateRequested;
        event Action<TViewsHolder> OnViewHolderDisableRequested;
    }
}