#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine.U2D;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace TierStudios.UIParticleSystem.Editor
{
    [InitializeOnLoad]
    public static class UIParticleAtlasManager
    {
        private const string ATLAS_FOLDER = "Assets/TierStudios/UIParticleSystem/InternalAssets/Atlas";
        private const string ATLAS_NAME = "UIParticleAtlas";
        private const string PREFS_KEY_WARNING_SHOWN = "UIParticleAtlas_WarningShown";

        private static SpriteAtlas cachedAtlas;

        static UIParticleAtlasManager()
        {
            EditorApplication.delayCall += EnsureSpriteAtlasEnabled;
        }

        [InitializeOnEnterPlayMode]
        private static void ResetOnEnterPlayMode(EnterPlayModeOptions options)
        {
            cachedAtlas = null;
        }

        private static void EnsureSpriteAtlasEnabled()
        {
            if (IsSpriteAtlasEnabled()) return;

            if (TryEnableSpriteAtlasSilently())
            {
                return;
            }

            if (!EditorPrefs.GetBool(PREFS_KEY_WARNING_SHOWN, false))
            {
                ShowWarningDialog();
            }
        }

        private static bool TryEnableSpriteAtlasSilently()
        {
            try
            {
#if UNITY_2021_2_OR_NEWER
                SpritePackerMode targetMode = SpritePackerMode.SpriteAtlasV2;
#else
                SpritePackerMode targetMode = SpritePackerMode.AlwaysOnAtlas;
#endif
                EditorSettings.spritePackerMode = targetMode;

                if (EditorSettings.spritePackerMode == targetMode)
                {
                    EditorPrefs.SetBool(PREFS_KEY_WARNING_SHOWN, true);
                    EditorApplication.delayCall += PackAtlasSilently;
                    return true;
                }

                return false;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[UIParticleAtlas] Could not enable Sprite Atlas automatically: {e.Message}");
                return false;
            }
        }

        private static void ShowWarningDialog()
        {
            bool openSettings = EditorUtility.DisplayDialog(
                "⚠️ Sprite Atlas Setup Required - UIParticleSystem",
                "UIParticleSystem requires Sprite Atlas to be enabled for draw call optimization.\n\n" +
                "Automatic enable failed. Please enable manually:\n\n" +
                "STEPS:\n" +
                "1. Edit > Project Settings > Editor\n" +
                "2. Find 'Sprite Atlas' setting\n" +
                "3. Change Mode to 'Sprite Atlas V2 - Enabled'\n" +
                "4. Restart Unity\n\n" +
                "Click 'Open Settings' to open Project Settings.",
                "Open Settings",
                "Cancel"
            );

            if (openSettings)
            {
                SettingsService.OpenProjectSettings("Project/Editor");
                EditorPrefs.SetBool(PREFS_KEY_WARNING_SHOWN, true);
            }
        }

        private static bool IsSpriteAtlasEnabled()
        {
            try
            {
                return EditorSettings.spritePackerMode != SpritePackerMode.Disabled;
            }
            catch
            {
                return false;
            }
        }

        public static void EnsureSpriteInAtlas(Sprite sprite)
        {
            if (sprite == null) return;

            if (!IsSpriteAtlasEnabled() && !TryEnableSpriteAtlasSilently())
            {
                Debug.LogWarning($"[UIParticleAtlas] ⚠️ Cannot add '{sprite.name}' - Sprite Atlas disabled!");
                return;
            }

            SpriteAtlas atlas = GetOrCreateAtlas();
            if (atlas == null)
            {
                Debug.LogError("[UIParticleAtlas] Failed to get or create atlas!");
                return;
            }

            if (IsSpriteInAtlas(atlas, sprite))
            {
                return;
            }

            AddSpriteToAtlas(atlas, sprite);
        }

        private static SpriteAtlas GetOrCreateAtlas()
        {
            if (cachedAtlas != null) return cachedAtlas;

            string atlasPath = Path.Combine(ATLAS_FOLDER, ATLAS_NAME + ".spriteatlas");
            cachedAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPath);

            if (cachedAtlas != null) return cachedAtlas;

            string[] guids = AssetDatabase.FindAssets(ATLAS_NAME + " t:SpriteAtlas");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Replace("\\", "/").Contains("/UIParticleSystem/"))
                {
                    cachedAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path);
                    if (cachedAtlas != null)
                        return cachedAtlas;
                }
            }

            return CreateNewAtlas();
        }

        private static SpriteAtlas CreateNewAtlas()
        {
            EnsureFolderExists(ATLAS_FOLDER);

            SpriteAtlas atlas = new SpriteAtlas();

            atlas.SetPackingSettings(new SpriteAtlasPackingSettings
            {
                blockOffset = 1,
                enableRotation = false,
                enableTightPacking = false,
                padding = 2,
            });

            atlas.SetTextureSettings(new SpriteAtlasTextureSettings
            {
                readable = false,
                generateMipMaps = false,
                sRGB = true,
                filterMode = FilterMode.Bilinear,
            });

            var platformSettings = atlas.GetPlatformSettings("DefaultTexturePlatform");
            platformSettings.maxTextureSize = 2048;
            platformSettings.format = TextureImporterFormat.Automatic;
            platformSettings.compressionQuality = 50;
            atlas.SetPlatformSettings(platformSettings);

            string atlasPath = Path.Combine(ATLAS_FOLDER, ATLAS_NAME + ".spriteatlas");
            AssetDatabase.CreateAsset(atlas, atlasPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            cachedAtlas = atlas;

            return atlas;
        }

        private static void EnsureFolderExists(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath)) return;

            string parentFolder = Path.GetDirectoryName(folderPath).Replace("\\", "/");
            string folderName = Path.GetFileName(folderPath);

            if (!AssetDatabase.IsValidFolder(parentFolder))
            {
                string uiParticleSystemFolder = "Assets/TierStudios/UIParticleSystem";
                if (!AssetDatabase.IsValidFolder(uiParticleSystemFolder + "/InternalAssets"))
                {
                    AssetDatabase.CreateFolder(uiParticleSystemFolder, "InternalAssets");
                }
            }

            AssetDatabase.CreateFolder(parentFolder, folderName);
        }

        private static bool IsSpriteInAtlas(SpriteAtlas atlas, Sprite sprite)
        {
            if (atlas == null || sprite == null) return false;

            SerializedObject serializedAtlas = new SerializedObject(atlas);
            SerializedProperty packablesProperty = serializedAtlas.FindProperty("m_EditorData.packables");

            if (packablesProperty == null || !packablesProperty.isArray)
            {
                return false;
            }

            string spritePath = AssetDatabase.GetAssetPath(sprite);
            if (string.IsNullOrEmpty(spritePath))
            {
                Debug.LogWarning($"[UIParticleAtlas] Could not get asset path for sprite '{sprite.name}'");
                return false;
            }

            Object spriteAsset = AssetDatabase.LoadAssetAtPath<Object>(spritePath);
            Texture2D parentTexture = null;

            if (spriteAsset is Texture2D)
            {
                parentTexture = spriteAsset as Texture2D;
            }
            else
            {
                Object[] assets = AssetDatabase.LoadAllAssetsAtPath(spritePath);
                foreach (var asset in assets)
                {
                    if (asset is Texture2D tex)
                    {
                        parentTexture = tex;
                        break;
                    }
                }
            }

            string spriteGuid = AssetDatabase.AssetPathToGUID(spritePath);

            for (int i = 0; i < packablesProperty.arraySize; i++)
            {
                SerializedProperty packable = packablesProperty.GetArrayElementAtIndex(i);
                Object packableObj = packable.objectReferenceValue;

                if (packableObj == null) continue;

                if (packableObj == sprite || packableObj == parentTexture || packableObj == spriteAsset)
                {
                    return true;
                }

                string packablePath = AssetDatabase.GetAssetPath(packableObj);
                string packableGuid = AssetDatabase.AssetPathToGUID(packablePath);

                if (!string.IsNullOrEmpty(spriteGuid) && !string.IsNullOrEmpty(packableGuid))
                {
                    if (packableGuid == spriteGuid)
                    {
                        return true;
                    }
                }

                if (AssetDatabase.IsValidFolder(packablePath) && spritePath.StartsWith(packablePath))
                {
                    return true;
                }
            }

            return false;
        }

        private static void AddSpriteToAtlas(SpriteAtlas atlas, Sprite sprite)
        {
            if (atlas == null || sprite == null)
            {
                Debug.LogError("[UIParticleAtlas] Atlas or sprite is null!");
                return;
            }

            try
            {
                string spritePath = AssetDatabase.GetAssetPath(sprite);
                if (string.IsNullOrEmpty(spritePath))
                {
                    Debug.LogError($"[UIParticleAtlas] Could not get asset path for sprite '{sprite.name}'!");
                    return;
                }

                Object spriteAsset = AssetDatabase.LoadAssetAtPath<Object>(spritePath);
                Texture2D parentTexture = null;

                if (spriteAsset is Texture2D)
                {
                    parentTexture = spriteAsset as Texture2D;
                }
                else
                {
                    Object[] assets = AssetDatabase.LoadAllAssetsAtPath(spritePath);
                    foreach (var asset in assets)
                    {
                        if (asset is Texture2D tex)
                        {
                            parentTexture = tex;
                            break;
                        }
                    }
                }

                if (parentTexture == null)
                {
                    Debug.LogError($"[UIParticleAtlas] Could not find parent texture for sprite '{sprite.name}'!");
                    return;
                }

                OptimizeSpriteForAtlas(spritePath);

                SerializedObject serializedAtlas = new SerializedObject(atlas);
                SerializedProperty packablesProperty = serializedAtlas.FindProperty("m_EditorData.packables");

                if (packablesProperty == null || !packablesProperty.isArray)
                {
                    Debug.LogError("[UIParticleAtlas] Could not find packables property!");
                    return;
                }

                HashSet<string> existingGuids = new HashSet<string>();
                List<int> invalidIndices = new List<int>();

                for (int i = 0; i < packablesProperty.arraySize; i++)
                {
                    SerializedProperty packable = packablesProperty.GetArrayElementAtIndex(i);
                    Object packableObj = packable.objectReferenceValue;

                    if (packableObj == null)
                    {
                        invalidIndices.Add(i);
                        continue;
                    }

                    string packablePath = AssetDatabase.GetAssetPath(packableObj);
                    string packableGuid = AssetDatabase.AssetPathToGUID(packablePath);

                    if (!string.IsNullOrEmpty(packableGuid))
                    {
                        existingGuids.Add(packableGuid);
                    }
                }

                for (int i = invalidIndices.Count - 1; i >= 0; i--)
                {
                    packablesProperty.DeleteArrayElementAtIndex(invalidIndices[i]);
                }

                string textureGuid = AssetDatabase.AssetPathToGUID(spritePath);

                if (string.IsNullOrEmpty(textureGuid))
                {
                    Debug.LogError($"[UIParticleAtlas] Could not get GUID for sprite asset '{spritePath}'!");
                    return;
                }

                if (existingGuids.Contains(textureGuid))
                {
                    serializedAtlas.ApplyModifiedProperties();
                    return;
                }

                int newIndex = packablesProperty.arraySize;
                packablesProperty.InsertArrayElementAtIndex(newIndex);
                SerializedProperty newPackable = packablesProperty.GetArrayElementAtIndex(newIndex);
                newPackable.objectReferenceValue = parentTexture;

                serializedAtlas.ApplyModifiedProperties();
                EditorUtility.SetDirty(atlas);

                AssetDatabase.SaveAssets();

                EditorApplication.delayCall += () =>
                {
                    try
                    {
                        if (atlas != null)
                        {
                            SpriteAtlasUtility.PackAtlases(new[] { atlas }, EditorUserBuildSettings.activeBuildTarget);

                            string atlasPath = AssetDatabase.GetAssetPath(atlas);
                            AssetDatabase.ImportAsset(atlasPath, ImportAssetOptions.ForceUpdate);

                            SpriteAtlas reloadedAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPath);
                        }
                    }
                    catch (System.Exception packEx)
                    {
                        Debug.LogError($"[UIParticleAtlas] Pack failed: {packEx.Message}");
                    }
                };
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[UIParticleAtlas] Failed to add sprite: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private static void OptimizeSpriteForAtlas(string texturePath)
        {
            if (string.IsNullOrEmpty(texturePath)) return;

            TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
            if (importer == null) return;

            bool changed = false;

            if (importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                changed = true;
            }

            if (changed)
            {
                EditorUtility.SetDirty(importer);
                importer.SaveAndReimport();
            }
        }

        public static void PackAtlas()
        {
            SpriteAtlas atlas = GetOrCreateAtlas();
            if (atlas == null)
            {
                Debug.LogError("[UIParticleAtlas] Atlas not found!");
                return;
            }

            try
            {
                EditorApplication.delayCall += () =>
                {
                    if (atlas != null)
                    {
                        SpriteAtlasUtility.PackAtlases(new[] { atlas }, EditorUserBuildSettings.activeBuildTarget);

                        string atlasPath = AssetDatabase.GetAssetPath(atlas);
                        AssetDatabase.ImportAsset(atlasPath, ImportAssetOptions.ForceUpdate);

                        SpriteAtlas reloadedAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPath);
                    }
                };
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[UIParticleAtlas] Pack failed: {ex.Message}");
            }
        }

        private static void PackAtlasSilently()
        {
            SpriteAtlas atlas = GetOrCreateAtlas();
            if (atlas != null)
            {
                try
                {
                    SpriteAtlasUtility.PackAtlases(new[] { atlas }, EditorUserBuildSettings.activeBuildTarget);
                }
                catch
                {
                }
            }
        }
    }
}
#endif
