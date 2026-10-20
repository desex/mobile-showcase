using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sotbifay.Spotlight
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("UI/Spotlight/Cutout")]
    public sealed class SpotlightCutout : UIBehaviour, ISpotlightCutout
    {
        [SerializeField] private SpotlightOverlay _overlay;
        
        private RectTransform _rectTransform;
        private Rect _worldSpaceRect = Rect.zero;
        
        public event Action Changed;

        public Rect GetWorldSpaceRect() => 
            _worldSpaceRect;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if(_rectTransform == null)
                _rectTransform = transform as RectTransform;
            
            var previousOverlay = _overlay;
            
            if(_overlay == null)
                _overlay = GetComponentInParent<SpotlightOverlay>();
            
            if (previousOverlay == _overlay)
                return;
            
            if(previousOverlay != null)
                previousOverlay.UnregisterCutout(this);
                
            if(_overlay != null)
                _overlay.RegisterCutout(this);
        }
#endif

        protected override void OnEnable()
        {
            base.OnEnable();
            
            _rectTransform = transform as RectTransform;
            
            if(_overlay != null)
                _overlay.RegisterCutout(this);
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            
            if(_overlay != null)
                _overlay.UnregisterCutout(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            if(_overlay != null)
                _overlay.UnregisterCutout(this);
        }

        private void Update()
        {
            if(_rectTransform == null)
                return;
            
            var worldSpaceRect = _rectTransform.GetWorldSpaceRect(_rectTransform.rect);
            if (worldSpaceRect == _worldSpaceRect)
                return;

            _worldSpaceRect = worldSpaceRect;
            Changed?.Invoke();
        }
    }
}