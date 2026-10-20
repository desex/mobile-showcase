using System;
using Newtonsoft.Json;

namespace Sotbify.MobileApp.Utils.Schemas.Notifications
{
    [Serializable]
    public struct NotificationRequestSchema
    {
        [JsonProperty("last_notification_id")] public int LastNotificationId;
    }
}