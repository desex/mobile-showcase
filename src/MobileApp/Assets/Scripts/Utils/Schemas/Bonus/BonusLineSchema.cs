using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sotbify.MobileApp.Utils.Schemas
{
    [Serializable]
    public class BonusLineSchema
    {
        [JsonProperty("name")] public string Name;
        [JsonProperty("name_translations")] public Dictionary<string, string> NameTranslations;
        
        [JsonProperty("value")] public string Value;
        [JsonProperty("value_translations")] public Dictionary<string, string> ValueTranslations;
    }
}