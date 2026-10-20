using System;
using Sotbifay.MobileApp.Logger;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;
#if UNITY_EDITOR == false
using UnityEngine;
using Plugins.MyNativeBindings;
#endif

namespace Sotbify.MobileApp.Services.SettingsOpener
{
    public class SettingsOpener : ISettingsOpener
    {
        private readonly ILogger _logger;

        public SettingsOpener(ILogger logger)
        {
            _logger = logger;
        }

        public void OpenSettings()
        {
            try
            {
#if UNITY_IOS && !UNITY_EDITOR
            var url = MyNativeBindings.GetSettingsUrl();
            Application.OpenURL(url);
#elif UNITY_ANDROID && !UNITY_EDITOR
            using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                string packageName = currentActivityObject.Call<string>("getPackageName");

                using (var uriClass = new AndroidJavaClass("android.net.Uri"))
                using (AndroidJavaObject uriObject =
                       uriClass.CallStatic<AndroidJavaObject>("fromParts", "package", packageName, null))
                using (var intentObject = new AndroidJavaObject("android.content.Intent",
                           "android.settings.APPLICATION_DETAILS_SETTINGS", uriObject))
                {
                    intentObject.Call<AndroidJavaObject>("addCategory", "android.intent.category.DEFAULT");
                    intentObject.Call<AndroidJavaObject>("setFlags", 0x10000000);
                    currentActivityObject.Call("startActivity", intentObject);
                }
            }
#endif
            }
            catch (Exception e)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                    _logger.Error("Exception while opening settings", e, this);
                
                throw;
            }
        }
    }
}