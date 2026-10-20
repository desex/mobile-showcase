using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace Sotbify.MobileApp.Utils.Schemas
{
    public struct PartnersSchema
    {
        [JsonProperty("id")] public int Id;
        [JsonProperty("name")] public string Name;
        [JsonProperty("url")] public string UrlString;
        [JsonProperty("logo_url")] public string LogoUrlString;
        [JsonProperty("color")] public string ColorHex;

        [CanBeNull]
        public Uri Url => string.IsNullOrEmpty(UrlString) == false
            ? new Uri(UrlString)
            : null;
        
        [CanBeNull]
        public Uri LogoUrl => string.IsNullOrEmpty(LogoUrlString) == false
            ? new Uri(LogoUrlString)
            : null;

        public Color Color => string.IsNullOrEmpty(ColorHex) == false && ColorUtility.TryParseHtmlString(ColorHex, out var result)
            ? result
            : Color.white;
    }
}