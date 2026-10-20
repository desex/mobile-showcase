using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.TextureProvider;
using Sotbify.MobileApp.UI.Window.Presenter;
using Sotbify.MobileApp.Utils.Data;

namespace Sotbify.MobileApp.Chat
{
    public sealed class ChatPresenter : WindowPresenter<IChat, ChatView>
    {
        private readonly ILogger _logger;
        private readonly ITextureProvider _textureProvider;

        private IDisposable _bindings;

        public ChatPresenter(ITextureProvider textureProvider, ILogger logger)
        {
            _textureProvider = textureProvider ?? throw new ArgumentNullException(nameof(textureProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override void Initialize()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override void OnDispose()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private async UniTask FillManager(ManagerData managerData, CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}