using System;
using System.Threading;
using Com.ForbiddenByte.OSA.Core;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.BonusesWindow.Core.Models;

namespace Sotbify.MobileApp.BonusesWindow.Core.ViewHolders
{
    public abstract class BaseBonusItemsViewsHolder : BaseItemViewsHolder
    {
        public abstract bool CanPresentModelType(Type modelType);
        public abstract UniTask UpdateViews(IBonusModel model, CancellationToken cancellationToken = default);
        public abstract bool IsRecyclable { get; }
        
        public virtual void UpdateSize() { }
    }
}