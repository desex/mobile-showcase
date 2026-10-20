using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion.Animation;
using R3;
using Sotbify.MobileApp.UI.Common.Components.DynamicTextWithBackground;
using Sotbify.MobileApp.Utils;
using Sotbify.MobileApp.Utils.Enums;
using Sotbify.MobileApp.Utils.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace Sotbify.MobileApp.FeedWindow.FeedList.Core.Views
{
    public class FeedBonusView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private LitMotionAnimation _loadingAnimation;
        [SerializeField] private TMP_Text _headerText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private GameObject _promocodeRoot;
        [SerializeField] private TMP_Text _promocodeText;
        [SerializeField] private DynamicTextWithBackground _bonusType;
        [SerializeField] private Button _claimBonusButton;
        
        [Space(10)]
        [Header("Copy bonus")]
        [SerializeField] private Button _copyBonusButton;
        [SerializeField] private Image _copyPromocodeImage;
        [SerializeField] private Sprite _targetPromocodeSprite;
        [SerializeField] private Image _copyPromocodeBackground;
        [SerializeField] private CanvasGroup _promoTextCanvasGroup;
        [SerializeField] private CanvasGroup _copiedCanvasGroup;
        [SerializeField] private Color _touchColor;
        [SerializeField] private Color _targetBackgroundColor;
        [SerializeField] private Color _targetTextColor;
        [SerializeField] private MotionParameters _copyMotionParameters;
        [SerializeField] private float _copiedShowSeconds;

        private Color _initialColor;

        public int Id { get; set; }
        public string Header { set => _headerText.SetText(value); }
        public string Description { set => _descriptionText.SetText(value); }
        public EHomeItemType Type { get; set; }

        public DynamicTextWithBackground BonusType => _bonusType;
        
        public string Promocode
        {
            get => _promocodeText != null ? _promocodeText.text : string.Empty;
            set
            {
                if(_promocodeRoot != null)
                    _promocodeRoot.SetActive(string.IsNullOrEmpty(value) == false);
                
                if(_promocodeText != null)
                    _promocodeText.text = value;
            }
        }

        public string ActivationUrl { get; set; }
        
        public Observable<string> CopyBonusObservable => _copyBonusButton.OnClickAsObservable()
            .Select(_promocodeText, static (_, promocodeText) => promocodeText.text);
        
        public Observable<string> ClaimBonusObservable => _claimBonusButton.OnClickAsObservable()
            .Select(this, static (_, @this) => @this.ActivationUrl);
        
        private void Awake()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public Sprite Image
        {
            set
            {
                if(_image != null)
                    _image.sprite = value!;
            }
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
        
        public async UniTask PlayCopyAnimation(CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private async UniTask CreateCopyAnimationTask(bool isCopying, Sprite targetSprite, Color targetColor, CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}
