using R3;
using UnityEngine;

namespace Sotbify.MobileApp.BlurredBackgroundWindow
{
    public interface IBlurredBackground
    {
        public ReadOnlyReactiveProperty<Material> TargetMaterial { get; }

        public void Start();
    }
}