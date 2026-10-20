using R3;
using Sotbify.MobileApp.UI.Common.Components.Toggle.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.UI.Common.Components.Toggle.Components
{
    [RequireComponent(typeof(IToggle))]
    public sealed class ToggleSprite : MonoBehaviour
    {
        [SerializeField] private Image _graphic;
        [Space] 
        [SerializeField] private Sprite _isOnSprite;
        [SerializeField] private Sprite _isOffSprite;

        private IToggle _toggle;

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
            
            _graphic.sprite = isOn ? _isOnSprite : _isOffSprite;
        }
    }
}