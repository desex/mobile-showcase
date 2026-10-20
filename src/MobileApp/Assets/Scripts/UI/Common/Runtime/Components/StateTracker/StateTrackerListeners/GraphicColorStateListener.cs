using LitMotion;
using LitMotion.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.UI.Common.Components.StateTracker.StateTrackerListeners
{
    public sealed class GraphicColorStateListener : StateTrackerListener<Color32>
    {
        private const string AnimationGroup = "Animation";
        
        [Required]
        [SerializeField] private Graphic _graphic;
        
        [BoxGroup(AnimationGroup)]
        [SerializeField] private float _duration = 0.1f;
        [BoxGroup(AnimationGroup)]
        [SerializeField] private Ease _ease = Ease.InQuad;

        private MotionHandle _handle;
        
        protected override void StateChangedInternal(StateTracker.State state, Color32 value)
        {
            if(_graphic == null)
                return;

#if UNITY_EDITOR
            if(Application.isPlaying == false)
                _graphic.color = value;
#endif

            var currentColor = _graphic.color;
            
            if(_handle.IsActive())
                _handle.Cancel();
            
            _handle = LMotion.Create(currentColor, value, _duration)
                .WithEase(_ease)
                .BindToColor(_graphic)
                .AddTo(_graphic);
        }
    }
}