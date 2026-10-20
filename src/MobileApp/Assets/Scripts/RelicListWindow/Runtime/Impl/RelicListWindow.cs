using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.Localization;
using Sotbify.MobileApp.Server;
using Sotbify.MobileApp.Utils.Schemas.Relics;
using UnityEngine.Pool;

namespace Sotbify.MobileApp.RelicListWindow
{
    public class RelicListWindow : IRelicListWindow, IDisposable
    {
        private readonly List<RelicModel> _relicsList = new();
        private readonly ReactiveProperty<IReadOnlyList<RelicModel>> _relics = new(Array.Empty<RelicModel>());
        private readonly ReactiveProperty<int> _totalRelicsCount = new(0);
        private readonly Subject<int> _openDescriptionSubject = new();

        private readonly Client _client;
        private readonly ILocalizationService _localizationService;

        public RelicListWindow (
            Client client,
            ILocalizationService localizationService
        )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
        }

        public ReadOnlyReactiveProperty<IReadOnlyList<RelicModel>> Relics => _relics;
        public ReadOnlyReactiveProperty<int> TotalRelicsCount => _totalRelicsCount;
        public Observable<int> OpenDescriptionObservable => _openDescriptionSubject;

        public async UniTask LoadRelics(CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void OpenDescription(int modelId) {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void Dispose()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private static RelicModel ConvertToModel(RelicSchema schema, string localeCode)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private static bool HasDifferences(IReadOnlyList<RelicSchema> relics, IReadOnlyList<RelicModel> relicModels)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}