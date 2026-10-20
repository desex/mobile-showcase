using UnityEngine;

namespace TierStudios.UIParticleSystem.Settings
{
    [System.Serializable]
    public class ForceOverLifetimeSettings
    {
        public bool useForceOverLifetime = false;

        public int mode = 0;

        public float x = 0f;
        public float xMin = 0f;
        public AnimationCurve xCurve    = AnimationCurve.Linear(0f, 0f, 1f, 0f);
        public AnimationCurve xMinCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);

        public float y = 0f;
        public float yMin = 0f;
        public AnimationCurve yCurve    = AnimationCurve.Linear(0f, 0f, 1f, 0f);
        public AnimationCurve yMinCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);

        public bool useLocalSpace = true;
    }
}
