using System;
using Newtonsoft.Json;

namespace Sotbify.MobileApp.Utils.Schemas
{
    [Serializable]
    public class NewsContentSchema
    {
        [JsonProperty("id")] public int Id;
        [JsonProperty("title")] public string Title;
        [JsonProperty("description")] public string Description;
        [JsonProperty("image_url")] public string ImageUrl;
        [JsonProperty("created_at")] public string CreatedAt;
        [JsonProperty("text")] public string Content;
        [JsonProperty("type")] public string Type;
        [JsonProperty("promo_code")] public string PromoCode;
    }
}