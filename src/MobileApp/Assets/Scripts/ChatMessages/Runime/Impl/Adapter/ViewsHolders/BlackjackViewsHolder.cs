using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.ChatMessages.Models;
using Sotbify.MobileApp.ChatMessages.Views;
using Sotbify.MobileApp.Utils.Enums.Blackjack;

namespace Sotbify.MobileApp.ChatMessages.ViewsHolders
{
    public class BlackjackViewsHolder : ChatItemViewsHolder
    {
        public BlackjackView BlackjackView { get; private set; }
        
        private BlackjackModel _model;

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

        public void ApplyResultVisual(EGameResult gameResult)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}