using Newtonsoft.Json;

namespace Sotbify.MobileApp.Utils.Schemas.Password
{
    public struct HasPasswordRequestSchema
    {
        [JsonProperty("qr_token")] public string QRToken;
    }
}