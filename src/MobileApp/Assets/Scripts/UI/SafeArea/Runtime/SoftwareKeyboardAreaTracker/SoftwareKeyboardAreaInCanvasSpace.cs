using System;
using UnityEngine;

namespace Sotbify.MobileApp.UI.SafeArea
{
    public sealed class SoftwareKeyboardAreaInCanvasSpace : ISoftwareKeyboardArea
    {
        private SoftwareKeyboardArea _softwareKeyboardArea = new();
        private RectTransform _canvasRectTransform;
        
        public SoftwareKeyboardAreaInCanvasSpace(Component component, bool includeInactiveCanvases = false)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));

            var canvas = component.GetComponentInParent<Canvas>(includeInactiveCanvases);
            if(canvas == null)
                return;
            
            _canvasRectTransform = canvas.GetComponent<RectTransform>();
        }

        public SoftwareKeyboardAreaInCanvasSpace(GameObject gameObject, bool includeInactiveCanvases = false)
        {
            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));

            var canvas = gameObject.GetComponentInParent<Canvas>(includeInactiveCanvases);
            if(canvas == null)
                return;
            
            _canvasRectTransform = canvas.GetComponent<RectTransform>();
        }

        public SoftwareKeyboardAreaInCanvasSpace(Canvas canvas)
        {
            if (canvas == null)
                throw new ArgumentNullException(nameof(canvas));

            _canvasRectTransform = canvas.GetComponent<RectTransform>();
        }

        public int GetHeight(bool includeInput = false)
        {
            if (_canvasRectTransform == null)
                return 0;

            if (_softwareKeyboardArea == null)
                return 0;

            var normalizedHeight = Mathf.Clamp01(_softwareKeyboardArea.GetHeight() / (float)Screen.height);
            var canvasHeight = _canvasRectTransform.sizeDelta.y;

            return Mathf.RoundToInt(canvasHeight * normalizedHeight);
        }

        public void Dispose()
        {
            _softwareKeyboardArea?.Dispose();
            _softwareKeyboardArea = null;

            _canvasRectTransform = null;
        }
    }
}