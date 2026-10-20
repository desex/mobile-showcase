using UnityEngine;

namespace Sotbify.MobileApp.UI.Common.Providers
{
    public sealed class MonoRootsRectProvider : MonoBehaviour, IRootsRectProvider
    {
        [SerializeField] private RectTransform _nexusRoot;
        [SerializeField] private RectTransform _backgroundBlurRoot;
        [SerializeField] private RectTransform _overlayRoot;
        [SerializeField] private RectTransform _tutorialRoot;
        [SerializeField] private RectTransform _commonRoot;

        public RectTransform NexusRoot => _nexusRoot;
        public RectTransform BackgroundBlurRoot => _backgroundBlurRoot;
        public RectTransform OverlayRoot => _overlayRoot;
        public RectTransform TutorialRoot => _tutorialRoot;
        public RectTransform CommonRoot => _commonRoot;
    }
}