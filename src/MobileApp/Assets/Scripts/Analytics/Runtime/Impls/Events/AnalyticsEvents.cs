using System;
using Sotbifay.Analytics;
using UnityEngine.Pool;

namespace Sotbify.MobileApp.Analytics
{
    public sealed partial class AnalyticsEvents
    {
        private readonly IObjectPool<AnalyticsEventBuilder<AnalyticsDestination>> _eventBuilderPool;
        private string _notificationUUID = null;
        
        public AnalyticsDestination Destination { get; set; } = AnalyticsDestination.AppMetrica;

        public string NotificationUUID
        {
            get => _notificationUUID ?? string.Empty;
            set => _notificationUUID = value;
        }

        public AnalyticsEvents(IObjectPool<AnalyticsEventBuilder<AnalyticsDestination>> eventBuilderPool)
        {
            _eventBuilderPool = eventBuilderPool ?? throw new ArgumentNullException(nameof(eventBuilderPool));
        }

        private PooledObject<AnalyticsEventBuilder<AnalyticsDestination>> GetBuilder(out AnalyticsEventBuilder<AnalyticsDestination> builder) =>
            _eventBuilderPool.Get(out builder);
    }
}