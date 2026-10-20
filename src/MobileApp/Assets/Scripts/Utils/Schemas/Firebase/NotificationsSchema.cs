using System;
using Newtonsoft.Json;

namespace Sotbify.MobileApp.Utils.Schemas.Firebase
{
    [Serializable]
    public struct NotificationsSchema
    {
        [JsonProperty("title")] public string Title;
        [JsonProperty("body")] public string Body;
        [JsonProperty("data")] public NotificationsData NotificationsData;  
    }
}