using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.Localization;
using Sotbify.MobileApp.Relic;
using Sotbify.MobileApp.Server;
using Sotbify.MobileApp.Utils.Schemas.Relics;
using UnityEngine;

namespace Sotbify.MobileApp.NewRelicWindow
{
    public sealed class NewRelicWindow : INewRelicWindow, IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        private readonly ReactiveProperty<Texture> _itemTexture = new(Texture2D.blackTexture);
        private readonly ReactiveProperty<string> _itemName = new(string.Empty);
        private readonly ReactiveProperty<string> _itemDescription = new(string.Empty);
        private readonly ReactiveProperty<int> _itemRarity = new(0);
        private readonly Subject<Unit> _closeSubject = new();

        private readonly Client _client;
        private readonly ILocalizationService _localizationService;
        private readonly Func<IRelicItem, IRelicInstance> _relicInstanceFactory;

        private RelicDescriptionSchema? _dailyRelicSchema = null;
        private IRelicInstance _relicInstance = null;

        public NewRelicWindow
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

        public ReadOnlyReactiveProperty<Texture> ItemTexture => _itemTexture;
        public ReadOnlyReactiveProperty<string> ItemName => _itemName;
        public ReadOnlyReactiveProperty<string> ItemDescription => _itemDescription;
        public ReadOnlyReactiveProperty<int> ItemRarity => _itemRarity;
        public Observable<Unit> CloseObservable => _closeSubject;

        public async UniTask<bool> CheckDailyBonusAsync(CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public async UniTask LoadInfoAsync(CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void Close() {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public UniTask ReleaseInfoAsync(CancellationToken cancellationToken = default)
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