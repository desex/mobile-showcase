using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Common.OSA;
using Sotbify.MobileApp.Db.NotificationWindowConfig;
using Sotbify.MobileApp.UI.Window.Presenter;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.NotificationCenterWindow
{
    public class NotificationCenterPresenter : WindowPresenter<INotificationCenter, NotificationCenterView>
    {
        private readonly ILogger _logger;
        private readonly INotificationWindowConfig _notificationWindowConfig;
        private CancellationTokenSource _cancellationTokenSource;

        private IDisposable _bindings;
        private IDisposable _listBindings;

        public NotificationCenterPresenter
        (
            ILogger logger,
            INotificationWindowConfig notificationWindowConfig
        )
        {
            _logger = logger;
            _notificationWindowConfig = notificationWindowConfig;
        }

        public override void Initialize()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override async UniTask AfterShowAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override UniTask AfterHideAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override void OnDispose()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void ClearCTS()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private async UniTaskVoid LoadNotificationsAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}