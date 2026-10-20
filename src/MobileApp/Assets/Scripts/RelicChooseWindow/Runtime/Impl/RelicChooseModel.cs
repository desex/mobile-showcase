using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using ObservableCollections;
using Sotbify.MobileApp.Localization;
using Sotbify.MobileApp.Relic;
using Sotbify.MobileApp.RelicChooseWindow.Adapter;
using Sotbify.MobileApp.RelicsDescriptionWindow;
using Sotbify.MobileApp.Server;
using Sotbify.MobileApp.Utils.Extensions;
using Sotbify.MobileApp.Utils.Schemas.Relics;

namespace Sotbify.MobileApp.RelicChooseWindow
{
    public sealed class RelicChooseModel : IRelicChooseModel, IDisposable
    {
        private readonly HashSet<int> _equippedRelicsList = new();
        private readonly ReactiveProperty<IReadOnlyCollection<int>> _equippedRelics = new(Array.Empty<int>());
        private readonly ReactiveProperty<RelicDescriptionModel> _relicDescription = new();
        private readonly ReactiveProperty<IRelicInstance> _relicInstance = new(null);
        private readonly ObservableList<RelicModel> _relics = new();
        private readonly Subject<Unit> _closeRelicChoseSubject = new();
        private readonly Subject<int> _equipRelicSubject = new();

        private readonly Client _client;
        private readonly ILocalizationService _localizationService;

        public RelicChooseModel(Client client, ILocalizationService localizationService)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
        }

        public ReadOnlyReactiveProperty<IReadOnlyCollection<int>> EquippedRelics => _equippedRelics;
        public ReadOnlyReactiveProperty<RelicDescriptionModel> RelicDescription => _relicDescription;
        public ReadOnlyReactiveProperty<IRelicInstance> RelicInstance => _relicInstance;
        public IReadOnlyObservableList<RelicModel> Relics => _relics;
        public Observable<Unit> CloseRelicChoseObservable => _closeRelicChoseSubject;
        public Observable<int> EquipRelicObservable => _equipRelicSubject;

        public void SetEquippedRelics(IEnumerable<int> equippedRelicIds)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void Equip() {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public async UniTask LoadRelics(CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void Reset()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public async UniTask OpenDescriptionAsync(int id, [CanBeNull] IRelicInstance relicInstance, CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void Close() {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private static RelicModel ConvertToModel(RelicSchema schema, string localeCode)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private static RelicDescriptionModel ConvertToModel(RelicDescriptionSchema schema, string localeCode)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void Dispose()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}