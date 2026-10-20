using Newtonsoft.Json;

namespace Sotbify.MobileApp.Utils.Schemas.Password
{
    public struct SavePasswordRequestSchema
    {
        [JsonProperty("password")] public string Password;
    }
}