namespace Sotbify.MobileApp.Common.Preferences
{
    public interface IPreferences
    {
        bool Has(string key);
        
        void Remove(string key);
        void RemoveAll();
        
        int GetInt(string key, int defaultValue = default);
        void SetInt(string key, int value);
        
        float GetFloat(string key, float defaultValue = default);
        void SetFloat(string key, float value);
        
        string GetString(string key, string defaultValue = default);
        void SetString(string key, string value);
        
        void Save();
    }
}