using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using Sotbify.MobileApp.Server;
using Sotbify.MobileApp.Services.ServerConstConverter;
using Sotbify.MobileApp.UI.Common.Components.Filter;
using Sotbify.MobileApp.Utils.Enums;

namespace Sotbify.MobileApp.FeedWindow.FeedListOverlay
{
    public class FeedListOverlay : IFeedListOverlay
    {
        private readonly ObservableList<FilterData<EHomeItemType>> _filters = new();
        
        private readonly Client _client;
        private readonly IServerConstConverter _serverConstConverter;

        private readonly ReactiveProperty<EHomeItemType> _activeFilter = new ();

        public FeedListOverlay
        (
            Client client,
            IServerConstConverter serverConstConverter
        )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _serverConstConverter = serverConstConverter ?? throw new ArgumentNullException(nameof(serverConstConverter));
        }

        public IReadOnlyObservableList<FilterData<EHomeItemType>> Filters => _filters;
        public ReadOnlyReactiveProperty<EHomeItemType> ActiveFilter => _activeFilter;
        
        public void FilterItems(EHomeItemType chosenType)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public async UniTask LoadFilters(CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        private FilterData<EHomeItemType> ConvertToData(KeyValuePair<string, int> filter)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void Clear()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}
