using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sotbify.MobileApp.Utils.Schemas.Feed
{
    [Serializable]
    public class FeedButtonSchema
    {
        [JsonProperty("title")] public string Title;
        [JsonProperty("title_translations")] public Dictionary<string, string> TitleTranslations;
        
        [JsonProperty("url")] public string Url;
    }
}