using System;
using UnityEngine;

namespace Sotbify.MobileApp.Common.Preferences
{
    public static class PreferencesExtensions
    {
        #region Bool
        
        public static bool GetBool(this IPreferences service, string key, bool defaultValue = false)
        {
            if (service is null)
                throw new ArgumentNullException(nameof(service));

            return service.GetInt(key, defaultValue ? 1 : 0) > 0;
        }
        
        public static void SetBool(this IPreferences service, string key, bool value)
        {
            if (service is null)
                throw new ArgumentNullException(nameof(service));
            
            service.SetInt(key, value ? 1 : 0);
        }
        
        #endregion

        #region GUID
        
        public static Guid GetGuid(this IPreferences service, string key, Guid defaultValue = default)
        {
            if (service is null)
                throw new ArgumentNullException(nameof(service));

            if (service.Has(key) == false)
                return defaultValue;
            
            var guidString = service.GetString(key, string.Empty);
            return Guid.TryParse(guidString, out var result) ? result : defaultValue;
        }
        
        public static void SetGuid(this IPreferences service, string key, Guid value)
        {
            if (service is null)
                throw new ArgumentNullException(nameof(service));
            
            service.SetString(key, value.ToString());
        }

        #endregion

        #region Object
        
        public static T Get<T>(this IPreferences service, string key, T defaultValue = default)
        {
            if (service is null)
                throw new ArgumentNullException(nameof(service));

            var json = service.GetString(key, string.Empty);
            if (string.IsNullOrEmpty(json))
                return defaultValue;
            
            return JsonUtility.FromJson<T>(json) ?? defaultValue;
        }
        
        public static void Set(this IPreferences service, string key, object value)
        {
            if (service is null)
                throw new ArgumentNullException(nameof(service));

            var json = JsonUtility.ToJson(value);
            service.SetString(key, json);
        }
        
        #endregion
        
        #region Long
        
        public static long GetLong(this IPreferences service, string key, long defaultValue = default)
        {
            if (service is null)
                throw new ArgumentNullException(nameof(service));

            if (service.Has(key) == false)
                return defaultValue;
            
            var longString = service.GetString(key, string.Empty);
            return long.TryParse(longString, out var result) ? result : defaultValue;
        }
        
        public static void SetLong(this IPreferences service, string key, long value)
        {
            if (service is null)
                throw new ArgumentNullException(nameof(service));
            
            service.SetString(key, value.ToString());
        }
        
        #endregion
        
        #region DateTime
        
        public static DateTime GetDateTime(this IPreferences service, string key, DateTime defaultValue = default)
        {
            if (service is null)
                throw new ArgumentNullException(nameof(service));

            if (service.Has(key) == false)
                return defaultValue;

            var ticks = service.GetLong(key, defaultValue.Ticks);
            return new DateTime(ticks);
        }
        
        public static void SetDateTime(this IPreferences service, string key, DateTime value)
        {
            if (service is null)
                throw new ArgumentNullException(nameof(service));
            
            service.SetLong(key, value.Ticks);
        }
        
        #endregion
    }
}