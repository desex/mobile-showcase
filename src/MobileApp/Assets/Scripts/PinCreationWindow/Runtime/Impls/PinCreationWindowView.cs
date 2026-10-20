using R3;
using Sotbify.MobileApp.UI.Window.View;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.PinCreationWindow
{
    public class PinCreationWindowView : FadeWindowView
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private TMP_InputField _pinInput;
        [SerializeField] private Button _nextButton;
        [SerializeField] private TMP_Text[] _pinDigits;
        [SerializeField] private GameObject[] _activeViews;
        [SerializeField] private GameObject _creationHeader;
        [SerializeField] private GameObject _creationDescription;
        [SerializeField] private GameObject _changingHeader;
        [SerializeField] private GameObject _changingDescription;

        public Observable<Unit> BackObservable => _backButton.OnClickAsObservable();
        public Observable<Unit> NextObservable => _nextButton.OnClickAsObservable();
        public Observable<string> PinChangedObservable => _pinInput.onValueChanged.AsObservable();
        public string Password => _pinInput.text;
        public bool CanContinue { set => _nextButton.interactable = value; }

        public void SetInputEnabled(bool isEnabled)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void SetChangingMode(bool isChangingPin)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override void OnAwakeInternal()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void SetPin(string pin)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void FocusInput()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}
