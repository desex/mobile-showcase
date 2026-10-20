#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Sotbify.MobileApp.Common.Preferences.Editor
{
    public class ClearPlayerPreferences : EditorWindow
    {
        [MenuItem("Tools/PlayerPrefs Cleaner")]
        public static void Clear()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("All PlayerPrefs have been deleted!");
        }
    }
}
#endif