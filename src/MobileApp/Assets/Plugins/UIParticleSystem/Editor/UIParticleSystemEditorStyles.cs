#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace TierStudios.UIParticleSystem.Editor
{
    public class UIParticleSystemEditorStyles
    {
        private GUIStyle settingsStyle;
        private GUIStyle titleStyle;
        private GUIStyle headerStyle;
        private GUIStyle toggleStyle;
        private GUIStyle transparentToggleStyle;
        private GUIStyle buttonStyle;
        private GUIStyle labelStyle;
        private GUIStyle sceneViewPanelStyle;
        private bool stylesInitialized = false;

        public GUIStyle SettingsStyle => settingsStyle;
        public GUIStyle Titlestyle => titleStyle;
        public GUIStyle HeaderStyle => headerStyle;
        public GUIStyle ToggleStyle => toggleStyle;
        public GUIStyle TransparentToggleStyle => transparentToggleStyle;
        public GUIStyle ButtonStyle => buttonStyle;
        public GUIStyle LabelStyle => labelStyle;
        public GUIStyle SceneViewPanelStyle => sceneViewPanelStyle;

        public void EnsureStylesInitialized()
        {
            if (!stylesInitialized)
            {
                InitializeStyles();
                stylesInitialized = true;
            }
        }

        private void InitializeStyles()
        {
            settingsStyle = new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(0, 0, 0, 0),
                margin = new RectOffset(0, 0, 0, 0)
            };


            Color boxColor = new Color(.35f, .35f, .35f, 1f);
            Texture2D bgTexture = MakeColorTexture(2, 2, boxColor);
            titleStyle = new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(0, 0, 0, 0),
                margin = new RectOffset(2, 2, 1, 1),
                normal = { background = bgTexture }
            };

            headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft
            };

            toggleStyle = new GUIStyle(EditorStyles.miniButton)
            {
                fontSize = 10,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                fixedHeight = 22,
                padding = new RectOffset(8, 8, 4, 4)
            };

            transparentToggleStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 10,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                fixedHeight = 15,
                padding = new RectOffset(3, 0, 0, 0),
                margin = new RectOffset(0, 0, 3, 3),
                normal = { background = null, textColor = GetPrimaryColor() },
                hover = { background = null, textColor = GetAccentColor() },
                active = { background = null, textColor = GetAccentColor() },
                focused = { background = null, textColor = GetPrimaryColor() }
            };

            buttonStyle = new GUIStyle(EditorStyles.miniButton)
            {
                fontSize = 9,
                fontStyle = FontStyle.Normal,
                fixedHeight = 18
            };

            labelStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                fontSize = 9,
                alignment = TextAnchor.MiddleLeft
            };

            sceneViewPanelStyle = new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(5, 5, 5, 5),
                margin = new RectOffset(0, 0, 0, 0)
            };
        }

        public GUIStyle CreateCustomStyle(string baseStyleName, int fontSize = 10, FontStyle fontStyle = FontStyle.Normal,
            TextAnchor alignment = TextAnchor.MiddleLeft, int fixedHeight = -1)
        {
            GUIStyle baseStyle = GetBaseStyle(baseStyleName);
            GUIStyle customStyle = new GUIStyle(baseStyle)
            {
                fontSize = fontSize,
                fontStyle = fontStyle,
                alignment = alignment
            };

            if (fixedHeight > 0)
            {
                customStyle.fixedHeight = fixedHeight;
            }

            return customStyle;
        }

        private GUIStyle GetBaseStyle(string styleName)
        {
            switch (styleName.ToLower())
            {
                case "button":
                    return EditorStyles.miniButton;
                case "label":
                    return EditorStyles.label;
                case "boldlabel":
                    return EditorStyles.boldLabel;
                case "helpbox":
                    return EditorStyles.helpBox;
                case "toolbar":
                    return EditorStyles.toolbar;
                case "textfield":
                    return EditorStyles.textField;
                case "popup":
                    return EditorStyles.popup;
                case "foldout":
                    return EditorStyles.foldout;
                default:
                    return EditorStyles.label;
            }
        }

        public Color GetPrimaryColor()
        {
            return EditorGUIUtility.isProSkin ? new Color(0.8f, 0.8f, 0.8f) : new Color(0.2f, 0.2f, 0.2f);
        }

        public Color GetSecondaryColor()
        {
            return EditorGUIUtility.isProSkin ? new Color(0.6f, 0.6f, 0.6f) : new Color(0.4f, 0.4f, 0.4f);
        }

        public Color GetAccentColor()
        {
            return EditorGUIUtility.isProSkin ? new Color(0.3f, 0.6f, 1f) : new Color(0.1f, 0.4f, 0.8f);
        }

        public Color GetLineColor()
        {
            return EditorGUIUtility.isProSkin ? new Color(0.1647059f, 0.1647059f, 0.1647059f) : new Color(0.1647059f, 0.1647059f, 0.1647059f);
        }

        public Color GetWarningColor()
        {
            return new Color(1f, 0.6f, 0.2f);
        }

        public Color GetErrorColor()
        {
            return new Color(1f, 0.3f, 0.3f);
        }

        public Color GetSuccessColor()
        {
            return new Color(0.3f, 0.8f, 0.3f);
        }

        public static Texture2D MakeColorTexture(int width, int height, Color color)
        {
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }

            Texture2D texture = new Texture2D(width, height);
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }

        public void DrawHorizontalLine(Color color, float thickness = 1f, float padding = 5f)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, thickness + padding * 2);
            rect.height = thickness;
            rect.y += padding;
            EditorGUI.DrawRect(rect, color);
        }

        public void DrawBox(Rect rect, Color color, float alpha = 0.1f)
        {
            Color boxColor = new Color(color.r, color.g, color.b, alpha);
            EditorGUI.DrawRect(rect, boxColor);
        }

        public bool DrawFoldoutHeader(string title, bool foldout, GUIStyle style = null)
        {
            if (style == null) style = toggleStyle;

            EditorGUILayout.BeginHorizontal();
            bool newFoldout = GUILayout.Button(title, style);
            EditorGUILayout.EndHorizontal();

            return foldout ^ newFoldout;
        }

        public void BeginSettingsSection()
        {
            EditorGUILayout.BeginVertical(settingsStyle);
        }

        public void EndSettingsSection()
        {
            EditorGUILayout.EndVertical();
        }

        public void BeginTitleSection()
        {
            EditorGUILayout.BeginVertical(titleStyle);
        }

        public void EndTitleSection()
        {
            EditorGUILayout.EndVertical();
        }

        public void BeginIndentedSection()
        {
            EditorGUI.indentLevel++;
        }

        public void EndIndentedSection()
        {
            EditorGUI.indentLevel--;
        }
    }
}
#endif