using System;
using Newtonsoft.Json;

namespace Sotbify.MobileApp.Utils.Schemas.Profile
{
    [Serializable]
    public struct ProfileSchema
    {
        [JsonProperty("player_name")] public string ProfileName;
        [JsonProperty("active_character")] public ProfileCharacterSchema? ActiveCharacter;
    }
}