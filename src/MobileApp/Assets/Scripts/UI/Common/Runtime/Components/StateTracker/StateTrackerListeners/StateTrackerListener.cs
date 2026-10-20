using System;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Sotbify.MobileApp.UI.Common.Components.StateTracker.StateTrackerListeners
{
    [ExecuteAlways]
    [HideMonoScript]
    public abstract class StateTrackerListener<TValue> : MonoBehaviour
    {
        [Serializable]
        public sealed class PropertyBlock : StatePropertyBlock<TValue>
        {
        }

        [Required] 
        [SerializeField] private StateTracker _tracker;
        [SerializeField] private PropertyBlock _properties;

        protected virtual void AwakeInternal()
        {
        }

        protected abstract void StateChangedInternal(StateTracker.State state, TValue value);

        #region Editor
#if UNITY_EDITOR

        protected virtual void OnValidate()
        {
            var state = _tracker != null ? _tracker.CurrentState : default;
            var value = _properties.GetValueByState(state);
            StateChangedInternal(state, value);
        }

#endif
        #endregion

        private void Awake()
        {
            if (_tracker == null)
                return;

            _tracker.AsReactiveProperty()
                .Subscribe(this, static (state, listener) =>
                {
                    var value = listener._properties.GetValueByState(state);
                    listener.StateChangedInternal(state, value);
                })
                .AddTo(this);

            AwakeInternal();
        }
    }
}