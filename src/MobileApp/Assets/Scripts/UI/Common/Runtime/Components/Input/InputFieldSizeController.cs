using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Sotbify.MobileApp.UI.Common.Components.Input
{
    public class InputFieldSizeController : MonoBehaviour
    {
        [Header("Viewport Settings")] 
        
        [SerializeField] private float _maxViewportWidth = 500f;
        [SerializeField] private float _maxViewportHeight = 300f;

        [Header("References")] 
        
        [SerializeField] private RectTransform _viewport;
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private RectTransform _content;
        
        [Header("Margins")] 
        [SerializeField] private float _horizontalMargin = 10f;
        [SerializeField] private float _verticalMargin = 5f;

        private Vector2 _lastContentSize;
        private bool _isLayoutDirty = true;

        private void Start()
        {
            if (_inputField != null)
            {
                _inputField.onValueChanged.AddListener(OnInputChanged);
            }

            UpdateViewportSize();
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                if (_content != null && _content.hasChanged)
                {
                    _content.hasChanged = false;
                    _isLayoutDirty = true;
                }
            }

            if (!_isLayoutDirty) 
                return;
            
            UpdateViewportSize();
            _isLayoutDirty = false;
        }

        private void OnInputChanged(string text)
        {
            _isLayoutDirty = true;
        }

        private void UpdateViewportSize()
        {
            if (_viewport == null || _content == null) 
                return;

            var preferredWidth = LayoutUtility.GetPreferredWidth(_content);
            var preferredHeight = LayoutUtility.GetPreferredHeight(_content);

            preferredHeight += _verticalMargin * 2f;

            var finalHeight = Mathf.Clamp(preferredHeight, 0, _maxViewportHeight);

            _viewport.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, finalHeight);

            _lastContentSize = new Vector2(preferredWidth, preferredHeight);
        }
        
        [Button("Refresh Viewport Size")]
        public void RefreshViewportSize()
        {
            UpdateViewportSize();
        }

        public void SetMaxViewportSize(float width, float height)
        {
            _maxViewportWidth = width;
            _maxViewportHeight = height;
            _isLayoutDirty = true;
        }

        public void SetMargins(float horizontal, float vertical)
        {
            _horizontalMargin = horizontal;
            _verticalMargin = vertical;
            _isLayoutDirty = true;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _isLayoutDirty = true;
        }

        private void Reset()
        {
            if (_viewport == null)
                _viewport = GetComponent<RectTransform>();

            if (_inputField == null)
                _inputField = GetComponentInChildren<TMP_InputField>();

            if (_content == null && _inputField != null)
                _content = _inputField.textComponent?.rectTransform;
        }
#endif
    }
}