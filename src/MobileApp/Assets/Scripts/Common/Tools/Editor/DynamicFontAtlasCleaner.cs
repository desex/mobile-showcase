using System.Linq;
using Sotbifay.MobileApp.Common.Tools.Editor.Utilities;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

namespace Sotbifay.MobileApp.Common.Tools.Editor
{
    [InitializeOnLoad]
    public static class DynamicFontAtlasCleaner
    {
        static DynamicFontAtlasCleaner()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
        }

        private static void OnBeforeAssemblyReload()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.EnteredEditMode)
                return;

            ClearDynamicFonts();
        }

        private static void ClearDynamicFonts()
        {
            var fontAssets = AssetsUtils.FindAllByType<TMP_FontAsset>()
                .Where(static fontAsset => fontAsset != null)
                .Where(static fontAsset => fontAsset.atlasPopulationMode == AtlasPopulationMode.Dynamic)
                .Where(static fontAsset => fontAsset.characterTable.Count > 0 || fontAsset.glyphTable.Count > 0);

            using (ListPool<string>.Get(out var clearedFonts))
            {
                foreach (var fontAsset in fontAssets)
                {
                    fontAsset.ClearFontAssetData(setAtlasSizeToZero: true);
                    EditorUtility.SetDirty(fontAsset);

                    clearedFonts.Add(fontAsset.name);
                }

                if (clearedFonts.Count == 0)
                    return;

                AssetDatabase.SaveAssets();
                
                Debug.Log($"[{nameof(DynamicFontAtlasCleaner)}] Dynamic TMP fonts cleared({clearedFonts.Count}): {string.Join(", ", clearedFonts)}");
            }
        }
    }
}