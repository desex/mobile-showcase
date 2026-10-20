using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sotbify.MobileApp.Utils.Schemas.Feed
{
    [Serializable]
    public struct FeedContentSchema
    {
        [JsonProperty("title")] public string Title;
        [JsonProperty("title_translations")] public Dictionary<string, string> TitleTranslations;
       
        [JsonProperty("image_url")] public string ImageUrl;
        [JsonProperty("sections")] public FeedContentSectionSchema[] FeedContentSchemas;
        [JsonProperty("next_id")] public int? NextId;
        [JsonProperty("prev_id")] public int? PreviousId;
    }
}