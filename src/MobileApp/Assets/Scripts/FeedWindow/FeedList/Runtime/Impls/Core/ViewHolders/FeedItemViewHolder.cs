using System;
using System.Threading;
using Com.ForbiddenByte.OSA.Core;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.FeedWindow.FeedList.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.FeedWindow.FeedList.Core.ViewHolders
{
    public abstract class FeedItemViewHolder : BaseItemViewsHolder
    {
        private readonly Subject<FeedItemViewHolder> _sizeChanged = new();
        public Observable<FeedItemViewHolder> SizeChanged => _sizeChanged;

        public override void CollectViews()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected void NotifySizeChanged() {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public override void OnBeforeDestroy()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        public abstract bool CanPresentModelType(Type modelType);
        public abstract UniTask BindAsync(IFeedModel model, CancellationToken cancellationToken = default);
        
        public virtual void UpdateSize()
        {
        }
    }
}
