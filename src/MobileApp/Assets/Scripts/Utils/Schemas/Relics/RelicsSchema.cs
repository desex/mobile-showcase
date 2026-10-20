using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sotbify.MobileApp.Utils.Schemas.Relics
{
    public struct RelicsSchema
    {
        [JsonProperty("artifacts")] public List<RelicSchema> Relics;
        [JsonProperty("total_artifacts_count")] public int TotalRelicsCount;
        
        public int RelicsCount => Relics?.Count ?? 0;
    }
}