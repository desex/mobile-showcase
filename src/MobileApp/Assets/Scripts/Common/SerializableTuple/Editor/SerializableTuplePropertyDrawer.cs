using UnityEditor;
using UnityEngine;

namespace Sotbify.MobileApp.Common.SerializableTuple.Editor
{
    [CustomPropertyDrawer(typeof(SerializableTuple<,>))]
    public class SerializableTuplePropertyDrawer : PropertyDrawer
    {
        private const string Item1FieldName = "_item1";
        private const string Item2FieldName = "_item2";
        
        private static readonly GUIContent Item1Label = new GUIContent("Item 1");
        private static readonly GUIContent Item2Label = new GUIContent("Item 2");

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var item1Prop = property.FindPropertyRelative(Item1FieldName);
            var item2Prop = property.FindPropertyRelative(Item2FieldName);

            // Draw header with foldout
            Rect headerRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(headerRect, property.isExpanded, label, true);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                float yPosition = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                // Draw Item 1
                if (item1Prop != null)
                {
                    Rect item1Rect = new Rect(position.x, yPosition, position.width, GetPropertyHeightSafe(item1Prop));
                    EditorGUI.PropertyField(item1Rect, item1Prop, Item1Label, true);
                    yPosition += GetPropertyHeightSafe(item1Prop) + EditorGUIUtility.standardVerticalSpacing;
                }

                // Draw Item 2
                if (item2Prop != null)
                {
                    Rect item2Rect = new Rect(position.x, yPosition, position.width, GetPropertyHeightSafe(item2Prop));
                    EditorGUI.PropertyField(item2Rect, item2Prop, Item2Label, true);
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;

            if (property.isExpanded)
            {
                var item1Prop = property.FindPropertyRelative(Item1FieldName);
                var item2Prop = property.FindPropertyRelative(Item2FieldName);

                height += EditorGUIUtility.standardVerticalSpacing;
                
                if (item1Prop != null)
                {
                    height += GetPropertyHeightSafe(item1Prop);
                }
                else
                {
                    height += EditorGUIUtility.singleLineHeight;
                }
                
                height += EditorGUIUtility.standardVerticalSpacing;
                
                if (item2Prop != null)
                {
                    height += GetPropertyHeightSafe(item2Prop);
                }
                else
                {
                    height += EditorGUIUtility.singleLineHeight;
                }
            }

            return height;
        }

        private float GetPropertyHeightSafe(SerializedProperty property)
        {
            try
            {
                return EditorGUI.GetPropertyHeight(property, true);
            }
            catch
            {
                return EditorGUIUtility.singleLineHeight;
            }
        }
    }
}