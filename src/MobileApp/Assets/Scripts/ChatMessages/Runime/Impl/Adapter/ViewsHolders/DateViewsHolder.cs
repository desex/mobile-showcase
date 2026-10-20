using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.ChatMessages.Models;
using Sotbify.MobileApp.ChatMessages.Views;

namespace Sotbify.MobileApp.ChatMessages.ViewsHolders
{
    public class DateViewsHolder : ChatItemViewsHolder
    {
        public DateView DateView { get; private set; }
        
        private DateModel _model;
        
        public override void CollectViews()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        public override bool CanPresentModelType(Type modelType)
            {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public override UniTask UpdateViews(IChatItemModel model, CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}