using System;
using Newtonsoft.Json;

namespace Sotbify.MobileApp.Utils.Schemas
{
    [Serializable]
    public class SignInSchema
    {
        [JsonProperty("access_token")] public string AccessToken;
        [JsonProperty("token_type")] public string TokenType;
        [JsonProperty("expire_at")] public string ExpireAt;
        [JsonProperty("player_name")] public string PlayerName;
        [JsonProperty("partner")] public PartnersSchema Partner;
    }
}