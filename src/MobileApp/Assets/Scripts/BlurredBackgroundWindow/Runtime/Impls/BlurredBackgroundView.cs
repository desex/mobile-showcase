using Sotbify.MobileApp.UI.Window.View;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.BlurredBackgroundWindow
{
    public class BlurredBackgroundView : FadeWindowView
    {
        [SerializeField] private Image _blurBackground;

        public Material BackgroundMaterial { set => _blurBackground.material = value; }
    }
}