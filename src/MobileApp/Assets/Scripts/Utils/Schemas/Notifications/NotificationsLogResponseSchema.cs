using System;
using Newtonsoft.Json;

namespace Sotbify.MobileApp.Utils.Schemas.Notifications
{
    [Serializable]
    public struct NotificationsLogResponseSchema
    {
        [JsonProperty("next_cursor")] public int? NextCursor;
        [JsonProperty("unread_count")] public int UnreadCount;
        [JsonProperty("items")] public NotificationSchema[] NotificationSchemas;
    }
}