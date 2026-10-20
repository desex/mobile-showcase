using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using R3;
using Sirenix.OdinInspector;
using Sotbify.MobileApp.UI.Layout;
using Sotbify.MobileApp.UI.Window.View;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.NewRelicWindow
{
    public sealed class NewRelicWindowView : FadeWindowView
    {
        private const string ControlsAnimationGroup = "Controls Animation";
        private const string ButtonsGroup = "Buttons";

        [SerializeField] private GameObject _mainRoot;
        [SerializeField] private Animator _mainAnimator;
        [SerializeField] private float _showAnimationDuration = 1f;
        [SerializeField] private string _openTriggerName = "Open";
        [SerializeField] private float _openAnimationDuration = 1f;

        [BoxGroup(ControlsAnimationGroup)] 
        [SerializeField] private CanvasGroup _controlsAnimationCanvasGroup;
        [BoxGroup(ControlsAnimationGroup)] 
        [SerializeField] private LerpLayoutElement _controlsAnimationLerpLayoutElement;
        [BoxGroup(ControlsAnimationGroup)] 
        [SerializeField] private float _controlsAnimationAlphaDuration = 1f;
        [BoxGroup(ControlsAnimationGroup)]
        [SerializeField] private float _controlsAnimationLerpDuration = 1f;
        [BoxGroup(ControlsAnimationGroup)]
        [SerializeField] private Ease _controlsAnimationAlphaEase;
        [BoxGroup(ControlsAnimationGroup)]
        [SerializeField] private Ease _controlsAnimationLerpEase;

        [SerializeField] private RawImage _itemRawImage;
        [SerializeField] private TMP_Text _itemNameText;
        [SerializeField] private TMP_Text _itemDescriptionText;
        [SerializeField] private int _rarity = 0;
        [SerializeField] private List<Graphic> _itemRarityItems;
        [SerializeField] private Color _activeRarityItemColor = Color.white;
        [SerializeField] private Color _inativeRarityItemColor = new (1f, 1f, 1f, 0.5f);

        [BoxGroup(ButtonsGroup)] 
        [SerializeField] private GameObject _openButtonRoot;
        [BoxGroup(ButtonsGroup)] 
        [SerializeField] private Button _openButton;
        [BoxGroup(ButtonsGroup)] 
        [SerializeField] private GameObject _continueButtonRoot;
        [BoxGroup(ButtonsGroup)] 
        [SerializeField] private Button _continueButton;

        private MotionHandle _controlsAnimationMotionHandle;

        public Texture ItemTexture
        {
            get => _itemRawImage != null ? _itemRawImage.texture : null;
            set
            {
                if (_itemRawImage != null)
                    _itemRawImage.texture = value;
            }
        }

        public string ItemName
        {
            get => _itemNameText != null ? _itemNameText.text : null;
            set
            {
                if(_itemNameText != null)
                    _itemNameText.text = value;
            }
        }
        
        public string ItemDescription
        {
            get => _itemDescriptionText != null ? _itemDescriptionText.text : null;
            set
            {
                if(_itemDescriptionText != null)
                    _itemDescriptionText.text = value;
            }
        }

        public int ItemRarity
        {
            get => _rarity;
            set
            {
                _rarity = value;
                UpdateRarityItems(value);
            }
        }

        public Observable<Unit> OpenObservable => _openButton != null 
            ? _openButton.OnClickAsObservable()
            : Observable.Empty<Unit>();
        
        public Observable<Unit> ContinueObservable => _continueButton != null 
            ? _continueButton.OnClickAsObservable()
            : Observable.Empty<Unit>();

        [Button]
        public async UniTask OpenAsync(CancellationToken cancellationToken = default)
        {
            using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, destroyCancellationToken);
            try
            {
                await SetShowControlsWithAnimationAsync(false, cancellationTokenSource.Token);

                if (_mainAnimator != null)
                    _mainAnimator.SetTrigger(_openTriggerName);

                await UniTask.Delay(TimeSpan.FromSeconds(_openAnimationDuration), cancellationToken: cancellationTokenSource.Token);

                MakeContinueButtonIsActive();
                await SetShowControlsWithAnimationAsync(true, cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                if (_mainAnimator != null) 
                    _mainAnimator.ResetTrigger(_openTriggerName);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            UpdateRarityItems(_rarity);
        }
#endif

        protected override void OnAwakeInternal()
        {
            base.OnAwakeInternal();
            UpdateRarityItems(_rarity);
        }

        [Button]
        protected override async UniTask ShowInternalAsync(CancellationToken cancellationToken)
        {
            if (_mainRoot != null)
                _mainRoot.SetActive(false);

            if (_controlsAnimationCanvasGroup != null)
            {
                _controlsAnimationCanvasGroup.alpha = 0f;
                _controlsAnimationCanvasGroup.blocksRaycasts = false;
                _controlsAnimationCanvasGroup.interactable = false;
            }

            if (_controlsAnimationLerpLayoutElement != null)
                _controlsAnimationLerpLayoutElement.Time = 0f;

            MakeOpenButtonIsActive();

            await base.ShowInternalAsync(cancellationToken);

            if (_mainRoot != null)
                _mainRoot.SetActive(true);

            await UniTask.Delay(TimeSpan.FromSeconds(_showAnimationDuration), cancellationToken: cancellationToken);
            await SetShowControlsWithAnimationAsync(true, cancellationToken);
        }

        [Button]
        protected override async UniTask HideInternalAsync(CancellationToken cancellationToken)
        {
            await SetShowControlsWithAnimationAsync(false, cancellationToken);
            await base.HideInternalAsync(cancellationToken);
        }
        
        private void UpdateRarityItems(int rarity)
        {
            for (var i = 0; i < _itemRarityItems.Count; i++)
            {
                var rarityItem = _itemRarityItems[i];
                if(rarityItem == null)
                    continue;
                
                rarityItem.color = i <= rarity - 1 ? _activeRarityItemColor : _inativeRarityItemColor;
            }
        }

        private void MakeOpenButtonIsActive()
        {
            if(_openButtonRoot != null)
                _openButtonRoot.SetActive(true);
            
            if(_continueButtonRoot != null)
                _continueButtonRoot.SetActive(false);
        }
        
        private void MakeContinueButtonIsActive()
        {
            if(_openButtonRoot != null)
                _openButtonRoot.SetActive(false);
            
            if(_continueButtonRoot != null)
                _continueButtonRoot.SetActive(true);
        }

        private async UniTask SetShowControlsWithAnimationAsync(bool show, CancellationToken cancellationToken)
        {
            if (_controlsAnimationMotionHandle.IsActive())
                _controlsAnimationMotionHandle.Cancel();
            
            if (show == false && _controlsAnimationCanvasGroup != null)
            {
                _controlsAnimationCanvasGroup.blocksRaycasts = false; 
                _controlsAnimationCanvasGroup.interactable = false;
            }

            _controlsAnimationMotionHandle = show ? CreateShowAnimation() : CreateHideAnimation();
            await _controlsAnimationMotionHandle.ToUniTask(cancellationToken);
                
            if (show && _controlsAnimationCanvasGroup != null)
            {
                _controlsAnimationCanvasGroup.blocksRaycasts = true; 
                _controlsAnimationCanvasGroup.interactable = true;
            }
        }

        private MotionHandle CreateShowAnimation()
        {
            var sequenceBuilder = LSequence.Create();

            if (_controlsAnimationLerpLayoutElement != null)
            {
                var lerpLayoutElementMotionHandle = LMotion.Create(_controlsAnimationLerpLayoutElement.Time, 1f, _controlsAnimationLerpDuration)
                    .WithEase(_controlsAnimationLerpEase)
                    .Bind(_controlsAnimationLerpLayoutElement, static (time, lerpLayoutElement) => lerpLayoutElement.Time = time)
                    .AddTo(_controlsAnimationLerpLayoutElement);

                sequenceBuilder.Append(lerpLayoutElementMotionHandle);
            }

            if (_controlsAnimationCanvasGroup != null)
            {
                var canvasGroupMotionHandle = LMotion.Create(_controlsAnimationCanvasGroup.alpha, 1f, _controlsAnimationAlphaDuration)
                    .WithEase(_controlsAnimationAlphaEase)
                    .BindToAlpha(_controlsAnimationCanvasGroup)
                    .AddTo(_controlsAnimationCanvasGroup);

                sequenceBuilder.Append(canvasGroupMotionHandle);
            }

            return sequenceBuilder.Run();
        }

        private MotionHandle CreateHideAnimation()
        {
            var sequenceBuilder = LSequence.Create();

            if (_controlsAnimationCanvasGroup != null)
            {
                var canvasGroupMotionHandle = LMotion.Create(_controlsAnimationCanvasGroup.alpha, 0f, _controlsAnimationAlphaDuration)
                    .WithEase(_controlsAnimationAlphaEase)
                    .BindToAlpha(_controlsAnimationCanvasGroup)
                    .AddTo(_controlsAnimationLerpLayoutElement);

                sequenceBuilder.Append(canvasGroupMotionHandle);
            }

            if (_controlsAnimationLerpLayoutElement != null)
            {
                var lerpLayoutElementMotionHandle = LMotion.Create(_controlsAnimationLerpLayoutElement.Time, 0f, _controlsAnimationLerpDuration)
                    .WithEase(_controlsAnimationLerpEase)
                    .Bind(_controlsAnimationLerpLayoutElement, static (time, lerpLayoutElement) => lerpLayoutElement.Time = time)
                    .AddTo(_controlsAnimationLerpLayoutElement);

                sequenceBuilder.Append(lerpLayoutElementMotionHandle);
            }

            return sequenceBuilder.Run();
        }
    }
}