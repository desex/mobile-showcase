using UnityEngine;

namespace Sotbify.MobileApp.UI.Common.Providers
{
    public interface IRootsRectProvider
    {
        public RectTransform NexusRoot { get; }
        public RectTransform BackgroundBlurRoot { get; }
        public RectTransform OverlayRoot { get; }
        public RectTransform TutorialRoot { get; }
        public RectTransform CommonRoot { get; }
    }   
}