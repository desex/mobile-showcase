using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.ItemRenderer;
using Sotbify.MobileApp.Localization;
using Sotbify.MobileApp.Relic;
using Sotbify.MobileApp.Server;
using Sotbify.MobileApp.Utils.Schemas.Relics;
using UnityEngine;

namespace Sotbify.MobileApp.RelicsDescriptionWindow
{
    public class RelicDescription : IRelicDescription, IDisposable
    {
        private readonly ReactiveProperty<RelicDescriptionModel> _relicDescription = new();
        private readonly ReactiveProperty<Texture> _texture = new();
        private readonly Subject<Unit> _closeSubject = new();

        private readonly Client _client;
        private readonly ILocalizationService _localizationService;
        private readonly Func<IRelicItem, IRelicInstance> _relicInstanceFactory;

        private IItemRenderer _itemRenderer;
        private GameObject _artifactInstance;

        private IRelicInstance _relicInstance;

        public RelicDescription
        (
            Client client,
            ILocalizationService localizationService,
            Func<IRelicItem, IRelicInstance> relicInstanceFactory
        )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
            _relicInstanceFactory = relicInstanceFactory ?? throw new ArgumentNullException(nameof(relicInstanceFactory));
        }

        public ReadOnlyReactiveProperty<RelicDescriptionModel> RelicDescriptionProperty => _relicDescription;
        public ReadOnlyReactiveProperty<Texture> TextureProperty => _texture;
        public Observable<Unit> CloseObservable => _closeSubject;

        public async UniTask LoadDescription(int relicId, CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void Close()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void Dispose()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private static RelicDescriptionModel ConvertToModel(RelicDescriptionSchema schema, string localeCode)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}