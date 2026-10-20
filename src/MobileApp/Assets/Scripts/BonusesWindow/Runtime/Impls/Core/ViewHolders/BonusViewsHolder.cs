using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.BonusesWindow.BonusItem;
using Sotbify.MobileApp.BonusesWindow.Core.Models;
using Sotbify.MobileApp.BonusesWindow.Core.Views;
using Sotbify.MobileApp.Localization;
using Sotbify.MobileApp.Services.PartnersService;
using Sotbify.MobileApp.Services.ServerConstConverter;
using Sotbify.MobileApp.TextureProvider;
using Sotbify.MobileApp.Utils.Data;
using Sotbify.MobileApp.Utils.Enums;
using Sotbify.MobileApp.Utils.Schemas;
using UnityEngine;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;
using Object = UnityEngine.Object;

namespace Sotbify.MobileApp.BonusesWindow.Core.ViewHolders
{
    public class BonusViewsHolder : BaseBonusItemsViewsHolder
    {
        private readonly List<BonusItemLine> _lineViews = new();
        private readonly Subject<Unit> _open = new();
        private readonly Subject<Unit> _close = new();
        
        private readonly ITextureProvider _textureProvider;
        private readonly IPartnersService _partnersService;
        private readonly IStringLocalizer _stringLocalizer;
        private readonly ILogger _logger;
        
        private BonusModel _model;
        private CancellationTokenSource _imageLoadCts;

        public BonusViewsHolder
        (
            ITextureProvider textureProvider,
            IPartnersService partnersService,
            IStringLocalizer stringLocalizer,
            ILogger logger
        )
        {
            _textureProvider = textureProvider ?? throw new ArgumentNullException(nameof(textureProvider));
            _partnersService = partnersService ?? throw new ArgumentNullException(nameof(partnersService));
            _stringLocalizer = stringLocalizer ?? throw new ArgumentNullException(nameof(stringLocalizer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public BonusView View { get; private set; }

        public string Brand => _model.Brand;
        public string Code => _model.Code;
        public Uri RulesUri => _model.RulesUri;
        public Uri ActivationUri => _model.ActivationUri;

        public Observable<Unit> Open => _open;
        public Observable<Unit> Close => _close;

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

        public override async UniTask UpdateViews(IBonusModel model, CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public override bool IsRecyclable => _model.IsOpened == false;

        public void ChangeOpenState()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void UpdateLines(IReadOnlyList<BonusLine> lines)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private BonusItemLine GetOrCreateDescriptionView(int index)
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