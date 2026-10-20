using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace Sotbify.MobileApp.Utils.Schemas.Relics
{
    [Serializable]
    public struct RelicSchema
    {
        [JsonProperty("id")] public int Id;
        [JsonProperty("name")] public string Name;
        [JsonProperty("name_translations")] public Dictionary<string, string> NameTranslations;
        [JsonProperty("key")] public string Key;
        [JsonProperty("rarity")] public int Rarity;
        [JsonProperty("color")] public string ColorHex;
        [JsonProperty("color_from")] public string RadialGradientFromHex;
        [JsonProperty("color_to")] public string RadialGradientToHex;
        [JsonProperty("img_url")] [CanBeNull] public string ImageUrlString;
        [JsonProperty("position")] public int? Position;
        
        public Color MainColor => string.IsNullOrEmpty(ColorHex) == false
                                  && ColorUtility.TryParseHtmlString(ColorHex, out var result)
            ? result
            : Color.clear;

        public Color GradientFrom => string.IsNullOrEmpty(RadialGradientFromHex) == false
                                           && ColorUtility.TryParseHtmlString(RadialGradientFromHex, out var result)
            ? result
            : Color.clear;

        public Color GradientTo => string.IsNullOrEmpty(RadialGradientToHex) == false
                                         && ColorUtility.TryParseHtmlString(RadialGradientToHex, out var result)
            ? result
            : Color.clear;

        [CanBeNull]
        public Uri ImageUrl => string.IsNullOrEmpty(ImageUrlString) == false
            ? new Uri(ImageUrlString)
            : null;
    }
}