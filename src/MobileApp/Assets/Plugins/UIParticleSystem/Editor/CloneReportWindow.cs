#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TierStudios.UIParticleSystem.Editor
{
    public class CloneReportWindow : EditorWindow
    {
        private const string PREF_KEY = "UIParticleSystem_ShowCloneReport";

        private string sourceName;
        private string targetName;
        private List<ReportEntry> entries = new List<ReportEntry>();
        private Vector2 scrollPosition;
        private GUIStyle headerStyle;
        private GUIStyle successStyle;
        private GUIStyle warningStyle;
        private GUIStyle skippedStyle;
        private GUIStyle sectionStyle;
        private GUIStyle detailStyle;
        private bool stylesInitialized;

        public enum EntryType
        {
            Success,
            Warning,
            Skipped
        }

        public struct ReportEntry
        {
            public string module;
            public string detail;
            public EntryType type;
        }

        public static bool IsEnabled
        {
            get => EditorPrefs.GetBool(PREF_KEY, true);
            set => EditorPrefs.SetBool(PREF_KEY, value);
        }

        [MenuItem("Tools/TierStudios/UI Particle System/Show Clone Report", false, 200)]
        private static void ToggleShowReport()
        {
            IsEnabled = !IsEnabled;
        }

        [MenuItem("Tools/UI Particle System/Show Clone Report", true)]
        private static bool ToggleShowReportValidate()
        {
            Menu.SetChecked("Tools/UI Particle System/Show Clone Report", IsEnabled);
            return true;
        }

        public static CloneReportWindow Show(string sourceName, string targetName, List<ReportEntry> entries)
        {
            if (!IsEnabled) return null;

            var window = GetWindow<CloneReportWindow>(true, "UI Particle System — Clone Report", true);
            window.sourceName = sourceName;
            window.targetName = targetName;
            window.entries = entries ?? new List<ReportEntry>();
            window.minSize = new Vector2(480, 360);
            window.maxSize = new Vector2(600, 800);
            window.ShowUtility();
            return window;
        }

        private void InitStyles()
        {
            if (stylesInitialized) return;

            headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(0, 0, 8, 8)
            };

            successStyle = new GUIStyle(EditorStyles.label)
            {
                richText = true,
                fontSize = 11,
                padding = new RectOffset(4, 4, 2, 2)
            };

            warningStyle = new GUIStyle(successStyle);
            skippedStyle = new GUIStyle(successStyle);

            sectionStyle = new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(8, 8, 6, 6),
                margin = new RectOffset(4, 4, 2, 2)
            };

            detailStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                richText = true,
                wordWrap = true,
                fontSize = 10,
                padding = new RectOffset(20, 4, 0, 2)
            };

            stylesInitialized = true;
        }

        private void OnGUI()
        {
            InitStyles();

            EditorGUILayout.Space(8);

            var titleRect = EditorGUILayout.GetControlRect(false, 30);
            EditorGUI.LabelField(titleRect, "Clone Report", headerStyle);

            Color lineColor = EditorGUIUtility.isProSkin
                ? new Color(0.3f, 0.6f, 1f, 0.5f)
                : new Color(0.1f, 0.4f, 0.8f, 0.5f);
            Rect lineRect = EditorGUILayout.GetControlRect(false, 2);
            EditorGUI.DrawRect(lineRect, lineColor);

            EditorGUILayout.Space(4);

            EditorGUILayout.BeginVertical(sectionStyle);
            EditorGUILayout.LabelField($"Source:  {sourceName}", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Target:  {targetName}", EditorStyles.boldLabel);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(4);

            int successCount = 0, warningCount = 0, skippedCount = 0;
            foreach (var e in entries)
            {
                switch (e.type)
                {
                    case EntryType.Success: successCount++; break;
                    case EntryType.Warning: warningCount++; break;
                    case EntryType.Skipped: skippedCount++; break;
                }
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            DrawBadge($"✓ {successCount} Cloned", new Color(0.2f, 0.7f, 0.2f));
            GUILayout.Space(8);
            DrawBadge($"⚠ {warningCount} Warnings", new Color(1f, 0.65f, 0f));
            GUILayout.Space(8);
            DrawBadge($"✗ {skippedCount} Skipped", new Color(0.8f, 0.3f, 0.3f));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(6);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            foreach (var entry in entries)
            {
                DrawEntry(entry);
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(4);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(8);

            bool dontShow = !IsEnabled;
            bool newDontShow = EditorGUILayout.ToggleLeft("Don't show again", dontShow, GUILayout.Width(140));
            if (newDontShow != dontShow)
            {
                IsEnabled = !newDontShow;
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Close", GUILayout.Width(120), GUILayout.Height(28)))
            {
                Close();
            }

            GUILayout.Space(8);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(4);
        }

        private void DrawBadge(string text, Color color)
        {
            var style = new GUIStyle(EditorStyles.miniLabel)
            {
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = color }
            };
            GUILayout.Label(text, style);
        }

        private void DrawEntry(ReportEntry entry)
        {
            Color bgColor;
            string icon;

            switch (entry.type)
            {
                case EntryType.Success:
                    bgColor = new Color(0.2f, 0.7f, 0.2f, 0.08f);
                    icon = "✓";
                    break;
                case EntryType.Warning:
                    bgColor = new Color(1f, 0.65f, 0f, 0.08f);
                    icon = "⚠";
                    break;
                case EntryType.Skipped:
                    bgColor = new Color(0.8f, 0.3f, 0.3f, 0.08f);
                    icon = "✗";
                    break;
                default:
                    bgColor = Color.clear;
                    icon = "•";
                    break;
            }

            Rect rect = EditorGUILayout.BeginVertical(sectionStyle);
            EditorGUI.DrawRect(rect, bgColor);

            Color textColor;
            switch (entry.type)
            {
                case EntryType.Success: textColor = new Color(0.3f, 0.85f, 0.3f); break;
                case EntryType.Warning: textColor = new Color(1f, 0.7f, 0.2f); break;
                case EntryType.Skipped: textColor = new Color(1f, 0.4f, 0.4f); break;
                default: textColor = Color.white; break;
            }

            string colorHex = ColorUtility.ToHtmlStringRGB(textColor);
            EditorGUILayout.LabelField($"<color=#{colorHex}>{icon}</color>  <b>{entry.module}</b>", successStyle);

            if (!string.IsNullOrEmpty(entry.detail))
            {
                EditorGUILayout.LabelField($"<color=#888888>{entry.detail}</color>", detailStyle);
            }

            EditorGUILayout.EndVertical();
        }
    }
}
#endif