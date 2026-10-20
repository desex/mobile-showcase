using System;
using Newtonsoft.Json;

namespace Sotbify.MobileApp.Utils.Schemas.Blackjack
{
    [Serializable]
    public sealed class BlackjackChatSchema
    {
        [JsonProperty("request_id")] public int RequestId;
    }
}