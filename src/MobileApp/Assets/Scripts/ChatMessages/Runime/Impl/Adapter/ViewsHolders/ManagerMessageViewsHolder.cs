using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.ChatMessages.Models;
using Sotbify.MobileApp.ChatMessages.Views;
using UnityEngine;

namespace Sotbify.MobileApp.ChatMessages.ViewsHolders
{
    public class ManagerMessageViewsHolder : ChatItemViewsHolder
    {
        private ManagerMessageView _managerMessageView;
        
        private ManagerMessageModel _model;

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

        public override void UpdateSize()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}