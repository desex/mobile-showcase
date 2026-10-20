using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Localization;
using Sotbify.MobileApp.Server;
using Sotbify.MobileApp.Services.ServerConstConverter;
using Sotbify.MobileApp.Utils.Data;
using Sotbify.MobileApp.Utils.Schemas.Notifications;

namespace Sotbify.MobileApp.NotificationCenterWindow
{
    public class NotificationCenter : INotificationCenter, IDisposable
    {
        private readonly Subject<Unit> _close = new ();
        private readonly Subject<Unit> _scrollTop = new ();
        private readonly ObservableList<NotificationModel> _notifications = new();
        private readonly ReactiveProperty<bool> _shouldShowLoading = new (true);
        private readonly ReactiveProperty<int> _unreadCount = new ();
        
        private readonly Client _client;
        private readonly ILocalizationService _localizationService;
        private readonly IServerConstConverter _serverConstConverter;
        private readonly ILogger _logger;

        private int? _nextCursor;
        private bool _isLoadedInitial;

        public NotificationCenter
        (
            Client client,
            ILocalizationService localizationService,
            IServerConstConverter serverConstConverter,
            ILogger logger
        )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
            _serverConstConverter = serverConstConverter ?? throw new ArgumentNullException(nameof(serverConstConverter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public bool IsAllLoaded { get; private set; }

        public ReadOnlyReactiveProperty<bool> ShouldShowLoading => _shouldShowLoading;
        public ReadOnlyReactiveProperty<int> UnreadCount => _unreadCount;
        public Observable<Unit> CloseObservable => _close;
        public Observable<Unit> ScrollToTop => _scrollTop;
        public IReadOnlyObservableList<NotificationModel> Notifications => _notifications;

        public async UniTask LoadMoreAsync(CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void Close()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private async UniTask LoadMoreAsyncInternal(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private List<NotificationModel> ConvertToModels(NotificationsLogResponseSchema schema)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private NotificationModel ConvertToModel(NotificationSchema schema, string localeCode)
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