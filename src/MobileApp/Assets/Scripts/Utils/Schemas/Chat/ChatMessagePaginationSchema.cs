using System;
using Newtonsoft.Json;

namespace Sotbify.MobileApp.Utils.Schemas.Chat
{
    public class ChatMessagePaginationSchema
    {
        [JsonProperty("items")] public ChatMessageResponseSchema[] Messages = Array.Empty<ChatMessageResponseSchema>();
        [JsonProperty("next_cursor")] public int? NextCursor;
    }
}