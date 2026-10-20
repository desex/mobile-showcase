using LitMotion;
using LitMotion.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Sotbify.MobileApp.UI.Common.Components.StateTracker.StateTrackerListeners
{
    public sealed class RotationStateListener : StateTrackerListener<Vector3>
    {
        private const string AnimationGroup = "Animation";
        
        [Required]
        [SerializeField] private Transform _transform;
        
        [BoxGroup(AnimationGroup)]
        [SerializeField] private float _duration = 0.1f;
        [BoxGroup(AnimationGroup)]
        [SerializeField] private Ease _ease = Ease.InQuad;

        private MotionHandle _handle;
        
        protected override void StateChangedInternal(StateTracker.State state, Vector3 value)
        {
            if(_transform == null)
                return;
            
#if UNITY_EDITOR
            if(Application.isPlaying == false)
                _transform.localRotation = Quaternion.Euler(value);
#endif

            var currentLocalRotation = _transform.localRotation;
            
            if(_handle.IsActive())
                _handle.Cancel();
            
            _handle = LMotion.Create(currentLocalRotation, Quaternion.Euler(value), _duration)
                .WithEase(_ease)
                .BindToLocalRotation(_transform)
                .AddTo(_transform);
        }
    }
}