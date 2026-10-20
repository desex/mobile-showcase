#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Sotbify.MobileApp.Utils.UDateTime.Editor
{
    [CustomPropertyDrawer(typeof(UDateTime))]
    public class UDateTimeDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            var amountRect = new Rect(position.x, position.y, position.width, position.height);
            EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("_dateTime"), GUIContent.none);
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}
#endif