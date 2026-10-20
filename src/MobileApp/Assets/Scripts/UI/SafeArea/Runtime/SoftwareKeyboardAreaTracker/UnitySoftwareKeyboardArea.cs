using UnityEngine;

namespace Sotbify.MobileApp.UI.SafeArea
{
    public class UnitySoftwareKeyboardArea : ISoftwareKeyboardArea
    {
        public int GetHeight(bool includeInput = false)
        {
            var area = TouchScreenKeyboard.area;
            var height = Mathf.RoundToInt(area.height);
            return Screen.height <= height ? 0 : height;
        }

        public void Dispose()
        {
        }
    }
}