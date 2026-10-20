#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using LeTai.Asset.TranslucentImage;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Relic;
using Sotbify.MobileApp.Server;
using Sotbify.MobileApp.Services.Browser;
using Sotbify.MobileApp.Services.PartnersService;
using UnityEngine;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.HomeWindow
{
    public sealed class Home : IHome, IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly SemaphoreSlim _loadSemaphore = new(1, 1);

        private readonly List<IRelicInstance?> _itemsList = new();
        private readonly ReactiveProperty<IReadOnlyList<IRelicInstance?>> _items = new(Array.Empty<IRelicInstance>());
        private readonly Subject<Unit> _editActiveSetSubject = new();
        private readonly Subject<Unit> _casinoOpened = new();

        private readonly Client _client;
        private readonly Func<IRelicItem, IRelicInstance> _relicInstanceFactory;
        private readonly IBrowser _browser;
        private readonly IPartnersService _partnersService;
        private readonly ILogger _logger;

        public Home
        (
            Client client,
            Func<IRelicItem, IRelicInstance> relicInstanceFactory,
            TranslucentImageSource translucentImageSource,
            IBrowser browser,
            IPartnersService partnersService,
            ILogger logger
        )
        {
            TranslucentImageSource = translucentImageSource ?? throw new ArgumentNullException(nameof(translucentImageSource));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _relicInstanceFactory = relicInstanceFactory ?? throw new ArgumentNullException(nameof(relicInstanceFactory));
            _browser = browser ?? throw new ArgumentNullException(nameof(browser));
            _partnersService = partnersService ?? throw new ArgumentNullException(nameof(partnersService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public TranslucentImageSource TranslucentImageSource { get; }
        public ReadOnlyReactiveProperty<IReadOnlyList<IRelicInstance?>> Items => _items;
        public Observable<Unit> EditActiveSetObservable => _editActiveSetSubject;
        public Observable<Unit> CasinoOpenedObservable => _casinoOpened;

        public async UniTask OpenCasino(CancellationToken cancellationToken)
        {
            foreach (var partner in _partnersService.Partners.Values)
            {
                var partnerUrl = partner.Url;
                _casinoOpened.OnNext(Unit.Default);
                await _browser.OpenLink(partnerUrl, cancellationToken);
                break;
            }
        }

        public async UniTask LoadItemsAsync(CancellationToken cancellationToken = default)
        {
            await _loadSemaphore.WaitAsync(cancellationToken);
            using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);
            try
            {
                var activeSet = await _client.LoadActiveSetAsync(cancellationTokenSource.Token);
                var maxPosition = activeSet
                    .Where(static relicSchema => relicSchema.Position.HasValue)
                    .Max(static relicSchema => relicSchema.Position ?? 0);

                foreach (var relicInstance in _itemsList)
                    relicInstance?.Dispose();
                
                _itemsList.Clear();
                _itemsList.AddRange(Enumerable.Repeat<IRelicInstance?>(null, Mathf.Max(3, maxPosition)));

                foreach (var relicSchema in activeSet)
                {
                    var relicItem = new RelicItem
                    {
                        Key = relicSchema.Key,
                        ImageUri = relicSchema.ImageUrl,
                        BackgroundColor = relicSchema.MainColor
                    };

                    var index = relicSchema.Position.HasValue ? relicSchema.Position.Value - 1 : 0;
                    
                    _itemsList[index]?.Dispose();
                    _itemsList[index] = _relicInstanceFactory.Invoke(relicItem);
                }

                _items.Value = _itemsList;
                _items.ForceNotify();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.Error("Failed to load items", e, this);
            }
            finally
            {
                _loadSemaphore.Release();
            }
        }

        public void EditActiveSet() =>
            _editActiveSetSubject.OnNext(Unit.Default);

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();

            _items.Dispose(true);

            foreach (var relicInstance in _itemsList)
                relicInstance?.Dispose();

            _itemsList.Clear();

            _editActiveSetSubject.Dispose(true);
        }
    }
}