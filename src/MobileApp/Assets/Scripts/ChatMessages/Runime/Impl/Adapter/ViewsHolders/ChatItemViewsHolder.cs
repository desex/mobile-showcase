using System;
using System.Threading;
using Com.ForbiddenByte.OSA.Core;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.ChatMessages.Models;

namespace Sotbify.MobileApp.ChatMessages.ViewsHolders
{
    public abstract class ChatItemViewsHolder : BaseItemViewsHolder
    {
        public abstract bool CanPresentModelType(Type modelType);
        public abstract UniTask UpdateViews(IChatItemModel model, CancellationToken cancellationToken = default);
        
        public virtual void UpdateSize() { }
    }
}