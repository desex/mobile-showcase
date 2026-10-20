namespace Sotbify.MobileApp.Services.RemoteConfig
{
    public interface IRemoteConfigService
    {
        string GetString(string key);
        bool GetBool(string key);
        long GetLong(string key);
        double GetDouble(string key);
    }
}