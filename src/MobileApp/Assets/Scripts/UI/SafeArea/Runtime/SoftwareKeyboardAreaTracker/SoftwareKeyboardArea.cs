namespace Sotbify.MobileApp.UI.SafeArea
{
    public sealed class SoftwareKeyboardArea :
#if UNITY_ANDROID && UNITY_EDITOR == false
        AndroidSoftwareKeyboardArea
#elif UNITY_IOS && UNITY_EDITOR == false
        IOSSoftwareKeyboardArea
#elif UNITY_EDITOR
        EditorSoftwareKeyboardArea
#else
        UnitySoftwareKeyboardArea
#endif
    {
    }
}