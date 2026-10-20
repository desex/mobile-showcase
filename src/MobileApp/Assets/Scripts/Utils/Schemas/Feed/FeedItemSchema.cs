using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Sotbify.MobileApp.Utils.Schemas.Bonus;
using UnityEngine;

namespace Sotbify.MobileApp.Utils.Schemas.Feed
{
    [Serializable]
    public class FeedItemSchema
    {
        [JsonProperty("id")] public int Id;
        
        [JsonProperty("title")] public string Title;
        [JsonProperty("title_translations")] public Dictionary<string, string> TitleTranslations;
        
        [JsonProperty("description")] public string Description;
        [JsonProperty("description_translations")] public Dictionary<string, string> DescriptionTranslations;
       
        [JsonProperty("image_url")] [CanBeNull] public string ImageUrlString;
        [JsonProperty("created_at")] public string CreatedAt;
        [JsonProperty("type")] public string Type;
        [JsonProperty("color")] public string ColorHex;
        [JsonProperty("is_read")] public bool IsRead;
        [JsonProperty("promo_code")] public BonusSchema PromoCode;
        [JsonProperty("button")] public FeedButtonSchema FeedButton;
        
        public Color? Color => string.IsNullOrEmpty(ColorHex) == false 
                              && ColorUtility.TryParseHtmlString(ColorHex, out var result)
            ? result
            : null;
        
        [CanBeNull]
        public Uri ImageUrl => string.IsNullOrEmpty(ImageUrlString) == false
            ? new Uri(ImageUrlString)
            : null;
    }
}