using System.Runtime.InteropServices;
using UnityEngine;

namespace Plugins.MyNativeBindings
{
    public static class MyNativeBindings
    {
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern System.IntPtr GetSettingsURL();

        [DllImport("__Internal")]
        private static extern void OpenSettings();
#endif

        public static string GetSettingsUrl()
        {
#if UNITY_IOS && !UNITY_EDITOR
            var ptr = GetSettingsURL();
            return Marshal.PtrToStringUTF8(ptr);
#else
            return string.Empty;
#endif
        }

        public static void OpenAppSettings()
        {
#if UNITY_IOS && !UNITY_EDITOR
            OpenSettings();
#else
            Debug.Log("OpenAppSettings works only on iOS");
#endif
        }
    }
}