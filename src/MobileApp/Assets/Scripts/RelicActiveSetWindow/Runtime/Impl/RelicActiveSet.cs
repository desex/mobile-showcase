using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Localization;
using Sotbify.MobileApp.RelicChooseWindow;
using Sotbify.MobileApp.Server;
using Sotbify.MobileApp.UI.Common.Providers;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils.Schemas.Relics;
using UnityEngine;
using UnityEngine.Pool;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.RelicActiveSetWindow
{
    public class RelicActiveSet : IRelicActiveSet
    {
        private readonly IWindowService _windowService;
        private readonly ILocalizationService _localizationService;
        private readonly IRootsRectProvider _rootsRectProvider;
        private readonly List<RelicLineModel> _relicLines = new();
        private readonly Subject<Unit> _updateActiveSet = new();
        private readonly ReactiveProperty<bool> _isEnoughToRandomize = new();
        private readonly Client _client;

        private IRelicChooseModel _relicChoose;

        public RelicActiveSet
        (
            IWindowService windowService,
            ILocalizationService localizationService,
            IRootsRectProvider rootsRectProvider,
            Client client,
            IRelicChooseModel relicChoose
        )
        {
            _windowService = windowService;
            _localizationService = localizationService;
            _rootsRectProvider = rootsRectProvider;
            _client = client;
            _relicChoose = relicChoose;
        }

        public Observable<Unit> CloseRelicChoseObservable => _relicChoose.CloseRelicChoseObservable;
        public Observable<int> EquipRelicObservable => _relicChoose.EquipRelicObservable;
        public ReadOnlyReactiveProperty<bool> IsEnoughToRandomize => _isEnoughToRandomize;
        public IReadOnlyList<RelicLineModel> RelicLines => _relicLines;
        public Observable<Unit> UpdateActiveSet => _updateActiveSet;

        public async UniTask LoadActiveSetAsync(CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public async UniTask OpenRelicChooseAsync(int id, CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public async UniTask CloseRelicChooseAsync(CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public async UniTask EquipRelicAsync(int relicId, int selectedPosition, CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public async UniTask RandomizeActiveSet(CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private static RelicLineModel ConvertToModel(RelicSchema schema, string localeCode)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}