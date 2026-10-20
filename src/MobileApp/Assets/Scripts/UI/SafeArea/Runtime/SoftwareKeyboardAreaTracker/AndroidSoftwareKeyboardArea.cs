using UnityEngine;

namespace Sotbify.MobileApp.UI.SafeArea
{
    public class AndroidSoftwareKeyboardArea : ISoftwareKeyboardArea
    {
        private const int AndroidR = 30;
        
        private AndroidJavaObject _unityPlayer;
        private AndroidJavaObject _rootView;
        
        private int _cachedSdkInt = -1;
        private int _cachedImeType = -1;
        private bool? _hasSoftInputDialog;

        protected AndroidSoftwareKeyboardArea()
        {
            using var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var currentActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
            _unityPlayer = currentActivity.Get<AndroidJavaObject>("mUnityPlayer");
            
            using var view = _unityPlayer?.Call<AndroidJavaObject>("getView");
            _rootView = view?.Call<AndroidJavaObject>("getRootView");
        }

        public int GetHeight(bool includeInput = false)
        {
            var rootView = GetRootView();
            if (rootView is null)
                return 0;

            var viewHeight = rootView.Call<int>("getHeight");
            if (viewHeight <= 0)
                return 0;

            var height = GetImeHeight(rootView, viewHeight);
            if (includeInput)
                height += GetSoftInputDialogHeight();

            if (height <= 0)
                return 0;

            // Screen.height is the render surface, the view is measured in physical pixels.
            var scale = (float)Screen.height / viewHeight;
            return Mathf.RoundToInt(height * scale);
        }

        public void Dispose()
        {
            _unityPlayer?.Dispose();
            _unityPlayer = null;
            
            _rootView?.Dispose();
            _rootView = null;
        }
        
        private AndroidJavaObject GetRootView()
        {
            return _rootView is not null && _rootView.Call<bool>("isAttachedToWindow")
                ? _rootView
                : null;
        }
        
        private int GetImeHeight(AndroidJavaObject rootView, int viewHeight)
        {
            using var windowInsets = rootView.Call<AndroidJavaObject>("getRootWindowInsets");

            // Android 11+ reports the keyboard directly, independently of the window mode
            if (windowInsets is not null && GetSdkInt() >= AndroidR)
            {
                using var ime = windowInsets.Call<AndroidJavaObject>("getInsets", GetImeType());
                return ime?.Get<int>("bottom") ?? 0;
            }

            using var rect = new AndroidJavaObject("android.graphics.Rect");
            rootView.Call("getWindowVisibleDisplayFrame", rect);

            // The visible frame is cut by the status bar at the top as well, so measure
            // from its bottom edge and drop the navigation bar that is always occluding.
            var occluded = viewHeight - rect.Get<int>("bottom");
            var navigationBar = windowInsets?.Call<int>("getStableInsetBottom") ?? 0;
            var height = occluded - navigationBar;

            // Anything smaller than that is an overlay of some kind, not the keyboard.
            return height > viewHeight * 0.15f ? height : 0;
        }
        
        private int GetSdkInt()
        {
            if (_cachedSdkInt > 0)
                return _cachedSdkInt;

            using var version = new AndroidJavaClass("android.os.Build$VERSION");
            return _cachedSdkInt = version.GetStatic<int>("SDK_INT");
        }
        
        private int GetImeType()
        {
            if (_cachedImeType >= 0)
                return _cachedImeType;

            using var type = new AndroidJavaClass("android.view.WindowInsets$Type");
            return _cachedImeType = type.CallStatic<int>("ime");
        }
        
        private int GetSoftInputDialogHeight()
        {
            if (HasSoftInputDialog() == false)
                return 0;

            using var softInputDialog = _unityPlayer.Get<AndroidJavaObject>("mSoftInputDialog");
            using var window = softInputDialog?.Call<AndroidJavaObject>("getWindow");
            using var decorView = window?.Call<AndroidJavaObject>("getDecorView");

            return decorView?.Call<int>("getHeight") ?? 0;
        }
        
        private bool HasSoftInputDialog()
        {
            if (_hasSoftInputDialog.HasValue)
                return _hasSoftInputDialog.Value;

            try
            {
                using var playerClass = _unityPlayer.Call<AndroidJavaObject>("getClass");
                using var field = playerClass.Call<AndroidJavaObject>("getDeclaredField", "mSoftInputDialog");
                _hasSoftInputDialog = field is not null;
            }
            catch (AndroidJavaException)
            {
                // UnityPlayerForGameActivity has no soft input dialog, the input field
                // is a part of the IME there and is already covered by its inset.
                _hasSoftInputDialog = false;
            }

            return _hasSoftInputDialog.Value;
        }
    }
}