using UnityEngine;

namespace Sotbify.MobileApp.UI.SafeArea
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class SoftwareKeyboardSafeArea : MonoBehaviour
    {
        [SerializeField] private bool _includeInput = false;
        
        [HideInInspector]
        [SerializeField] private RectTransform _rectTransform;

        private SoftwareKeyboardArea _softwareKeyboardArea = null;
        private DrivenRectTransformTracker _tracker;
        
        private float _previousHeight;
        
        public bool IncludeInput
        {
            get => _includeInput;
            set => _includeInput = value;
        }

        private RectTransform CachedRectTransform => _rectTransform != null && _rectTransform.gameObject == gameObject
            ? _rectTransform
            : _rectTransform = transform as RectTransform ?? GetComponent<RectTransform>();

        private void Awake()
        {
            _softwareKeyboardArea = new SoftwareKeyboardArea();
        }

        private void OnEnable()
        {
            _previousHeight = -1f;
            _tracker.Add(this, CachedRectTransform, DrivenTransformProperties.Anchors);
        }

        private void Update()
        {
            var height = _softwareKeyboardArea?.GetHeight(_includeInput) ?? 0;
            if (Mathf.Abs(height - _previousHeight) < float.Epsilon)
                return;
            
            var screenSize = new Vector2(Screen.width, Screen.height);
            var safeArea = new Rect
            (
                0,
                height,
                screenSize.x,
                screenSize.y - height
            );

            CachedRectTransform.anchorMin = safeArea.position / screenSize;
            CachedRectTransform.anchorMax = (safeArea.position + safeArea.size) / screenSize;
            
            _previousHeight = height;
        }

        private void OnDisable() => _tracker.Clear();

        private void OnDestroy() => _softwareKeyboardArea?.Dispose();
    }
}