using Newtonsoft.Json;

namespace Sotbify.MobileApp.Utils.Schemas
{
    public struct DeviceInfoSchema
    {
        [JsonProperty("id")] public int Id;
        [JsonProperty("serial_number")] public string SerialNumber;
        [JsonProperty("player_id")] public string PlayerId;
        [JsonProperty("player_name")] public string PlayerName;
        [JsonProperty("is_activated")] public bool IsActivated;
    }
}