using LitMotion;
using R3;
using Sotbify.MobileApp.UI.Common.Components.Toggle.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.UI.Common.Components.Toggle.Components
{
    [RequireComponent(typeof(IToggle))]
    public sealed class ToggleColor : MonoBehaviour
    {
        [SerializeField] private Graphic _graphic;
        [Space]
        [SerializeField] private float _animationDuration = 0.33f;
        [SerializeField] private Ease _animationEase = Ease.InQuad;
        [SerializeField] private Color _isOnColor = Color.white;
        [SerializeField] private Color _isOffColor = Color.gray;

        private IToggle _toggle;
        private MotionHandle _animationMotionHandle;

        private void Awake()
        {
            _toggle = GetComponent<IToggle>();
        }

        private void Start()
        {
            if(_toggle == null)
                return;
            
            _toggle.IsOn
                .Subscribe(this, (isOn, state) => state.SetIsOn(isOn))
                .AddTo(this);
        }

        private void SetIsOn(bool isOn)
        {
            if(_graphic == null)
                return;
            
            if (_animationMotionHandle.IsActive()) 
                _animationMotionHandle.Cancel();
            
            var targetColor = isOn ? _isOnColor : _isOffColor;
            _animationMotionHandle = LMotion.Create(_graphic.color, targetColor, _animationDuration)
                .WithEase(_animationEase)
                .Bind(_graphic, (color, graphic) => graphic.color = color)
                .AddTo(_graphic);
        }
    }
}