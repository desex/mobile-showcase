using System;
using Newtonsoft.Json;

namespace Sotbify.MobileApp.Utils.Schemas.Firebase
{
    [Serializable]
    public struct NotificationsData
    {
        [JsonProperty("notification_uuid")] public string NotificationUuid;
    }
}