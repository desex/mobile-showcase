using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbifay.MobileApp.Assets;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.ItemRenderer;
using Sotbify.MobileApp.Relic;
using Sotbify.MobileApp.UI.Window.Presenter;
using UnityEngine;
using UnityEngine.Pool;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.HomeWindow
{
    public class HomePresenter : WindowPresenter<IHome, HomeView>
    {
        private const string LogoAssetKey = "Logo";

        private readonly IAssetsProvider _assetsProvider;
        private readonly IObjectPool<IItemRenderer> _itemRendererPool;
        private readonly ILogger _logger;

        private IDisposable _bindings;
        private IItemRenderer _itemRenderer;
        private GameObject _logoInstance;

        public HomePresenter(IAssetsProvider assetsProvider, IObjectPool<IItemRenderer> itemRendererPool, ILogger logger)
        {
            _assetsProvider = assetsProvider ?? throw new ArgumentNullException(nameof(assetsProvider));
            _itemRendererPool = itemRendererPool ?? throw new ArgumentNullException(nameof(itemRendererPool));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override void Initialize()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override async UniTask BeforeShowAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override UniTask AfterHideAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override void OnDispose()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void ReleaseLogoPreview()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}