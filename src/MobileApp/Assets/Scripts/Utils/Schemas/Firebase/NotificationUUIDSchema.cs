using System;
using Newtonsoft.Json;

namespace Sotbify.MobileApp.Utils.Schemas.Firebase
{
    [Serializable]
    public sealed class NotificationUUIDSchema
    {
        [JsonProperty("notification_uuid")] public string Uuid = string.Empty;
    }
}