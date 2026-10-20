using System;
using Newtonsoft.Json;

namespace Sotbify.MobileApp.Utils.Schemas.Chat
{
    [Serializable]
    public class ManagerSchema
    {
        [JsonProperty("id")] public int Id;
        [JsonProperty("username")] public string Username;
        [JsonProperty("fullname")] public string Fullname;
        [JsonProperty("thumbnail_url")] public string ThumbnailUrl;
        [JsonProperty("online_time")] public string OnlineTime;
        [JsonProperty("is_online")] public bool IsOnline;
    }
}