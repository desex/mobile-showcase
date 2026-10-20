using System;
using Newtonsoft.Json;

namespace Sotbify.MobileApp.Utils.Schemas.Relics
{
    [Serializable]
    public class EquipRelicRequest
    {
        [JsonProperty("artifact_id")] public int ArtifactId;
        [JsonProperty("position")] public int Position;
    }
}