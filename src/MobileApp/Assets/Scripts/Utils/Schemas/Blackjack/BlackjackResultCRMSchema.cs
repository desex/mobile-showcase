using Newtonsoft.Json;

namespace Sotbify.MobileApp.Utils.Schemas.Blackjack
{
    public struct BlackjackResultCRMSchema
    {
        [JsonProperty("request_id")] public int RequestId;
        [JsonProperty("result")] public string Result;
    }
}