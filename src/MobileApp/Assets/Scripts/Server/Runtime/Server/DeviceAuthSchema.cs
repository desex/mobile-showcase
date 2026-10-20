using System;
using Newtonsoft.Json;

namespace Sotbify.MobileApp.Server
{
    [Serializable]
    public class DeviceAuthSchema
    {
        [JsonProperty("qr_token")] public string QRToken;
        [JsonProperty("language_code")] public string LocaleCode;
    }
}