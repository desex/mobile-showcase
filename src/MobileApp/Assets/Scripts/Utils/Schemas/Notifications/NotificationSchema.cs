using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sotbify.MobileApp.Utils.Schemas.Notifications
{
    [Serializable]
    public struct NotificationSchema
    {
        [JsonProperty("id")] public int Id;
        
        [JsonProperty("title")] public string Title;
        [JsonProperty("title_translations")] public Dictionary<string, string> TitleTranslations;
        
        [JsonProperty("body")] public string Description;
        [JsonProperty("body_translations")] public Dictionary<string, string> DescriptionTranslations;
        
        [JsonProperty("category")] public string Category;
        [JsonProperty("created_at")] public string CreatedAt;
        [JsonProperty("send_at")] public string SentAt;
    }
}