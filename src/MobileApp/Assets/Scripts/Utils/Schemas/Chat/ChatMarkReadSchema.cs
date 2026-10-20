using Newtonsoft.Json;

namespace Sotbify.MobileApp.Utils.Schemas.Chat
{
    public class ChatMarkReadSchema
    {
        [JsonProperty("last_msg_id")] public int LastMessageId;
    }
}