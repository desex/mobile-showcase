using System;
using LitMotion.Animation;
using TierStudios.UIParticleSystem.Core;

namespace Sotbify.MobileApp.Common.LitMotion
{
    [Serializable]
    [LitMotionAnimationComponentMenu("UI/Particle System/Attractor/Strength")]
    public sealed class AttractorStrengthAnimation : FloatPropertyAnimationComponent<UIParticleSystem>
    {
        protected override float GetValue(UIParticleSystem target)
        {
            var attractorSettings = target.AttractorSettings;
            return attractorSettings.attractorStrength;
        }

        protected override void SetValue(UIParticleSystem target, in float value)
        {
            var attractorSettings = target.AttractorSettings;
            attractorSettings.attractorStrength = value;
        }
    }
    
    [Serializable]
    [LitMotionAnimationComponentMenu("UI/Particle System/Attractor/Range")]
    public sealed class AttractorRangeAnimation : FloatPropertyAnimationComponent<UIParticleSystem>
    {
        protected override float GetValue(UIParticleSystem target)
        {
            var attractorSettings = target.AttractorSettings;
            return attractorSettings.attractorRange;
        }

        protected override void SetValue(UIParticleSystem target, in float value)
        {
            var attractorSettings = target.AttractorSettings;
            attractorSettings.attractorRange = value;
        }
    }
}