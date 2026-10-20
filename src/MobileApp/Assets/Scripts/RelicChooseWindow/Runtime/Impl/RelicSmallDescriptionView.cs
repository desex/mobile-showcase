using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using R3;
using Sotbify.MobileApp.Utils;
using Sotbify.MobileApp.Utils.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.RelicChooseWindow
{
    public class RelicSmallDescriptionView : MonoBehaviour
    {
        [Header("Assignation")] 
        [SerializeField] private TMP_Text _header;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private RawImage _relicPreview;
        [SerializeField] private Texture2D _previewDefaultTexture;

        [Header("Animation")] 
        [SerializeField] private int _rarity = 0;
        [SerializeField] private List<Graphic> _stars;
        [SerializeField] private Color _activeStarColor = Color.white;
        [SerializeField] private Color _inativeStarColor = new (1f, 1f, 1f, 0.5f);
        
        [Header("Animation")] 
        [SerializeField] private CanvasGroup _selfCanvasGroup;
        [SerializeField] private MotionParameters _motionParameters;
     
        [Header("Buttons")] 
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _equipButton;

        private MotionHandle _activeHandle;
        
        public bool IsActive { get; private set; }

        public string Name
        {
            get => _header != null ? _header.text : string.Empty;
            set
            {
                if(_header != null)
                    _header.text = value;
            }
        }

        public string Description
        {
            get => _description != null ? _description.text : string.Empty;
            set
            {
                if(_description != null)
                    _description.text = value;
            }
        }

        public Texture Preview
        {
            get => _relicPreview != null ? _relicPreview.texture : null;
            set
            {
                if(_relicPreview == null)
                    return;
                
                _relicPreview.texture = value != null ? value : _previewDefaultTexture;
            }
        }

        public bool EquipIsActive
        {
            get => _equipButton != null && _equipButton.interactable;
            set
            {
                if(_equipButton != null)
                    _equipButton.interactable = value;
            }
        }

        public int Rarity
        {
            get => _rarity;
            set
            {
                _rarity = value;
                UpdateRarity(value);
            }
        }

        public Observable<Unit> EquipObservable => _equipButton.OnClickAsObservable();

        public Observable<Unit> CloseObservable => _closeButton.OnClickAsObservable();

        public UniTask SetActiveAsync(bool isActive, CancellationToken cancellationToken = default)
        {
            if(_activeHandle.IsActive())
                _activeHandle.Complete();
            
            IsActive = isActive;
            _activeHandle = isActive
                ? CreateShowAnimation()
                : CreateHideAnimation();

            return _activeHandle.ToUniTask(CancelBehavior.Complete, cancellationToken);
        }

        private MotionHandle CreateShowAnimation()
        {
            gameObject.SetActive(true);
            _selfCanvasGroup.blocksRaycasts = true;
            
            return LMotion
                .Create(_selfCanvasGroup.alpha, 1f, _motionParameters.Time)
                .WithEase(_motionParameters.Ease)
                .BindToAlpha(_selfCanvasGroup);
        }
        
        private MotionHandle CreateHideAnimation()
        {
            _selfCanvasGroup.blocksRaycasts = false;
            
            return LMotion
                .Create(_selfCanvasGroup.alpha, 0f, _motionParameters.Time)
                .WithEase(_motionParameters.Ease)
                .WithOnComplete(() => gameObject.SetActive(false))
                .BindToAlpha(_selfCanvasGroup);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if(_relicPreview != null && _relicPreview.texture == null)
                _relicPreview.texture = _previewDefaultTexture;
            
            UpdateRarity(_rarity);
        }
#endif
        
        private void Awake()
        {
            if(_relicPreview != null && _relicPreview.texture == null)
                _relicPreview.texture = _previewDefaultTexture;

            UpdateRarity(_rarity);
        }

        private void UpdateRarity(int rarity)
        {
            for (var i = 0; i < _stars.Count; i++)
            {
                var star = _stars[i];
                if(star == null)
                    continue;
                
                star.color = i <= rarity - 1 ? _activeStarColor : _inativeStarColor;
            }
        }
    }
}