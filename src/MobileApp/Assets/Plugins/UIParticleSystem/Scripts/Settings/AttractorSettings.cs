using UnityEngine;
using TierStudios.UIParticleSystem.Enums;

namespace TierStudios.UIParticleSystem.Settings
{
    [System.Serializable]
    public class AttractorSettings
    {
        public bool useAttractor = false;
        public Transform attractorTarget = null;
        public Vector2 attractorPosition = Vector2.zero;
        public float attractorStrength = 100f;
        public float attractorRange = 200f;

        public bool attractorFollowsMouse = false;
        public bool attractorRelativeToEmitter = true;
        public AttractorType attractorType = AttractorType.Attract;
        public AnimationCurve attractorFalloff = AnimationCurve.Linear(0f, 1f, 1f, 0f);
        public float orbitalSpeed = 50f;

        public bool useAttractorLifetimeLimit = false;
        public float attractorMinLifetime = 0.2f;
        public float attractorMaxLifetime = 1.0f;

        public bool attractorKillOnReach = false;
        public float attractorKillRadius = 10f;
        public bool attractorFadeOnApproach = true;
        public float attractorFadeStartDistance = 30f;

        public AnimationCurve attractorLerp = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    }
}