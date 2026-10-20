using LitMotion;
using R3;
using Sotbify.MobileApp.UI.Common.Components.Toggle.Interfaces;
using UnityEngine;

namespace Sotbify.MobileApp.UI.Common.Components.Toggle.Components
{
    [RequireComponent(typeof(IToggle))]
    public class ToggleMotion : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private float _moveDelta;
        
        [Space]
        [SerializeField] private float _animationDuration = 0.33f;
        [SerializeField] private Ease _animationEase = Ease.InQuad;
        
        private IToggle _toggle;
        private MotionHandle _animationMotionHandle;
        
        private Vector2 _offPosition;
        private Vector2 _onPosition;
        
        private void Awake()
        {
            _toggle = GetComponent<IToggle>();
        }

        private void Start()
        {
            if(_toggle == null)
                return;
            
            _offPosition = _rectTransform.anchoredPosition;
            _onPosition = new Vector2(_rectTransform.anchoredPosition.x + _moveDelta, _rectTransform.anchoredPosition.y);;
            
            _toggle.IsOn
                .Subscribe(this, (isOn, state) => state.SetIsOn(isOn))
                .AddTo(this);
        }

        private void SetIsOn(bool isOn)
        {
            if(_rectTransform == null)
                return;
            
            if (_animationMotionHandle.IsActive()) 
                _animationMotionHandle.Cancel();
            
            var targetAnchoredPosition = isOn ? _onPosition : _offPosition;

            _animationMotionHandle = LMotion
                .Create(_rectTransform.anchoredPosition, targetAnchoredPosition, _animationDuration)
                .WithEase(_animationEase)
                .Bind(value => _rectTransform.anchoredPosition = value)
                .AddTo(_rectTransform);
        }
    }
}