using UnityEditor;
using UnityEngine;

namespace Sotbifay.Launcher.Common.Tools.Editor
{
    [InitializeOnLoad]
    public static class ColorContextMenuInjector
    {
        static ColorContextMenuInjector()
        {
            EditorApplication.contextualPropertyMenu += OnContextMenu;
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
        }

        private static void OnBeforeAssemblyReload()
        {
            EditorApplication.contextualPropertyMenu -= OnContextMenu;
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
        }

        private static void OnContextMenu(GenericMenu menu, SerializedProperty property)
        {
            if (property.propertyType != SerializedPropertyType.Color)
                return;
            
            // menu.AddItem(new GUIContent("Convert to/Linear"), false, () =>
            // {
            //     property.colorValue = property.colorValue.linear;
            //     property.serializedObject.ApplyModifiedProperties();
            // });
            //
            // menu.AddItem(new GUIContent("Convert to/sRGB"), false, () =>
            // {
            //     property.colorValue = property.colorValue.gamma;
            //     property.serializedObject.ApplyModifiedProperties();
            // });
            
            menu.AddItem(new GUIContent("Paste HEX as/Linear"), false, () =>
            {
                var hex = GetHexFromCopyBuffer();
                
                if (ColorUtility.TryParseHtmlString(hex, out var sRgbColor))
                {
                    property.colorValue = SrgbToLinear(sRgbColor);
                    property.serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    Debug.LogWarning($"Invalid HEX color in clipboard: {hex}");
                }
            });
            
            menu.AddItem(new GUIContent("Paste HEX as/sRGB"), false, () =>
            {
                var hex = GetHexFromCopyBuffer();
                if (ColorUtility.TryParseHtmlString(hex, out var sRgbColor))
                {
                    property.colorValue = sRgbColor;
                    property.serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    Debug.LogWarning($"Invalid HEX color in clipboard: {hex}");
                }
            });
        }

        private static string GetHexFromCopyBuffer()
        {
            var hex = EditorGUIUtility.systemCopyBuffer.Trim();
            return hex.StartsWith('#') == false ? $"#{hex}" : hex;
        }
        
        private static Color SrgbToLinear(Color srgbColor)
        {
            return new Color(
                ChannelToLinear(srgbColor.r),
                ChannelToLinear(srgbColor.g),
                ChannelToLinear(srgbColor.b),
                srgbColor.a
            );
        }

        private static float ChannelToLinear(float c)
        {
            return (c <= 0.04045f) ? c / 12.92f : Mathf.Pow((c + 0.055f) / 1.055f, 2.4f);
        }
    }
}