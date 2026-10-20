using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.ChatMessages.Models;
using Sotbify.MobileApp.ChatMessages.Views;
using UnityEngine;

namespace Sotbify.MobileApp.ChatMessages.ViewsHolders
{
    public class UserMessageViewsHolder : ChatItemViewsHolder
    {
        private UserMessageView _userMessageView;
        
        private UserMessageModel _model;

        public override void CollectViews()
        {
            _userMessageView = root.GetComponent<UserMessageView>();
        }

        public override bool CanPresentModelType(Type modelType)
            => typeof(UserMessageModel) == modelType;

        public override UniTask UpdateViews(IChatItemModel model, CancellationToken cancellationToken = default)
        {
            _model = model as UserMessageModel ?? throw new ArgumentNullException(nameof(model));

            _userMessageView.Message = _model.Message;
            
            return UniTask.CompletedTask;
        }

        public override void UpdateSize()
        {
            if (_model == null)
                return;

            if (_model.CachedSize == Vector2.zero)
                _model.CachedSize = _userMessageView.ApplyPreferredSize();
            else
                _userMessageView.ApplyPreferredSize(_model.CachedSize);
            
            
            root.anchorMin = new Vector2(1, 0);
            root.anchorMax = new Vector2(1, 0);
            root.pivot = new Vector2(1, 0);
        }
    }
}