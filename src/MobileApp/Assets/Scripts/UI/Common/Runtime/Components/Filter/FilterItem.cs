using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using R3;
using Sirenix.OdinInspector;
using Sotbify.MobileApp.Utils;
using Sotbify.MobileApp.Utils.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.UI.Common.Components.Filter
{
    public class FilterItem<T> : MonoBehaviour
        where T : Enum
    {
        private const string ColorTab = "Colors";
        
        private readonly Subject<T> _filterActivated = new();

        [SerializeField] private Graphic _backgroundGraphic;
        [SerializeField] private Graphic _cancelImageGraphic;
        [SerializeField] private TMP_Text _filterText;
        [Space]
        [SerializeField] private TMP_Text _itemCountText;
        [SerializeField] private string _itemCountFormat = "{0}";
        [Space]
        [SerializeField] private CanvasGroup _itemCountCanvasGroup;
        [SerializeField] private Button _selfButton;

        [SerializeField, TabGroup(ColorTab)] private Color _backgroundActiveColor;
        [SerializeField, TabGroup(ColorTab)] private Color _backgroundDisabledColor;
        [SerializeField, TabGroup(ColorTab)] private Color _textDisabledColor;
        [SerializeField, TabGroup(ColorTab)] private Color _textActiveColor;

        [SerializeField] private MotionParameters _stateMotionParameters;

        private int _count;
        private bool _isActivated;
        private MotionHandle _animationHandle;
        private bool _isInitialized;

        public string Name
        {
            get => _filterText != null ? _filterText.text : string.Empty;
            set
            {
                if (_filterText != null)
                    _filterText.text = value;
            }
        }

        public int Count
        {
            get => _count;
            set
            {
                _count = value;
                if(_itemCountText != null)
                    _itemCountText.SetText(_itemCountFormat, value);
            }
        }

        public bool IsActivated
        {
            get => _isActivated;
            set
            {
                _isActivated = value; 
                
                if(_isInitialized)
                    PlayActivateAnimation(value);
            }
        }

        public T Value { get; set; }

        public Observable<Unit> FilterChosen => _selfButton.OnClickAsObservable();
        public Observable<T> ActivatedFilter => _filterActivated;

        private void Awake()
        {
            if(_itemCountText != null)
                _itemCountText.SetText(_itemCountFormat, _count);

            ActivateWithoutAnimation(_isActivated);
            _isInitialized = true;
        }

        private void ActivateWithoutAnimation(bool isActivated)
        {
            if(_animationHandle.IsActive())
                _animationHandle.Cancel();
            
            _backgroundGraphic.color = isActivated ? _backgroundActiveColor : _backgroundDisabledColor;
            _itemCountCanvasGroup.alpha = isActivated ? 0f : 1f;
            _filterText.color = isActivated ? _textActiveColor : _textDisabledColor;
            _cancelImageGraphic.color = isActivated ? _textActiveColor : _cancelImageGraphic.color.ChangeAlpha(0f);
        }

        private void PlayActivateAnimation(bool isActivated)
        {
            if(_animationHandle.IsActive())
                _animationHandle.Cancel();
            
            _filterActivated.OnNext(isActivated ? Value : default);

            var targetBackgroundColor = isActivated ? _backgroundActiveColor : _backgroundDisabledColor;
            var targetCounterOpacity = isActivated ? 0f : 1f;
            var targetTextColor = isActivated ? _textActiveColor : _textDisabledColor;
            var targetCancelColor = isActivated ? _textActiveColor : _cancelImageGraphic.color.ChangeAlpha(0f);

            var sequenceBuilder = LSequence.Create();
            
            var backgroundColorHandle = LMotion.Create(_backgroundGraphic.color, targetBackgroundColor, _stateMotionParameters.Time)
                .WithEase(_stateMotionParameters.Ease)
                .BindToColor(_backgroundGraphic);
            
            var counterOpacityHandle = LMotion.Create(_itemCountCanvasGroup.alpha, targetCounterOpacity, _stateMotionParameters.Time)
                .WithEase(_stateMotionParameters.Ease)
                .BindToAlpha(_itemCountCanvasGroup);
            
            var textColorHandle = LMotion.Create(_filterText.color, targetTextColor, _stateMotionParameters.Time)
                .WithEase(_stateMotionParameters.Ease)
                .BindToColor(_filterText);
            
            var cancelOpacityHandle = LMotion.Create(_cancelImageGraphic.color, targetCancelColor, _stateMotionParameters.Time)
                .WithEase(_stateMotionParameters.Ease)
                .BindToColor(_cancelImageGraphic);

            sequenceBuilder.Join(backgroundColorHandle);
            sequenceBuilder.Join(counterOpacityHandle);
            sequenceBuilder.Join(textColorHandle);
            sequenceBuilder.Join(cancelOpacityHandle);

            _animationHandle = sequenceBuilder.Run()
                .AddTo(this);
        }
    }
}