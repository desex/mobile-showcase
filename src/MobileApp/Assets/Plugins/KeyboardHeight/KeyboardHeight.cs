using System;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

namespace Sotbify.MobileApp.Plugins.KeyboardHeight
{
    public static class KeyboardHeight
    {
        
#if UNITY_IOS && !UNITY_EDITOR
        private delegate void KeyboardHeightChangedCallback(int height);

        [DllImport("__Internal")]
        private static extern void RegisterKeyboardCallback(KeyboardHeightChangedCallback callback);

        [DllImport("__Internal")]
        private static extern void UnregisterKeyboardCallback();

        [DllImport("__Internal")]
        private static extern bool IsKeyboardHeightPluginTerminating();

        [DllImport("__Internal")]
        private static extern int GetCachedKeyboardHeight();

        private static readonly KeyboardHeightChangedCallback Callback = OnKeyboardHeightChanged;
#elif UNITY_ANDROID && !UNITY_EDITOR
        private static AndroidJavaObject _activity;
        private static AndroidJavaClass _plugin;
        private static AndroidKeyboardHeightCallback _callback;
#endif

#pragma warning disable CS0414 // Field is assigned but its value is never used
        private static int _cachedHeight = 0;
#pragma warning restore CS0414 // Field is assigned but its value is never used
#pragma warning disable CS0414 // Field is assigned but its value is never used
        private static bool _isRegistered = false;
#pragma warning restore CS0414 // Field is assigned but its value is never used
        private static SynchronizationContext _unitySynchronizationContext;
        private static int _unityThreadId;

        public static event Action<int> HeightChanged;

        static KeyboardHeight()
        {
#if UNITY_IOS && !UNITY_EDITOR
            Register();
#endif
        }

        public static bool TryGetHeight(out int height)
        {
            height = GetHeight();
            return height > 0;
        }

        public static void Register()
        {
            CaptureUnitySynchronizationContext();

#if UNITY_IOS && !UNITY_EDITOR
            if (_isRegistered || IsTerminating())
            {
                return;
            }

            RegisterKeyboardCallback(Callback);
            _isRegistered = true;
#elif UNITY_ANDROID && !UNITY_EDITOR
            if (_isRegistered)
            {
                return;
            }

            using var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            _activity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
            _plugin = new AndroidJavaClass("com.sotbify.mobileapp.keyboardheight.KeyboardHeightPlugin");
            _callback = new AndroidKeyboardHeightCallback();
            _plugin.CallStatic("register", _activity, _callback);

            _isRegistered = true;
#endif
        }

        public static void Unregister()
        {
            if (HeightChanged != null)
            {
                return;
            }

#if UNITY_IOS && !UNITY_EDITOR
            if (!_isRegistered)
            {
                return;
            }

            UnregisterKeyboardCallback();
            _isRegistered = false;
#elif UNITY_ANDROID && !UNITY_EDITOR
            if (!_isRegistered)
            {
                return;
            }

            _plugin?.CallStatic("unregister");
            _callback = null;
            _plugin?.Dispose();
            _plugin = null;
            _activity?.Dispose();
            _activity = null;
            _isRegistered = false;
#endif
            _cachedHeight = 0;
        }

        public static bool IsTerminating()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return IsKeyboardHeightPluginTerminating();
#else
            return false;
#endif
        }

        public static int GetHeight()
        {
#if UNITY_IOS && !UNITY_EDITOR
            if (IsTerminating())
            {
                return 0;
            }

            Register();

            if (_cachedHeight > 0)
            {
                return _cachedHeight;
            }
            
            int cached = GetCachedKeyboardHeight();
            if (cached > 0)
            {
                _cachedHeight = cached;
            }
            return cached;
#elif UNITY_ANDROID && !UNITY_EDITOR
            Register();
            return _plugin?.CallStatic<int>("getHeight") ?? 0;
#elif UNITY_EDITOR
            return 0;
#else
            return 0;
#endif
        }

#if UNITY_IOS && !UNITY_EDITOR
        [AOT.MonoPInvokeCallback(typeof(KeyboardHeightChangedCallback))]
        private static void OnKeyboardHeightChanged(int height)
        {
            if (!_isRegistered || IsTerminating())
            {
                return;
            }

            _cachedHeight = height;
            RaiseHeightChanged(height);
            Debug.Log($"[KeyboardHeight] Height updated: {height}");
        }
#elif UNITY_ANDROID && !UNITY_EDITOR
        private static void OnAndroidKeyboardHeightChanged(int height)
        {
            if (!_isRegistered)
            {
                return;
            }

            if (_cachedHeight == height)
            {
                return;
            }

            _cachedHeight = height;
            RaiseHeightChanged(height);
            Debug.Log($"[KeyboardHeight] Height updated: {height}");
        }

        private sealed class AndroidKeyboardHeightCallback : AndroidJavaProxy
        {
            public AndroidKeyboardHeightCallback() : base("com.sotbify.mobileapp.keyboardheight.KeyboardHeightPlugin$Callback")
            {
            }

            public void onKeyboardHeightChanged(int height)
            {
                OnAndroidKeyboardHeightChanged(height);
            }
        }
#endif

        private static void CaptureUnitySynchronizationContext()
        {
            if (_unitySynchronizationContext != null)
            {
                return;
            }

            _unitySynchronizationContext = SynchronizationContext.Current;
            _unityThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        private static void RaiseHeightChanged(int height)
        {
            var currentThreadId = Thread.CurrentThread.ManagedThreadId;
            if (_unitySynchronizationContext == null || currentThreadId == _unityThreadId)
            {
                HeightChanged?.Invoke(height);
                return;
            }

            _unitySynchronizationContext.Post(_ =>
            {
                HeightChanged?.Invoke(height);
            }, null);
        }
    }
}