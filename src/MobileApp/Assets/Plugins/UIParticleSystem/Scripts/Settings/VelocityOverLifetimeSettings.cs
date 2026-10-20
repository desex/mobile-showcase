using UnityEngine;

namespace TierStudios.UIParticleSystem.Settings
{
    [System.Serializable]
    public class VelocityOverLifetimeSettings
    {
        public bool useVelocityOverLifetime = false;

        public bool useLinearVelocity = true;
        public Vector2 linearVelocity = Vector2.zero;
        public bool useLinearVelocityCurves = false;
        public AnimationCurve linearVelocityX = AnimationCurve.Linear(0f, 0f, 1f, 0f);
        public AnimationCurve linearVelocityY = AnimationCurve.Linear(0f, 0f, 1f, 0f);
        public AnimationCurve linearVelocityXMinCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);
        public AnimationCurve linearVelocityYMinCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);
        public bool useRandomLinearVelocity = false;
        public Vector2 linearVelocityMin = Vector2.zero;
        public Vector2 linearVelocityMax = Vector2.zero;

        public bool useOrbitalVelocity = false;
        public float orbitalVelocity = 0f;
        public bool useOrbitalVelocityCurve = false;
        public AnimationCurve orbitalVelocityCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);
        public AnimationCurve orbitalVelocityMinCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);
        public Vector2 orbitalCenter = Vector2.zero;
        public bool useRandomOrbitalVelocity = false;
        public float orbitalVelocityMin = 0f;
        public float orbitalVelocityMax = 0f;

        public bool useLocalSpace = true;
    }
}