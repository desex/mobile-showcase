using System;
using Newtonsoft.Json;

namespace Sotbify.MobileApp.Utils.Schemas.Chat
{
    [Serializable]
    public struct UnreadCountSchema
    {
        [JsonProperty("unread_count")] public int Count;
    }
}