using R3;
using Sotbify.MobileApp.Db.BlurBackgroundConfig;
using UnityEngine;

namespace Sotbify.MobileApp.BlurredBackgroundWindow
{
    public class BlurredBackground : IBlurredBackground
    {
        private readonly IBlurBackgroundConfig _blurBackgroundConfig;
        
        private readonly ReactiveProperty<Material> _targetMaterial = new ();

        public ReadOnlyReactiveProperty<Material> TargetMaterial => _targetMaterial;

        public BlurredBackground(IBlurBackgroundConfig blurBackgroundConfig)
        {
            _blurBackgroundConfig = blurBackgroundConfig;
        }

        public void Start()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}