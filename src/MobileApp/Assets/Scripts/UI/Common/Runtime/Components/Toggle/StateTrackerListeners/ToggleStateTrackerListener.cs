using System;
using R3;
using Sirenix.OdinInspector;
using Sotbify.MobileApp.UI.Common.Components.StateTracker;
using UnityEngine;

namespace Sotbify.MobileApp.UI.Common.Components.Toggle.StateTrackerListeners
{
    [ExecuteAlways]
    [HideMonoScript]
    public abstract class ToggleStateTrackerListener<TValue> : MonoBehaviour
    {
        [Serializable]
        public sealed class PropertyBlock : StatePropertyBlock<TValue> { }

        [Required] 
        [SerializeField] private StateTracker.StateTracker _tracker;
        [Required]
        [SerializeField] private BaseToggle _toggle;
        [SerializeField] private PropertyBlock _isOnProperties;
        [SerializeField] private PropertyBlock _isOffProperties;

        protected virtual void AwakeInternal() { }
        
        protected abstract void StateChangedInternal(StateTracker.StateTracker.State state, TValue value);
        
        #region Editor
#if UNITY_EDITOR

        protected virtual void OnValidate()
        {
            var isOn = _toggle == null || _toggle.IsOn.CurrentValue;
            var propertyBlock = isOn ? _isOnProperties : _isOffProperties;
                    
            var state = _tracker != null ? _tracker.CurrentState : default;
                    
            var value = propertyBlock.GetValueByState(state);
            StateChangedInternal(state, value);
        }
        
#endif
        #endregion

        private void Awake()
        {
            if(_toggle == null)
                return;
            
            if(_tracker == null)
                return;
            
            _toggle.IsOn
                .AsUnitObservable()
                .Merge(_tracker.AsObservable().AsUnitObservable())
                .Subscribe(this, static (_, listener) =>
                {
                    var toggle = listener._toggle;
                    var isOn = toggle == null || toggle.IsOn.CurrentValue;
                    var propertyBlock = isOn ? listener._isOnProperties : listener._isOffProperties;
                    
                    var tracker = listener._tracker;
                    var state = tracker != null ? tracker.CurrentState : default;
                    
                    var value = propertyBlock.GetValueByState(state);
                    listener.StateChangedInternal(state, value);
                })
                .AddTo(this);

            AwakeInternal();
        }
    }
}