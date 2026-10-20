using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.FeedWindow.FeedList.Core.Views;
using Sotbify.MobileApp.FeedWindow.FeedList.Models;
using Sotbify.MobileApp.Localization;
using Sotbify.MobileApp.TextureProvider;
using Sotbify.MobileApp.Utils.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.FeedWindow.FeedList.Core.ViewHolders
{
    public class FeedBonusesViewHolder : FeedItemViewHolder
    {
        private readonly ITextureProvider _textureProvider;
        private readonly IStringLocalizer _stringLocalizer;
        
        private FeedBonusModel _model;
        private CancellationTokenSource _imageLoadCts;

        public FeedBonusesViewHolder(ITextureProvider textureProvider, IStringLocalizer stringLocalizer)
        {
            _textureProvider = textureProvider ?? throw new ArgumentNullException(nameof(textureProvider));
            _stringLocalizer = stringLocalizer ?? throw new ArgumentNullException(nameof(stringLocalizer));
        }
        
        public FeedBonusView View { get; private set; }

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

        public override async UniTask BindAsync(IFeedModel model, CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        public override void UpdateSize() 
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public override void OnBeforeRecycleOrDisable(int newItemIndex)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public override void OnBeforeDestroy()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void CancelImageLoad()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}
