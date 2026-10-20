using Newtonsoft.Json;

namespace Sotbify.MobileApp.Utils.Schemas
{
    public sealed class AnalyticsEventSchema
    {
        [JsonProperty("action")] public string Action = string.Empty;
        [JsonProperty("notification_uuid")] public string NotificationUUID = string.Empty;
    }
}