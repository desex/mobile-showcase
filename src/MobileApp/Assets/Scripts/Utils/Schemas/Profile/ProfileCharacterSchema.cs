using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace Sotbify.MobileApp.Utils.Schemas.Profile
{
    [Serializable]
    public struct ProfileCharacterSchema
    {
        [JsonProperty("id")] public int Id;

        [JsonProperty("title")] public string Name;
        [JsonProperty("title_translations")] public Dictionary<string, string> NameTranslations;

        [JsonProperty("description")] public string Description;
        [JsonProperty("description_translations")] public Dictionary<string, string> DescriptionTranslations;

        [JsonProperty("image_url")] public string ImageUrlString;
        [JsonProperty("color")] public string ColorHex;
        [JsonProperty("is_active")] public bool IsActive;

        public Color Color => string.IsNullOrEmpty(ColorHex) == false
                              && ColorUtility.TryParseHtmlString(ColorHex, out var result)
            ? result
            : Color.clear;
        
        [CanBeNull]
        public Uri ImageUrl => string.IsNullOrEmpty(ImageUrlString) == false
            ? new Uri(ImageUrlString)
            : null;
    }
}