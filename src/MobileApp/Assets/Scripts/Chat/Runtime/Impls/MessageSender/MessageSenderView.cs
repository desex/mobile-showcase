using R3;
using Sotbify.MobileApp.Utils.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.Chat.MessageSender
{
    public class MessageSenderView : MonoBehaviour
    {
        [SerializeField] private ChatFastMessagesBar _fastMessagesBar;
        [Space]
        [SerializeField] private Button _sendButton;
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private LayoutElement _selfLayout;
        [Space]
        [SerializeField] private Color _initialTextColor;
        [SerializeField] private Color _targetTextColor;
        [SerializeField] private Color _targetBackgroundColor;
        [SerializeField] private Image _backgroundImage;
        [Space]
        [SerializeField] private float _minHeight = 54f;
        [SerializeField] private float _minActiveHeight = 78f;
        [SerializeField] private float _maxHeight = 240f;
        [SerializeField] private float _verticalPadding = 60f;

        public Observable<Unit> SendClickObservable => _sendButton.OnClickAsObservable();
        public Observable<string> FastMessageObservable => _fastMessagesBar.FastButtonPressedObservable;
        public Observable<string> MessageChangedObservable => _inputField.onValueChanged.AsObservable();

        public string Message => _inputField.text;

        public bool IsSendButtonInteractable
        {
            get => _sendButton != null && _sendButton.interactable;
            set => _sendButton.interactable = value;
        }

        private Color _initialBackgroundColor;
        private Color _initialPlaceholderColor;
        private int _previousLineCount;
        private int _previousTextLength;

        public void ClearInput()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void RefreshStateAfterTextChanged()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void Awake()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void Start()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void OnTextChanged(string text)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void SwitchColor(bool isSelected)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void RefreshState()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private float GetTargetHeight(bool hasMessage)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private float GetFullTextPreferredHeight()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void CompensateScrollOnDeletedLines(bool isDeleting, bool wasOverflowed, int previousLineCount)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private bool IsOverflowed(int lineCount)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private static float GetLineHeight(TMP_TextInfo textInfo)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void CaptureTextState()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void MarkLayoutDirty()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}