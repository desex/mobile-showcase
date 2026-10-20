using System;
using UnityEngine;
using VContainer.Unity;

namespace Sotbify.MobileApp.Common.Preferences
{
    public sealed class UnityPlayerPreferences : IPreferences, IInitializable, IDisposable
    {
        public bool Has(string key) => PlayerPrefs.HasKey(key);

        public void Remove(string key) => PlayerPrefs.DeleteKey(key);

        public void RemoveAll() => PlayerPrefs.DeleteAll();

        public int GetInt(string key, int defaultValue = default) => 
            PlayerPrefs.GetInt(key, defaultValue);

        public void SetInt(string key, int value) => 
            PlayerPrefs.SetInt(key, value);

        public float GetFloat(string key, float defaultValue = default) => 
            PlayerPrefs.GetFloat(key, defaultValue);

        public void SetFloat(string key, float value) => 
            PlayerPrefs.SetFloat(key, value);

        public string GetString(string key, string defaultValue = default) => 
            PlayerPrefs.GetString(key, defaultValue);

        public void SetString(string key, string value)
        {
            if (value is null)
            {
                Remove(key);
                return;
            }
            
            PlayerPrefs.SetString(key, value);
        }
        
        public void Initialize()
        {
            Application.focusChanged += OnFocusChanged;
        }

        public void Dispose()
        {
            Application.focusChanged -= OnFocusChanged;
            Save();
        }

        public void Save() => PlayerPrefs.Save();

        private void OnFocusChanged(bool focus)
        {
            if (focus == false)
                Save();
        }
    }
}