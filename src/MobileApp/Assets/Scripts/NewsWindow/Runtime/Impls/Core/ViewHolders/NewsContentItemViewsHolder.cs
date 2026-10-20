using System;
using Com.ForbiddenByte.OSA.Core;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.NewsWindow.Models;

namespace Sotbify.MobileApp.NewsWindow.Core.ViewHolders
{
    public abstract class NewsContentItemViewsHolder : BaseItemViewsHolder
    {
        public abstract bool CanPresentModelType(Type modelType);
        public abstract UniTask UpdateViews(INewsContentItemModel model);
        public virtual void UpdateSize() { }
    }
}