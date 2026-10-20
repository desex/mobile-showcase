using System;
using Newtonsoft.Json;

namespace Sotbify.MobileApp.Utils.Schemas.Chat
{
    [Serializable]
    public class ChatMessageRequestSchema
    {
        [JsonProperty("text")] public string Message;
    }
}