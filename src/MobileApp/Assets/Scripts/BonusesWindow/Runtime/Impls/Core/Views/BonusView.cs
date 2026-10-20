using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Animation;
using R3;
using Sotbify.MobileApp.BonusesWindow.BonusItem;
using Sotbify.MobileApp.UI.Common.Components.DynamicTextWithBackground;
using Sotbify.MobileApp.Utils;
using Sotbify.MobileApp.Utils.Enums;
using Sotbify.MobileApp.Utils.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace Sotbify.MobileApp.BonusesWindow.Core.Views
{
    public class BonusView : MonoBehaviour
    {
        private readonly Subject<float> _heightChanged = new();
        
        [SerializeField] private TMP_Text _bonusTitle;
        [SerializeField] private TMP_Text _description;

        [SerializeField] private TMP_Text _timer;
        [SerializeField] private RectTransform _additionalContent;
        [SerializeField] private LitMotionAnimation _loadingAnimation;
        [SerializeField] private RectTransform _linesRootTransform;
        
        [SerializeField] private LayoutElement _layoutElement;
        [SerializeField] private BonusItemLine _linePrefab;
        
        [SerializeField] private Image _logoImage;
        [SerializeField] private AspectRatioFitter _logoAspect;
        [SerializeField] private Graphic _copyOutline;
        [SerializeField] private Image _buttonAccentBackground;
        [SerializeField] private BonusItemLine _rulesAccent;
        [SerializeField] private DynamicTextWithBackground _bonusType;
        [SerializeField] private float _radialMotionOffset;

        [field: Space(10)]
        [field: Header("Animations")]
        [field: SerializeField] public float ClosedHeight { get; private set; } = 140f;

        [SerializeField] private MotionParameters _openMotionParameters;
        
        [Space(10)]
        [Header("Copy promocode")]
        [SerializeField] private Image _copyPromocodeImage;
        [SerializeField] private Sprite _targetPromocodeSprite;
        [SerializeField] private Image _copyPromocodeBackground;
        [SerializeField] private GameObject _promocodeRoot;
        [SerializeField] private TMP_Text _promocode;
        [SerializeField] private CanvasGroup _promoTextCanvasGroup;
        [SerializeField] private CanvasGroup _copiedCanvasGroup;
        [SerializeField] private Color _touchColor;
        [SerializeField] private Color _targetBackgroundColor;
        [SerializeField] private Color _targetTextColor;
        [SerializeField] private MotionParameters _copyMotionParameters;
        [SerializeField] private float _copiedShowSeconds;
        
        [SerializeField] private OpenButton _openButton;
        [SerializeField] private Button _copyButton;
        [SerializeField] private Button _activateButton;
        [SerializeField] private Button _openRulesButton;
        
        private TimeSpan _duration;
        private Color _initialColor;
        private Color _initialOutlineColor;
        private Vector2 _initialRadialAnchoredPosition;
        private Color _accentColor;

        public Observable<Unit> OpenButtonClick => _openButton.OnButtonClick;
        public Observable<Unit> CopyButtonClick => _copyButton.OnClickAsObservable();
        public Observable<Unit> ActivateButtonClick => _activateButton.OnClickAsObservable();
        public Observable<Unit> OpenRulesButtonClick => _openRulesButton.OnClickAsObservable();
        public Observable<float> HeightChanged => _heightChanged;

        public float OpenedHeight { get; private set; }

        public int Id { get; set; }

        public string Description
        {
            get => _description.text;
            set => _description.text = value;
        }

        public string Title
        {
            get => _bonusTitle.text;
            set => _bonusTitle.text = value;
        }

        public Sprite Image
        {
            get => _logoImage.sprite;
            set => _logoImage.sprite = value;
        }

        public Color BonusTypeColor
        {
            get => _bonusType.Color;
            set => _bonusType.Color = value;
        }

        public string Promocode
        {
            get => _promocode.text;
            set
            {
                if(_promocodeRoot != null)
                    _promocodeRoot.SetActive(string.IsNullOrEmpty(value) == false);
                
                _promocode.text = value;
            }
        }

        public EBonusType Type { get; set; }

        public string BonusType
        {
            get => _bonusType.Message;
            set => _bonusType.Message = value;
        }

        public DynamicTextWithBackground TypeText => _bonusType;

        public Color AccentColor
        {
            set
            {
                _accentColor = value;
                _buttonAccentBackground.color = value;
                _rulesAccent.Color = value;
            }
        }

        public float ImageAspectRatio
        {
            get => _logoAspect.aspectRatio;
            set => _logoAspect.aspectRatio = value;
        }

        public RectTransform LinesRootTransform => _linesRootTransform;
        public RectTransform AdditionalContent => _additionalContent;
        public BonusItemLine LinePrefab => _linePrefab;

        public TimeSpan Duration
        {
            get => _duration;
            set
            {
                _duration = value;
                _timer.text = value.HigherTimeToString();
            }
        }

        private void Awake()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public async UniTask StartTimer(CancellationToken token)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public async UniTask OpenAsync(CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        public void SetOpenedImmediate()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public async UniTask CloseAsync(CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void SetClosedImmediate()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public async UniTask PlayCopyAnimation(CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        public void CalculateOpenedHeight()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

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

        public void StopLoadingAnimation()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void SetHeight(float height)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private async UniTask CreateCopyAnimationTask(bool isCopying, Sprite targetSprite, Color targetBackgroundColor, CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void OnDestroy()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}