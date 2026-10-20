using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sotbify.MobileApp.Utils.Schemas.Feed
{
    [Serializable]
    public struct FeedContentSectionSchema
    {
        [JsonProperty("position")] public int Position;
        [JsonProperty("type")] public string Type;
        
        [JsonProperty("value")] public string Value;
        [JsonProperty("value_translations")] public Dictionary<string, string> ValueTranslations;
    }
}