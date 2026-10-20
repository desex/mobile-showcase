using System;
using System.Collections.Generic;
using System.Linq;
﻿using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbifay.MobileApp.Assets;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Db.RelicInstancesConfig;
using Sotbify.MobileApp.ItemRenderer;
using Sotbify.MobileApp.TextureProvider;
using Sotbify.MobileApp.UI.Window.Presenter;
using UnityEngine;
using UnityEngine.Pool;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.RelicActiveSetWindow
{
    public sealed class RelicActiveSetPresenter : WindowPresenter<IRelicActiveSet, RelicActiveSetView>
    {
        private readonly IRelicInstancesConfig _relicInstancesConfig;
        private readonly IAssetsProvider _assetsProvider;
        private readonly IObjectPool<IItemRenderer> _itemRendererPool;
        private readonly ITextureProvider _textureProvider;
        private readonly ILogger _logger;
        private readonly List<RelicPreview> _previews = new();

        private IDisposable _bindings;
        private int _selectedRelicPosition;

        public RelicActiveSetPresenter
        (
            IRelicInstancesConfig relicInstancesConfig,
            IAssetsProvider assetsProvider,
            IObjectPool<IItemRenderer> itemRendererPool,
            ITextureProvider textureProvider,
            ILogger logger
        )
        {
            _relicInstancesConfig = relicInstancesConfig ?? throw new ArgumentNullException(nameof(relicInstancesConfig));
            _assetsProvider = assetsProvider ?? throw new ArgumentNullException(nameof(assetsProvider));
            _itemRendererPool = itemRendererPool ?? throw new ArgumentNullException(nameof(itemRendererPool));
            _textureProvider = textureProvider ?? throw new ArgumentNullException(nameof(textureProvider));
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

        private UniTask EditRelicAsync(int id, int index, CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private async UniTask RandomizeActiveSetAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private async UniTask UpdateActiveSet(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private async UniTask LoadPreviewsAsync(IReadOnlyList<RelicLineModel> lineModels, CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private async UniTask LoadPreviewAsync(RelicLineModel line, RelicSetLine view, CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private static Gradient SetGradientColor(Gradient gradient, Color color)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private async UniTask LoadPreviewWithItemRendererAsync(RelicInstanceInfo relicInfo, RelicSetLine view, CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private async UniTask LoadPreviewWithTextureProviderAsync(Uri imageUrl, RelicSetLine view, CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void ReleasePreviews()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void ReleasePreview(IItemRenderer renderer, GameObject instance)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private readonly struct RelicPreview
        {
            public readonly RelicSetLine Line;
            public readonly IItemRenderer Renderer;
            public readonly GameObject Instance;

            public RelicPreview(RelicSetLine line, IItemRenderer renderer, GameObject instance)
            {
                Line = line;
                Renderer = renderer;
                Instance = instance;
            }
        }
    }
}