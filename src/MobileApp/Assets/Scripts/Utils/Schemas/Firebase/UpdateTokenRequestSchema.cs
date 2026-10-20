using System;
using Newtonsoft.Json;

namespace Sotbify.MobileApp.Utils.Schemas.Firebase
{
    [Serializable]
    public sealed class UpdateTokenRequestSchema
    {
        [JsonProperty("firebase_token")] 
        public string Token = string.Empty;
    }
}