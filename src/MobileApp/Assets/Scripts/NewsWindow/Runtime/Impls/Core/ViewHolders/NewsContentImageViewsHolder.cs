using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion.Animation;
using R3;
using Sotbify.MobileApp.NewsWindow.Models;
using Sotbify.MobileApp.TextureProvider;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.NewsWindow.Core.ViewHolders
{
    public sealed class NewsContentImageViewsHolder : NewsContentItemViewsHolder
    {
        private readonly ITextureProvider _textureProvider;
        private readonly Subject<NewsContentImageViewsHolder> _sizeResolved = new();
        private Image _image;
        private LitMotionAnimation _loadingAnimation;
        private CancellationTokenSource _loadCancellation;
        private NewsContentImageModel _model;

        public NewsContentImageModel Model => _model;
        public Observable<NewsContentImageViewsHolder> SizeResolved => _sizeResolved;

        public NewsContentImageViewsHolder(ITextureProvider textureProvider)
        {
            _textureProvider = textureProvider ?? throw new ArgumentNullException(nameof(textureProvider));
        }

        public override void CollectViews()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public override bool CanPresentModelType(Type modelType) {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public override async UniTask UpdateViews(INewsContentItemModel model)
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

        private void CancelLoad()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void StartLoadingAnimation()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void StopLoadingAnimation()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}
