using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using Sotbify.MobileApp.BonusesWindow.Core.Models;
using Sotbify.MobileApp.DockWindowSwitcher;
using Sotbify.MobileApp.NexusWindow;
using Sotbify.MobileApp.Server;
using Sotbify.MobileApp.Services.Browser;
using Sotbify.MobileApp.Services.PartnersService;
using Sotbify.MobileApp.Services.ServerConstConverter;
using Sotbify.MobileApp.UI.Common.Components.Filter;
using Sotbify.MobileApp.Utils.Enums;
using Sotbify.MobileApp.Utils.Enums.Nexus;
using UnityEngine;
using UnityEngine.Pool;

namespace Sotbify.MobileApp.BonusesWindow
{
    public class BonusesWindow : IBonusesWindow, IDisposable
    {
        private readonly ObservableList<IBonusModel> _promocodes = new();
        private readonly ObservableList<FilterData<EBonusType>> _filters = new();
        
        private readonly IBonusItemLoader _bonusItemLoader;
        private readonly ReactiveProperty<bool> _isNoBonusActive = new();

        private readonly Client _client;
        private readonly IServerConstConverter _serverConstConverter;
        private readonly INexus _nexus;
        private readonly IBrowser _browser;
        
        private readonly List<IBonusModel> _itemsOutFilter = new();

        public BonusesWindow(
            IBonusItemLoader bonusItemLoader,
            Client client,
            IServerConstConverter serverConstConverter,
            INexus nexus,
            IBrowser browser
        )
        {
            _bonusItemLoader = bonusItemLoader;
            _client = client;
            _serverConstConverter = serverConstConverter;
            _nexus = nexus;
            _browser = browser;
        }

        public IReadOnlyObservableList<IBonusModel> Promocodes => _promocodes;
        public IReadOnlyObservableList<FilterData<EBonusType>> Filters => _filters;
        
        public ReadOnlyReactiveProperty<bool> IsNoBonusActive => _isNoBonusActive;

        public async UniTask LoadBonusesAsync(CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        public async UniTask LoadFiltersAsync(CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void Clear()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        public void Copy(string promocode)
            {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void FilterItems(EBonusType chosenType)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public UniTask OpenUrlAsync(string url, CancellationToken cancellationToken = default)
            {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void OpenChat()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void Dispose()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        private FilterData<EBonusType> ConvertToData(string type, int count)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}