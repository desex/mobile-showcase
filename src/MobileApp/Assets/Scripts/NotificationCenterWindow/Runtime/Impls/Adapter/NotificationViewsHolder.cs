using System;
using Com.ForbiddenByte.OSA.Core;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Db.NotificationWindowConfig;
using UnityEngine.UI;

namespace Sotbify.MobileApp.NotificationCenterWindow
{
    public class NotificationViewsHolder : BaseItemViewsHolder
    {
        private readonly INotificationWindowConfig _config;
        private readonly ILogger _logger;
        
        private NotificationView _view;
        
        public NotificationViewsHolder(INotificationWindowConfig config, ILogger logger)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override void CollectViews()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void Bind(NotificationModel model)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public override void MarkForRebuild()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public override void UnmarkForRebuild()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}