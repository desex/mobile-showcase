using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace Sotbify.MobileApp.Utils.Schemas.Bonus
{
    [Serializable]
    public class BonusSchema
    {
        [JsonProperty("id")] public int Id;

        [JsonProperty("title")] public string Title;
        [JsonProperty("title_translations")] public Dictionary<string, string> TitleTranslations;
        
        [JsonProperty("code")] public string Code;
        
        [JsonProperty("description")] public string Description;
        [JsonProperty("description_translations")] public Dictionary<string, string> DescriptionTranslations;
      
        [JsonProperty("type")] public string Type;
        [JsonProperty("brand")] public string Brand;
        [JsonProperty("activation_url")] public string ActivationUrlString;
        [JsonProperty("rules_url")] public string RulesUrlString;
        [JsonProperty("color")] public string ColorHex;
        [JsonProperty("img_url")] public string ImageUrlString;
        [JsonProperty("created_at")] public string CreatedAt;
        [JsonProperty("expired_at")] public string ExpiredAt;
        [JsonProperty("lines")] public List<BonusLineSchema> Lines;
        
        public Color Color => string.IsNullOrEmpty(ColorHex) == false && ColorUtility.TryParseHtmlString(ColorHex, out var result)
            ? result
            : Color.clear;
        
        [CanBeNull]
        public Uri ImageUrl => string.IsNullOrEmpty(ImageUrlString) == false
            ? new Uri(ImageUrlString)
            : null;
        
        [CanBeNull]
        public Uri RulesUrl => string.IsNullOrEmpty(RulesUrlString) == false
            ? new Uri(RulesUrlString)
            : null;
        
        [CanBeNull]
        public Uri ActivationUrl => string.IsNullOrEmpty(ActivationUrlString) == false
            ? new Uri(ActivationUrlString)
            : null;
    }
}