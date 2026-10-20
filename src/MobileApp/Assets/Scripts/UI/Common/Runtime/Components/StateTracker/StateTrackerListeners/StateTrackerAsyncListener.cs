using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Sotbify.MobileApp.UI.Common.Components.StateTracker.StateTrackerListeners
{
    [ExecuteAlways]
    [HideMonoScript]
    public abstract class StateTrackerAsyncListener<TValue> : MonoBehaviour
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

        protected abstract UniTask ChangeStateAsyncInternal(StateTracker.State state, TValue value, CancellationToken cancellationToken);

        #region Editor
#if UNITY_EDITOR

        protected virtual void OnValidate()
        {
            var state = _tracker != null ? _tracker.CurrentState : default;
            var value = _properties.GetValueByState(state);
            ChangeStateAsyncInternal(state, value, CancellationToken.None);
        }

#endif
        #endregion

        private void Awake()
        {
            if (_tracker == null)
                return;

            _tracker.AsReactiveProperty()
                .SubscribeAwait(this, static (state, listener, cancellationToken) =>
                {
                    var value = listener._properties.GetValueByState(state);
                    return listener.ChangeStateAsyncInternal(state, value, cancellationToken);
                }, AwaitOperation.Switch)
                .AddTo(this);

            AwakeInternal();
        }
    }
}