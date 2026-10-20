using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.UI.Common
{
    public static class ScrollViewExtensions
    {
        public static Vector2 CalculateFocusedScrollPosition(this ScrollRect scrollView, Vector2 focusPoint) => 
            CalculateFocusedScrollPosition(scrollView, focusPoint, Vector2.one * 0.5f);

        public static Vector2 CalculateFocusedScrollPosition(this ScrollRect scrollView, Vector2 focusPoint, Vector2 focusPivot)
        {
            var contentSize = scrollView.content.rect.size;
            var viewportSize = scrollView.viewport.rect.size;
            
            Vector2 contentScale = scrollView.content.localScale;

            contentSize.Scale(contentScale);
            focusPoint.Scale(contentScale);

            var scrollPosition = scrollView.normalizedPosition;
            if (contentSize.x > viewportSize.x)
                scrollPosition.x = Mathf.Clamp01((focusPoint.x - viewportSize.x * focusPivot.x) / (contentSize.x - viewportSize.x));

            if (contentSize.y > viewportSize.y)
                scrollPosition.y = Mathf.Clamp01((focusPoint.y - viewportSize.y * focusPivot.y) / (contentSize.y - viewportSize.y));

            return scrollPosition;
        }
        
        public static Vector2 CalculateFocusedScrollLocalPosition(this ScrollRect scrollView, Vector2 point)
        {
            var contentSizeOffset = scrollView.content.rect.size;
            contentSizeOffset.Scale(scrollView.content.pivot);

            return scrollView.CalculateFocusedScrollPosition(point + contentSizeOffset);
        }

        public static void FocusAtPoint(this ScrollRect scrollView, Vector2 focusPoint)
        {
            scrollView.normalizedPosition = scrollView.CalculateFocusedScrollPosition(focusPoint);
        }

        public static void FocusAtLocalPoint(this ScrollRect scrollView, Vector2 focusPoint)
        {
            scrollView.normalizedPosition = scrollView.CalculateFocusedScrollLocalPosition(focusPoint);
        }
    }
}