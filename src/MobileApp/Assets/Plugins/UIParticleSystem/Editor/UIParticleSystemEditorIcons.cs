#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace TierStudios.UIParticleSystem.Editor
{
    public static class UIParticleSystemEditorIcons
    {
        private static string iconBasePath = null;
        private static bool isInitialized = false;
        private static readonly Dictionary<string, GUIContent> iconCache = new Dictionary<string, GUIContent>();
        private static readonly Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();

        private static void InitializeIconPaths()
        {
            if (isInitialized) return;

            Texture2D testIcon = Resources.Load<Texture2D>("Icons/ColorOverLifetime");
            if (testIcon != null)
            {
                iconBasePath = "RESOURCES";
                isInitialized = true;
                return;
            }

            var scriptGuids = AssetDatabase.FindAssets("UIParticleSystemEditorIcons");
            if (scriptGuids.Length > 0)
            {
                var scriptPath = AssetDatabase.GUIDToAssetPath(scriptGuids[0]);
                var scriptDirectory = Path.GetDirectoryName(scriptPath);

                var resourceIconPath = Path.Combine(Path.GetDirectoryName(scriptDirectory), "Resources", "Icons").Replace("\\", "/");
                if (AssetDatabase.IsValidFolder(resourceIconPath))
                {
                    iconBasePath = resourceIconPath;
                    isInitialized = true;
                    return;
                }

                iconBasePath = Path.Combine(scriptDirectory, "Resources", "Icons").Replace("\\", "/");
                if (AssetDatabase.IsValidFolder(iconBasePath))
                {
                    isInitialized = true;
                    return;
                }

                iconBasePath = Path.Combine(scriptDirectory, "Icons").Replace("\\", "/");
                if (AssetDatabase.IsValidFolder(iconBasePath))
                {
                    isInitialized = true;
                    return;
                }
            }

            if (string.IsNullOrEmpty(iconBasePath) || !AssetDatabase.IsValidFolder(iconBasePath))
            {
                var uiParticleSystemFolders = AssetDatabase.FindAssets("UIParticleSystem", new[] { "Assets" })
                    .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                    .Where(path => AssetDatabase.IsValidFolder(path) && Path.GetFileName(path) == "UIParticleSystem")
                    .ToArray();

                if (uiParticleSystemFolders.Length > 0)
                {
                    var uiParticleSystemPath = uiParticleSystemFolders[0];

                    var newResourceIconPath = Path.Combine(uiParticleSystemPath, "Editor", "Icons").Replace("\\", "/");
                    if (AssetDatabase.IsValidFolder(newResourceIconPath))
                    {
                        iconBasePath = newResourceIconPath;
                        isInitialized = true;
                        return;
                    }

                    var oldResourceIconPath = Path.Combine(uiParticleSystemPath, "Editor", "Resources", "Icons").Replace("\\", "/");
                    if (AssetDatabase.IsValidFolder(oldResourceIconPath))
                    {
                        iconBasePath = oldResourceIconPath;
                        isInitialized = true;
                        return;
                    }

                    var oldIconPath = Path.Combine(uiParticleSystemPath, "Editor", "Icons").Replace("\\", "/");
                    if (AssetDatabase.IsValidFolder(oldIconPath))
                    {
                        iconBasePath = oldIconPath;
                        isInitialized = true;
                        return;
                    }
                }
            }

            isInitialized = true;
        }

        public static GUIContent GetIcon(string iconName)
        {
            if (iconCache.TryGetValue(iconName, out GUIContent cached))
            {
                if (cached != null && cached.image != null)
                    return cached;
            }

            InitializeIconPaths();

            Texture2D iconTexture = null;

            if (iconBasePath == "RESOURCES" || string.IsNullOrEmpty(iconBasePath))
            {
                iconTexture = Resources.Load<Texture2D>($"Icons/{iconName}");
                if (iconTexture != null)
                {
                    var content = new GUIContent(iconTexture);
                    iconCache[iconName] = content;
                    return content;
                }
            }

            if (!string.IsNullOrEmpty(iconBasePath) && iconBasePath != "RESOURCES")
            {
                string iconPath = Path.Combine(iconBasePath, iconName + ".png").Replace("\\", "/");
                iconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);

                if (iconTexture != null)
                {
                    var content = new GUIContent(iconTexture);
                    iconCache[iconName] = content;
                    return content;
                }

                iconPath = Path.Combine(iconBasePath, iconName + ".PNG").Replace("\\", "/");
                iconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);

                if (iconTexture != null)
                {
                    var content = new GUIContent(iconTexture);
                    iconCache[iconName] = content;
                    return content;
                }
            }

            string[] guids = AssetDatabase.FindAssets($"{iconName} t:Texture2D");
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (assetPath.Contains(iconName))
                {
                    iconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                    if (iconTexture != null)
                    {
                        var content = new GUIContent(iconTexture);
                        iconCache[iconName] = content;
                        return content;
                    }
                }
            }

            var fallback = GetFallbackIcon(iconName);
            iconCache[iconName] = fallback;
            return fallback;
        }

        public static Texture2D GetIconTexture(string iconName)
        {
            if (textureCache.TryGetValue(iconName, out Texture2D cached))
            {
                if (cached != null)
                    return cached;
            }

            InitializeIconPaths();

            Texture2D iconTexture = null;

            if (iconBasePath == "RESOURCES" || string.IsNullOrEmpty(iconBasePath))
            {
                iconTexture = Resources.Load<Texture2D>($"Icons/{iconName}");
                if (iconTexture != null)
                {
                    textureCache[iconName] = iconTexture;
                    return iconTexture;
                }
            }

            if (!string.IsNullOrEmpty(iconBasePath) && iconBasePath != "RESOURCES")
            {
                string iconPath = Path.Combine(iconBasePath, iconName + ".png").Replace("\\", "/");
                iconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);

                if (iconTexture != null)
                {
                    textureCache[iconName] = iconTexture;
                    return iconTexture;
                }

                iconPath = Path.Combine(iconBasePath, iconName + ".PNG").Replace("\\", "/");
                iconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);

                if (iconTexture != null)
                {
                    textureCache[iconName] = iconTexture;
                    return iconTexture;
                }
            }

            string[] guids = AssetDatabase.FindAssets($"{iconName} t:Texture2D");
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (assetPath.Contains(iconName))
                {
                    iconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                    if (iconTexture != null)
                    {
                        textureCache[iconName] = iconTexture;
                        return iconTexture;
                    }
                }
            }

            return null;
        }

        private static GUIContent GetFallbackIcon(string iconName)
        {
            switch (iconName.ToLower())
            {
                case "uiparticlesystemicon":
                case "particle":
                    return EditorGUIUtility.IconContent("Prefab Icon");

                case "emmision":
                case "emission":
                    return EditorGUIUtility.IconContent("ParticleSystem Icon");

                case "shape":
                    return EditorGUIUtility.IconContent("RectTransform Icon");

                case "coloroverlifetime":
                case "color":
                    return EditorGUIUtility.IconContent("ColorField");

                case "sizeoverlifetime":
                case "size":
                    return EditorGUIUtility.IconContent("ScaleTool");

                case "rotationoverlifetime":
                case "rotation":
                    return EditorGUIUtility.IconContent("RotateTool");

                case "noise":
                    return EditorGUIUtility.IconContent("WaveSine");

                case "texturesheetanimation":
                case "texturesheet":
                    return EditorGUIUtility.IconContent("Texture2D Icon");

                case "trail":
                    return EditorGUIUtility.IconContent("TrailRenderer Icon");

                case "attractor":
                    return EditorGUIUtility.IconContent("AvatarPivot");

                case "events":
                    return EditorGUIUtility.IconContent("EventTrigger Icon");

                case "velocityoverlifetime":
                case "velocity":
                    return EditorGUIUtility.IconContent("MoveTool");

                case "subemitters":
                    return EditorGUIUtility.IconContent("ParticleSystem Icon");

                case "soundeffect":
                    return EditorGUIUtility.IconContent("AudioSource Icon");

                case "collision":
                    return EditorGUIUtility.IconContent("BoxCollider2D Icon");

                case "inheritvelocity":
                    return EditorGUIUtility.IconContent("Rigidbody Icon");

                case "mobileoptimization":
                    return EditorGUIUtility.IconContent("BuildSettings.Android");

                default:
                    return EditorGUIUtility.IconContent("_Help");
            }
        }

        public static void ClearCache()
        {
            iconCache.Clear();
            textureCache.Clear();
            iconBasePath = null;
            isInitialized = false;
        }

        [InitializeOnEnterPlayMode]
        private static void ResetOnEnterPlayMode(EnterPlayModeOptions options)
        {
            ClearCache();
        }
    }
}
#endif
