using Sotbify.MobileApp.UI.Window.View;
using System.Collections.Generic;
using LitMotion.Animation;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.ProfileCharacterChoose
{
    public class ProfileCharacterChooseView : FadeWindowView
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private Graphic _background;
        [SerializeField] private Color _defaultBackgroundColor = Color.black;
        [SerializeField] private Image _image;
        [SerializeField] private Button _previousButton;
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _chooseButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private LitMotionAnimation _loadingAnimation;

        [Header("Counter")]
        [SerializeField] private RectTransform _counterContainer;
        [SerializeField] private Graphic _counterPrefab;
        [SerializeField] private Color _initialColor = Color.gray;
        [SerializeField] private Color _targetColor = Color.white;
        private readonly List<Graphic> _counter = new();

        public Observable<Unit> PreviousPressed => _previousButton.OnClickAsObservable();
        public Observable<Unit> NextPressed => _nextButton.OnClickAsObservable();
        public Observable<Unit> CloseObservable => _closeButton.OnClickAsObservable();
        public Observable<Unit> ChoosePressed => _chooseButton.OnClickAsObservable();

        public void ShowCharacter(ProfileCharacterData character, bool hasPrevious, bool hasNext)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void UpdateCounter(int characterCount, int characterIndex)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void SetImage(Sprite sprite)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void SetImageLoading(bool loading)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}
