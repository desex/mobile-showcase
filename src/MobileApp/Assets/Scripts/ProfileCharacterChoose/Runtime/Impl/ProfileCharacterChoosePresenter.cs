using Sotbify.MobileApp.UI.Window.Presenter;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.TextureProvider;
using UnityEngine;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.ProfileCharacterChoose
{
    public class ProfileCharacterChoosePresenter : WindowPresenter<IProfileCharacterChoose, ProfileCharacterChooseView>
    {
        private readonly ITextureProvider _textureProvider;
        private readonly ILogger _logger;
        private readonly CompositeDisposable _disposables = new();
        private CancellationTokenSource _imageCancellation;
        private Sprite _sprite;
        private bool _visible;

        public ProfileCharacterChoosePresenter(ITextureProvider textureProvider, ILogger logger)
        {
            _textureProvider = textureProvider;
            _logger = logger;
        }

        public override void Initialize()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override UniTask BeforeShowAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void ShowCharacter(ProfileCharacterData character)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private async UniTask LoadImageAsync(Uri uri, CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override UniTask BeforeHideAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override void OnDispose()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void CancelImageLoad()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void ReleaseSprite()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}
