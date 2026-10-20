using UnityEngine;
#if UNITY_IOS && UNITY_EDITOR == false
using System.Runtime.InteropServices;
#endif

namespace Sotbify.MobileApp.UI.SafeArea
{
    public class IOSSoftwareKeyboardArea : ISoftwareKeyboardArea
    {
        public int GetHeight(bool includeInput = false)
        {
            var normalizedHeight = Mathf.Clamp01(SoftwareKeyboardArea_GetNormalizedHeight());
            var height = Mathf.RoundToInt(normalizedHeight * Screen.height);

            if (includeInput && height > 0)
                height = Mathf.Max(height, Mathf.RoundToInt(TouchScreenKeyboard.area.height));

            return height < Screen.height ? height : 0;
        }

        public void Dispose()
        {
        }
        
#if UNITY_IOS && UNITY_EDITOR == false
        [DllImport("__Internal")]
        private static extern float SoftwareKeyboardArea_GetNormalizedHeight();
#else
        private static float SoftwareKeyboardArea_GetNormalizedHeight() => 0;
#endif
    }
}
