using TMPro;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.UI;
using UnityEngine;

namespace Sotbify.MobileApp.UI.InputField.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(DragScrollInputField), true)]
    public sealed class DragScrollInputFieldEditor : SelectableEditor
    {
        private struct Foldout
        {
            public static bool FontSettings = true;
            public static bool ExtraSettings = true;
        }

        private SerializedProperty _textViewport;
        private SerializedProperty _textComponent;
        private SerializedProperty _text;
        private SerializedProperty _contentType;
        private SerializedProperty _lineType;
        private SerializedProperty _lineLimit;
        private SerializedProperty _inputType;
        private SerializedProperty _characterValidation;
        private SerializedProperty _inputValidator;
        private SerializedProperty _regexValue;
        private SerializedProperty _keyboardType;
        private SerializedProperty _characterLimit;
        private SerializedProperty _caretBlinkRate;
        private SerializedProperty _caretWidth;
        private SerializedProperty _caretColor;
        private SerializedProperty _customCaretColor;
        private SerializedProperty _selectionColor;
        private SerializedProperty _hideMobileKeyboard;
        private SerializedProperty _hideMobileInput;
        private SerializedProperty _placeholder;
        private SerializedProperty _verticalScrollbar;
        private SerializedProperty _scrollbarScrollSensitivity;
        private SerializedProperty _onValueChanged;
        private SerializedProperty _onEndEdit;
        private SerializedProperty _onSelect;
        private SerializedProperty _onDeselect;
        private SerializedProperty _readOnly;
        private SerializedProperty _richText;
        private SerializedProperty _richTextEditingAllowed;
        private SerializedProperty _resetOnDeActivation;
        private SerializedProperty _keepTextSelectionVisible;
        private SerializedProperty _restoreOriginalTextOnEscape;
        private SerializedProperty _shouldActivateOnSelect;

        private SerializedProperty _mOnFocusSelectAll;
        private SerializedProperty _mGlobalPointSize;
        private SerializedProperty _mGlobalFontAsset;

        private SerializedProperty _scrollRect;
        private SerializedProperty _textSelectionDelay;

        private AnimBool _mCustomColor;

        //TMP_InputValidator m_ValidationScript;

        protected override void OnEnable()
        {
            base.OnEnable();

            _textViewport = serializedObject.FindProperty("m_TextViewport");
            _textComponent = serializedObject.FindProperty("m_TextComponent");
            _text = serializedObject.FindProperty("m_Text");
            _contentType = serializedObject.FindProperty("m_ContentType");
            _lineType = serializedObject.FindProperty("m_LineType");
            _lineLimit = serializedObject.FindProperty("m_LineLimit");
            _inputType = serializedObject.FindProperty("m_InputType");
            _characterValidation = serializedObject.FindProperty("m_CharacterValidation");
            _inputValidator = serializedObject.FindProperty("m_InputValidator");
            _regexValue = serializedObject.FindProperty("m_RegexValue");
            _keyboardType = serializedObject.FindProperty("m_KeyboardType");
            _characterLimit = serializedObject.FindProperty("m_CharacterLimit");
            _caretBlinkRate = serializedObject.FindProperty("m_CaretBlinkRate");
            _caretWidth = serializedObject.FindProperty("m_CaretWidth");
            _caretColor = serializedObject.FindProperty("m_CaretColor");
            _customCaretColor = serializedObject.FindProperty("m_CustomCaretColor");
            _selectionColor = serializedObject.FindProperty("m_SelectionColor");

            _hideMobileKeyboard = serializedObject.FindProperty("m_HideSoftKeyboard");
            _hideMobileInput = serializedObject.FindProperty("m_HideMobileInput");

            _placeholder = serializedObject.FindProperty("m_Placeholder");
            _verticalScrollbar = serializedObject.FindProperty("m_VerticalScrollbar");
            _scrollbarScrollSensitivity = serializedObject.FindProperty("m_ScrollSensitivity");

            _onValueChanged = serializedObject.FindProperty("m_OnValueChanged");
            _onEndEdit = serializedObject.FindProperty("m_OnEndEdit");
            _onSelect = serializedObject.FindProperty("m_OnSelect");
            _onDeselect = serializedObject.FindProperty("m_OnDeselect");
            _readOnly = serializedObject.FindProperty("m_ReadOnly");
            _richText = serializedObject.FindProperty("m_RichText");
            _richTextEditingAllowed = serializedObject.FindProperty("m_isRichTextEditingAllowed");
            _resetOnDeActivation = serializedObject.FindProperty("m_ResetOnDeActivation");
            _keepTextSelectionVisible = serializedObject.FindProperty("m_KeepTextSelectionVisible");
            _restoreOriginalTextOnEscape = serializedObject.FindProperty("m_RestoreOriginalTextOnEscape");

            _mOnFocusSelectAll = serializedObject.FindProperty("m_OnFocusSelectAll");
            _shouldActivateOnSelect = serializedObject.FindProperty("m_ShouldActivateOnSelect");

            _mGlobalPointSize = serializedObject.FindProperty("m_GlobalPointSize");
            _mGlobalFontAsset = serializedObject.FindProperty("m_GlobalFontAsset");

            _mCustomColor = new AnimBool(_customCaretColor.boolValue);
            _mCustomColor.valueChanged.AddListener(Repaint);
            
            _scrollRect = serializedObject.FindProperty(nameof(_scrollRect));
            _textSelectionDelay = serializedObject.FindProperty(nameof(_textSelectionDelay));
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _mCustomColor.valueChanged.RemoveListener(Repaint);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            base.OnInspectorGUI();

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_textViewport);

            EditorGUILayout.PropertyField(_textComponent);

            TextMeshProUGUI text = null;
            if (_textComponent != null && _textComponent.objectReferenceValue != null)
            {
                text = _textComponent.objectReferenceValue as TextMeshProUGUI;
                //if (text.supportRichText)
                //{
                //    EditorGUILayout.HelpBox("Using Rich Text with input is unsupported.", MessageType.Warning);
                //}
            }

            EditorGUI.BeginDisabledGroup(_textComponent == null || _textComponent.objectReferenceValue == null);

            // TEXT INPUT BOX
            EditorGUILayout.PropertyField(_text);

            // INPUT FIELD SETTINGS

            #region INPUT FIELD SETTINGS

            Foldout.FontSettings = EditorGUILayout.Foldout(Foldout.FontSettings, "Input Field Settings", true, TMP_UIStyleManager.boldFoldout);

            if (Foldout.FontSettings)
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(_mGlobalFontAsset,
                    new GUIContent("Font Asset", "Set the Font Asset for both Placeholder and Input Field text object."));
                if (EditorGUI.EndChangeCheck())
                {
                    TMP_InputField inputField = target as TMP_InputField;
                    inputField.SetGlobalFontAsset(_mGlobalFontAsset.objectReferenceValue as TMP_FontAsset);
                }


                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(_mGlobalPointSize,
                    new GUIContent("Point Size", "Set the point size of both Placeholder and Input Field text object."));
                if (EditorGUI.EndChangeCheck())
                {
                    TMP_InputField inputField = target as TMP_InputField;
                    inputField.SetGlobalPointSize(_mGlobalPointSize.floatValue);
                }

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(_characterLimit);

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(_contentType);
                if (!_contentType.hasMultipleDifferentValues)
                {
                    EditorGUI.indentLevel++;

                    if (_contentType.enumValueIndex == (int)TMP_InputField.ContentType.Standard ||
                        _contentType.enumValueIndex == (int)TMP_InputField.ContentType.Autocorrected ||
                        _contentType.enumValueIndex == (int)TMP_InputField.ContentType.Custom)
                    {
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(_lineType);
                        if (EditorGUI.EndChangeCheck())
                        {
                            if (text != null)
                            {
                                if (_lineType.enumValueIndex == (int)TMP_InputField.LineType.SingleLine)
                                    text.textWrappingMode = TextWrappingModes.PreserveWhitespaceNoWrap;
                                else
                                {
                                    text.textWrappingMode = TextWrappingModes.Normal;
                                }
                            }
                        }

                        if (_lineType.enumValueIndex != (int)TMP_InputField.LineType.SingleLine)
                        {
                            EditorGUILayout.PropertyField(_lineLimit);
                        }
                    }

                    if (_contentType.enumValueIndex == (int)TMP_InputField.ContentType.Custom)
                    {
                        EditorGUILayout.PropertyField(_inputType);
                        EditorGUILayout.PropertyField(_keyboardType);
                        EditorGUILayout.PropertyField(_characterValidation);
                        if (_characterValidation.enumValueIndex == (int)TMP_InputField.CharacterValidation.Regex)
                        {
                            EditorGUILayout.PropertyField(_regexValue);
                        }
                        else if (_characterValidation.enumValueIndex == (int)TMP_InputField.CharacterValidation.CustomValidator)
                        {
                            EditorGUILayout.PropertyField(_inputValidator);
                        }
                    }

                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(_placeholder);
                EditorGUILayout.PropertyField(_verticalScrollbar);

                if (_verticalScrollbar.objectReferenceValue != null)
                    EditorGUILayout.PropertyField(_scrollbarScrollSensitivity);

                EditorGUILayout.PropertyField(_caretBlinkRate);
                EditorGUILayout.PropertyField(_caretWidth);

                EditorGUILayout.PropertyField(_customCaretColor);

                _mCustomColor.target = _customCaretColor.boolValue;

                if (EditorGUILayout.BeginFadeGroup(_mCustomColor.faded))
                {
                    EditorGUILayout.PropertyField(_caretColor);
                }

                EditorGUILayout.EndFadeGroup();

                EditorGUILayout.PropertyField(_selectionColor);

                EditorGUI.indentLevel--;
            }

            #endregion


            // CONTROL SETTINGS

            #region CONTROL SETTINGS

            Foldout.ExtraSettings = EditorGUILayout.Foldout(Foldout.ExtraSettings, "Control Settings", true, TMP_UIStyleManager.boldFoldout);

            if (Foldout.ExtraSettings)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(_mOnFocusSelectAll,
                    new GUIContent("OnFocus - Select All", "Should all the text be selected when the Input Field is selected?"));
                EditorGUILayout.PropertyField(_resetOnDeActivation,
                    new GUIContent("Reset On Deactivation",
                        "Should the Text and Caret position be reset when Input Field loses focus and is Deactivated?"));

                EditorGUI.indentLevel++;
                GUI.enabled = !_resetOnDeActivation.boolValue;
                EditorGUILayout.PropertyField(_keepTextSelectionVisible,
                    new GUIContent("Keep Text Selection Visible",
                        "Should the text selection remain visible when the input field loses focus and is deactivated?"));
                GUI.enabled = true;
                EditorGUI.indentLevel--;

                EditorGUILayout.PropertyField(_restoreOriginalTextOnEscape,
                    new GUIContent("Restore On ESC Key",
                        "Should the original text be restored when pressing ESC? (Property not applicable for HoloLens)"));
                EditorGUILayout.PropertyField(_shouldActivateOnSelect,
                    new GUIContent("Should Activate On Select", "Determines if the Input Field will be activated when selected."));
                EditorGUILayout.PropertyField(_hideMobileKeyboard,
                    new GUIContent("Hide Soft Keyboard", "Controls the visibility of the mobile virtual keyboard."));

                EditorGUI.BeginDisabledGroup(_hideMobileKeyboard.boolValue);
                EditorGUILayout.PropertyField(_hideMobileInput,
                    new GUIContent("Hide Mobile Input", "Controls the visibility of the editable text field above the mobile virtual keyboard."));
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.PropertyField(_readOnly);
                EditorGUILayout.PropertyField(_richText);
                EditorGUILayout.PropertyField(_richTextEditingAllowed, new GUIContent("Allow Rich Text Editing"));

                EditorGUI.indentLevel--;
            }

            #endregion


            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_onValueChanged);
            EditorGUILayout.PropertyField(_onEndEdit);
            EditorGUILayout.PropertyField(_onSelect);
            EditorGUILayout.PropertyField(_onDeselect);

            EditorGUI.EndDisabledGroup();
            
            EditorGUILayout.PropertyField(_scrollRect);
            EditorGUILayout.PropertyField(_textSelectionDelay);

            serializedObject.ApplyModifiedProperties();
        }
    }
}