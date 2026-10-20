using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Sotbifay.Launcher.Common.Tools.Editor
{
    [InitializeOnLoad]
    public static class PlayFromBootSceneTool
    { 
        private const string MenuName = "Sotbifay/Play from boot scene";
        private static readonly string IsEnabledKey = $"{nameof(PlayFromBootSceneTool)}_Enabled";
        
        private static string _lastOpenedScenePath = string.Empty; 
        
        private static bool IsEnabled
        {
            get => EditorPrefs.GetBool(IsEnabledKey, true);
            set => EditorPrefs.SetBool(IsEnabledKey, value);
        }
        
        static PlayFromBootSceneTool()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            Menu.SetChecked(MenuName, IsEnabled);
        }
        
        [MenuItem(MenuName)]
        private static void MenuToggleAction(MenuCommand command)
        {
            IsEnabled = IsEnabled == false;
            Menu.SetChecked(MenuName, IsEnabled);
        }
        
        private static void OnBeforeAssemblyReload()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.EnteredEditMode:
                    OpenLastScene();
                    break;
                
                case PlayModeStateChange.ExitingEditMode:
                    ChangeScene();
                    break;
            }
        }
        
        private static void ChangeScene()
        {
            if (IsEnabled == false)
                return;
            
            var scenes = EditorBuildSettings.scenes;
            if (scenes.Length <= 0)
                return;

            var firstScenePath = scenes[0].path;
            var lastOpenedScene = SceneManager.GetActiveScene();
            _lastOpenedScenePath = lastOpenedScene.path;

            if (firstScenePath == lastOpenedScene.path)
                return;
            
            EditorSceneManager.SaveOpenScenes();
            EditorSceneManager.OpenScene(firstScenePath);
        }

        private static void OpenLastScene()
        {
            if (string.IsNullOrEmpty(_lastOpenedScenePath))
                return;

            EditorSceneManager.OpenScene(_lastOpenedScenePath);
        }
    }
}