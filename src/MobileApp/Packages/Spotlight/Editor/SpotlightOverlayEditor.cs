using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace Sotbifay.Spotlight.Editor
{
    [ExecuteAlways]
    [CustomEditor(typeof(SpotlightOverlay))]
    public sealed class SpotlightOverlayEditor : ImageEditor
    {
        private SerializedProperty _smoothnessProperty;
        private SerializedProperty _cornerRadiusProperty;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            _smoothnessProperty = serializedObject.FindProperty("_smoothness");
            _cornerRadiusProperty = serializedObject.FindProperty("_cornerRadius");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SpriteGUI();
            AppearanceControlsGUI();
            EditorGUILayout.PropertyField(_smoothnessProperty);
            EditorGUILayout.PropertyField(_cornerRadiusProperty);
            EditorGUILayout.PropertyField(m_RaycastTarget);

            serializedObject.ApplyModifiedProperties();
        }
    }
}