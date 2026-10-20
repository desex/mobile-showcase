using R3;
using Sotbify.MobileApp.UI.Common.Components.Toggle.Interfaces;
using UnityEngine;

namespace Sotbify.MobileApp.UI.Common.Components.Toggle
{
    public abstract class BaseToggle : MonoBehaviour, IToggle
    {
        [SerializeField] private BaseToggleGroup _group;
        [SerializeField] private SerializableReactiveProperty<bool> _isOn = new();

        public ReadOnlyReactiveProperty<bool> IsOn => _isOn;

        public void SetIsOn(bool isOn)
        {
            if (_group == null)
            {
                _isOn.Value = isOn;
                return;
            }

            _group.SetIsOn(this, isOn, (property: _isOn, isOn), (changed, state) =>
            {
                if (changed == false)
                    return;

                state.property.Value = state.isOn;
            });
        }

        public void ConnectToGroup(BaseToggleGroup toggleGroup)
        {
            _group = toggleGroup;
            toggleGroup.Register(this);
        }

        private void Awake()
        {
            _isOn.AddTo(this);
            OnAwakeInternal();
        }

        private void OnEnable()
        {
            if (_group != null)
                _group.Register(this);

            OnEnableInternal();
        }

        private void OnDisable()
        {
            if (_group != null)
                _group.Unregister(this);

            OnDisableInternal();
        }

        protected virtual void OnAwakeInternal()
        {
        }

        protected virtual void OnEnableInternal()
        {
        }

        protected virtual void OnDisableInternal()
        {
        }
    }
}