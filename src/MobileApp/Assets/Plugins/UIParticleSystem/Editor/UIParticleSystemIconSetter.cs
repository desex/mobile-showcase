#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TierStudios.UIParticleSystem.Editor
{
    [InitializeOnLoad]
    public static class UIParticleSystemIconSetter
    {
        private const string IconTextureName = "UIParticleSystemIcon";
        private const float SceneIconSize = 28f;

        private static Texture2D cachedIcon;
        private static bool scriptIconApplied;

        static UIParticleSystemIconSetter()
        {
            EditorApplication.delayCall += ApplyAllIcons;
            EditorApplication.hierarchyChanged += ApplySceneObjectIcons;
            EditorSceneManager.sceneOpened += OnSceneOpened;
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            EditorApplication.delayCall += ApplySceneObjectIcons;
        }

        private static void ApplyAllIcons()
        {
            ApplyScriptIcon();
            ApplySceneObjectIcons();
        }

        private static Texture2D GetIcon()
        {
            if (cachedIcon != null) return cachedIcon;

            cachedIcon = FindIconTexture();
            return cachedIcon;
        }

        private static Texture2D FindIconTexture()
        {
            var possiblePaths = new[]
            {
                "Assets/TierStudios/UIParticleSystem/Editor/Icons/UIParticleSystemIcon.png",
                "Assets/UIParticleSystem/Editor/Icons/UIParticleSystemIcon.png"
            };

            foreach (var iconPath in possiblePaths)
            {
                var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
                if (tex != null) return tex;
            }

            var scriptGuids = AssetDatabase.FindAssets("UIParticleSystemEditorIcons");
            if (scriptGuids.Length > 0)
            {
                var dir = Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(scriptGuids[0]));
                var localIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(
                    Path.Combine(dir, "Icons", IconTextureName + ".png").Replace("\\", "/"));
                if (localIcon != null) return localIcon;
            }

            string[] guids = AssetDatabase.FindAssets(IconTextureName + " t:Texture2D");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains("UIParticleSystem") && Path.GetFileNameWithoutExtension(path) == IconTextureName)
                    return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            }

            return null;
        }

        private static void ApplyScriptIcon()
        {
            if (scriptIconApplied) return;

            Texture2D icon = GetIcon();
            if (icon == null) return;

            string scriptPath = FindUIParticleSystemScript();
            if (string.IsNullOrEmpty(scriptPath)) return;

            var script = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath);
            if (script == null) return;

            EditorGUIUtility.SetIconForObject(script, icon);

#if UNITY_2021_2_OR_NEWER
            var importer = AssetImporter.GetAtPath(scriptPath) as MonoImporter;
            if (importer != null && importer.GetIcon() == null)
            {
                importer.SetIcon(icon);
                importer.SaveAndReimport();
            }
#endif
            scriptIconApplied = true;
        }

        private static void ApplySceneObjectIcons()
        {
            Texture2D icon = GetIcon();
            if (icon == null) return;

            Core.UIParticleSystem[] systems = FindAllParticleSystems();
            foreach (var system in systems)
            {
                if (system == null) continue;

                if (EditorGUIUtility.GetIconForObject(system.gameObject) != icon)
                    EditorGUIUtility.SetIconForObject(system.gameObject, icon);

                if (EditorGUIUtility.GetIconForObject(system) != icon)
                    EditorGUIUtility.SetIconForObject(system, icon);
            }
        }

        private static Core.UIParticleSystem[] FindAllParticleSystems()
        {
#if UNITY_2023_1_OR_NEWER
            return Object.FindObjectsByType<Core.UIParticleSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#elif UNITY_2020_1_OR_NEWER
            return Object.FindObjectsOfType<Core.UIParticleSystem>(true);
#else
            return Object.FindObjectsOfType<Core.UIParticleSystem>();
#endif
        }

        private static string FindUIParticleSystemScript()
        {
            foreach (var guid in AssetDatabase.FindAssets("t:MonoScript UIParticleSystem"))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.EndsWith("/UIParticleSystem.cs") || path.EndsWith("\\UIParticleSystem.cs"))
                    return path;
            }
            return null;
        }

        private static Vector3 GetSystemWorldCenter(Core.UIParticleSystem system)
        {
            var rect = system.transform as RectTransform;
            if (rect != null)
                return rect.TransformPoint(rect.rect.center);

            return system.transform.position;
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            if (sceneView == null) return;
            if (Event.current == null || Event.current.type != EventType.Repaint) return;
            if (Selection.gameObjects == null || Selection.gameObjects.Length == 0) return;

            Texture2D icon = GetIcon();
            if (icon == null) return;

            Handles.BeginGUI();
            foreach (var selected in Selection.gameObjects)
            {
                if (selected == null) continue;

                var system = selected.GetComponent<Core.UIParticleSystem>();
                if (system == null) continue;

                Vector3 worldPos = GetSystemWorldCenter(system);
                Vector2 guiPoint = HandleUtility.WorldToGUIPoint(worldPos);
                Rect rect = new Rect(
                    guiPoint.x - SceneIconSize * 0.5f,
                    guiPoint.y - SceneIconSize * 0.5f,
                    SceneIconSize,
                    SceneIconSize);
                GUI.DrawTexture(rect, icon, ScaleMode.ScaleToFit, true);
            }
            Handles.EndGUI();
        }
    }
}
#endif
