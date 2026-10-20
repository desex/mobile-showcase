using LitMotion.Animation;
using R3;
using Sotbify.MobileApp.UI.Window.View;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace Sotbify.MobileApp.ProfileWindow.Core
{
    public class ProfileWindowView : FadeWindowView
    {
        [SerializeField] private TMP_Text _profileName;
        [SerializeField] private TMP_Text _characterName;
        [SerializeField] private Image _profileImage;
        [SerializeField] private Graphic _profileBackground;
        [SerializeField] private LitMotionAnimation _loadingAnimation;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _editButton;
        [SerializeField] private Button _passwordLoginButton;
        [Space]
        [SerializeField] private TMP_Text _passwordStatusText;
        [SerializeField] private LocalizedString _enabledPasswordLocalizedString;
        [SerializeField] private LocalizedString _disabledPasswordLocalizedString;
        
        public Sprite ProfileImage
        {
            set
            {
                _profileImage.sprite = value;
                _profileImage.preserveAspect = true;
            }
        }

        public Color ProfileBackgroundColor { set => _profileBackground.color = value; }
        public string ProfileName { set => _profileName.text = value; }
        public string CharacterName { set => _characterName.text = value; }
        public Observable<Unit> CloseObservable => _closeButton.OnClickAsObservable();
        public Observable<Unit> EditObservable => _editButton.OnClickAsObservable();
        public Observable<Unit> PasswordLoginObservable => _passwordLoginButton.OnClickAsObservable();

        public void SetPasswordStatus(bool isEnabled)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void ChangeLoadStatus(bool isLoading)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        private void StartLoadingAnimation()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void StopLoadingAnimation()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}
