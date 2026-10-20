using UnityEngine;

namespace TierStudios.UIParticleSystem.Settings
{
    [System.Serializable]
    public class SizeBySpeedSettings
    {
        public bool useSizeBySpeed = false;
        public bool separateAxes = false;
        
        public AnimationCurve sizeCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);
        public float sizeMultiplier = 1f;
        
        public AnimationCurve sizeXCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);
        public AnimationCurve sizeYCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);
        public Vector2 size2DMultiplier = Vector2.one;
        
        public float minSpeed = 0f;
        public float maxSpeed = 10f;
    }
}
