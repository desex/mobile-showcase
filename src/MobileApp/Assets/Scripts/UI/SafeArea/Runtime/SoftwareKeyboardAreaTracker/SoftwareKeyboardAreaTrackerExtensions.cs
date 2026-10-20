using System;
using UnityEngine;

namespace Sotbify.MobileApp.UI.SafeArea
{
    public static class SoftwareKeyboardAreaTrackerExtensions
    {
        public static float GetNormalizedHeight(this ISoftwareKeyboardArea softwareKeyboardArea)
        {
            return softwareKeyboardArea is not null
                ? Mathf.Clamp01(softwareKeyboardArea.GetHeight() / (float)Screen.height)
                : throw new ArgumentNullException(nameof(softwareKeyboardArea));
        }

        public static int GetHeightInCanvasSpace(this ISoftwareKeyboardArea softwareKeyboardArea, Component component, bool includeInactive = false)
        {
            if (softwareKeyboardArea is null)
                throw new ArgumentNullException(nameof(softwareKeyboardArea));
            
            return component != null 
                ? softwareKeyboardArea.GetHeightInCanvasSpace(component.gameObject, includeInactive) 
                : throw new ArgumentNullException(nameof(component));
        }

        public static int GetHeightInCanvasSpace(this ISoftwareKeyboardArea softwareKeyboardArea, GameObject gameObject, bool includeInactive = false)
        {
            if (softwareKeyboardArea is null)
                throw new ArgumentNullException(nameof(softwareKeyboardArea));

            var canvas = gameObject.GetComponentInParent<Canvas>(includeInactive);
            return canvas != null 
                ? softwareKeyboardArea.GetHeightInCanvasSpace(canvas) 
                : 0;
        }
        
        public static int GetHeightInCanvasSpace(this ISoftwareKeyboardArea softwareKeyboardArea, Canvas canvas)
        {
            if (softwareKeyboardArea is null)
                throw new ArgumentNullException(nameof(softwareKeyboardArea));

            if(canvas == null)
                throw new ArgumentNullException(nameof(canvas));
            
            if (canvas.TryGetComponent<RectTransform>(out var canvasRectTransform) == false)
                return 0;

            var normalizedHeight = softwareKeyboardArea.GetNormalizedHeight();
            var canvasHeight = canvasRectTransform.sizeDelta.y;

            return Mathf.RoundToInt(canvasHeight * normalizedHeight);
        }
    }
}