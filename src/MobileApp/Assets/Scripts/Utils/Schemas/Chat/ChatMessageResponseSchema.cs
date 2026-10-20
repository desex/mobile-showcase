using Newtonsoft.Json;

namespace Sotbify.MobileApp.Utils.Schemas.Chat
{
    public class ChatMessageResponseSchema
    {
        [JsonProperty("created_at")] public string CreatedAt;
        [JsonProperty("device_id")] public int DeviceId;
        [JsonProperty("id")] public int Id;
        [JsonProperty("manager_id")] public int ManagerId;
        [JsonProperty("media_url")] public string MediaUrl;
        [JsonProperty("msg_type")] public string MessageType;
        [JsonProperty("sender")] public string Sender;
        [JsonProperty("text")] public string Text;
    }
}