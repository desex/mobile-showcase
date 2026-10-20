using R3;
using R3.Triggers;
using Sotbify.MobileApp.Utils.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Stab.StabChatWindow.MessageSender
{
    public class StabMessageSenderView : MonoBehaviour
    {
        [SerializeField] private Button _sendButton;
        [SerializeField] private TMP_InputField _inputField;
        
        [SerializeField] private Color _targetBackgroundColor;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Color _targetTextAreaColor;
        
        public Observable<Unit> SendClickObservable => _sendButton.OnClickAsObservable();
        public Observable<BaseEventData> OnSelectObservable => _inputField.OnSelectAsObservable();
        public Observable<BaseEventData> OnDeselectObservable => _inputField.OnDeselectAsObservable();
        public string Message => _inputField.text;
        
        private Color _initialBackgroundColor;
        private Color _initialPlaceholderColor;

        private void Awake()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void ClearInput()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void SwitchColor(bool isSelected)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void OnEnable()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        private void OnDisable()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void SetDirty(string newValue)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}