using System;
using LitMotion.Animation;
using R3;
using Sotbify.MobileApp.Localization;
using Sotbify.MobileApp.UI.Common.Components;
using Sotbify.MobileApp.UI.Common.Components.DynamicTextWithBackground;
using Sotbify.MobileApp.Utils.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace Sotbify.MobileApp.FeedWindow.FeedList.Core.Views
{
    public class PostView : MonoBehaviour
    {
        [SerializeField] private Graphic _typeBackground;
        [SerializeField] private Graphic _statusBackground;
        [SerializeField] private Graphic _background;
        [SerializeField] private SpriteWithLoader _image;
        [SerializeField] private Image _colorImage;
        [SerializeField] private LitMotionAnimation _loadingAnimation;
        [SerializeField] private PostButtonView _postButton;
        [SerializeField] private TMP_Text _headerText;
        [SerializeField] private Graphic _timeClock;
        [SerializeField] private TMP_Text _contentText;
        [SerializeField] private DynamicTextWithBackground _typeText;
        [SerializeField] private Button _selfButton;
        
        [Header("Created At")]
        [SerializeField] private LocalizationVariableEntry<DateTimeVariable> _createdAtVariableEntry;
        [SerializeField] private Graphic _createdAtTextGraphic;
        
        [Header("Status")]
        [SerializeField] private DynamicTextWithBackground _statusText;
        [SerializeField] private LocalizedString _readLocalizedString;
        [SerializeField] private LocalizedString _unreadLocalizedString;

        [Header("Colors if post is colored")]
        [SerializeField] private Color _targetButtonColor;
        [SerializeField] private Color _targetTypeBackgroundColor;
        [SerializeField] private Color _targetButtonTextColor;        
        [SerializeField] private Color _targetStatusTextColor;
        [SerializeField] private Color _targetTextColor;
        [SerializeField] private Color _targetTimeColor;
        [SerializeField] private Color _targetStatusBackgroundColor;

        [Header("Initial Colors")]
        [SerializeField] private Color _initialTypeBackgroundColor;
        [SerializeField] private Color _initialButtonColor;
        [SerializeField] private Color _initialButtonTextColor;
        [SerializeField] private Color _initialStatusTextColor;
        [SerializeField] private Color _initialTextColor;
        [SerializeField] private Color _initialTimeColor;
        [SerializeField] private Color _initialBackgroundColor;
        [SerializeField] private Color _initialTypeTextColor;
        [SerializeField] private Color _initialStatusBackgroundColor;

        private bool _isRead;
        private Color _initialColor;
        private DateTime _createdAt;

        public int Id { get; set; }

        public string Header
        {
            get => _headerText.text;
            set => _headerText.SetText(value);
        }

        public string Content
        {
            get => _contentText.text;
            set => _contentText.SetText(value);
        }

        public string ButtonText
        {
            get => _postButton.ButtonText;
            set => _postButton.ButtonText = value;
        }

        public string ButtonHref
        {
            get => _postButton.ButtonHref;
            set => _postButton.ButtonHref = value;
        }

        public string TextedType
        {
            get => _typeText.Message;
            set => _typeText.Message = value;
        }

        public bool IsRead
        {
            get => _isRead;
            set
            {
                _isRead = value;
                UpdateStatusText(value);
            }
        }

        public DateTime CreatedAt
        {
            get => _createdAtVariableEntry?.Variable?.Value ?? DateTime.MinValue;
            set
            {
                if(_createdAtVariableEntry?.Variable is null)
                    return;
                
                _createdAtVariableEntry.Variable.Value = value;
            }
        }

        public EHomeItemType Type { get; set; }
        
        public DynamicTextWithBackground TypeText => _typeText;
        
        public DynamicTextWithBackground StatusText => _statusText;

        public Sprite Image
        {
            get => _image != null ? _image.Sprite : null;
            set
            {
                if(_image != null)
                    _image.Sprite = value!;
            }
        }

        public Observable<int> OpenItemContent => _selfButton.OnClickAsObservable()
            .Select(this, static (_, @this) => @this.Id);

        public Observable<string> OpenPostHref => _postButton.OnClick
            .Select(_postButton, static (_, postButton) => postButton.ButtonHref ?? string.Empty);

        public void ClearImage()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void StartLoadingAnimation()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void ChangeHrefButtonState(bool shouldShow)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void StopLoadingAnimation()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void ChangeColor(Color color)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void ResetColor()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void Awake()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void UpdateStatusText(bool isRead)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}
