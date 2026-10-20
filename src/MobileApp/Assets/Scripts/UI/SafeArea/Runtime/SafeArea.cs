using UnityEngine;

namespace Sotbify.MobileApp.UI.SafeArea
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class SafeArea : MonoBehaviour
    {
        [HideInInspector]
        [SerializeField] private RectTransform _rectTransform;

        private DrivenRectTransformTracker _tracker;

        private RectTransform CachedRectTransform => _rectTransform != null && _rectTransform.gameObject == gameObject
            ? _rectTransform
            : _rectTransform = transform as RectTransform ?? GetComponent<RectTransform>();

        private void Start() => Fit();

        private void OnEnable() => _tracker.Add(this, CachedRectTransform, DrivenTransformProperties.Anchors);

        private void OnDisable() => _tracker.Clear();

        private void Fit()
        {
            var safeArea = Screen.safeArea;
            var screenSize = new Vector2(Screen.width, Screen.height);

            CachedRectTransform.anchorMin = safeArea.position / screenSize;
            CachedRectTransform.anchorMax = (safeArea.position + safeArea.size) / screenSize;
        }
    }
}