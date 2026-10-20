using System;
using Newtonsoft.Json;

namespace Sotbify.MobileApp.Utils.Schemas.Blackjack
{
    [Serializable]
    public sealed class BlackjackRequestSchema
    {
        [JsonProperty("result")] public string Result;
    }
}