using UnityEngine;
using TierStudios.UIParticleSystem.Enums;

namespace TierStudios.UIParticleSystem.Settings
{
    [System.Serializable]
    public class TrailSettings
    {
        public bool useTrails = false;
        public float trailLifetime = 1f;
        public float trailUpdateInterval = 0.05f;
        public float trailWidth = 2f;
        public Material trailMaterial;

        public TrailColorMode trailColorMode = TrailColorMode.InheritParticleColor;

        public Color trailColor = Color.white;
        public Color trailColorSecondary = Color.white;

        public Gradient trailGradient = new Gradient();
        public Gradient trailGradientSecondary = new Gradient();

        public AnimationCurve trailWidthCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

        [HideInInspector] public int trailMaxPoints = 20;
        [HideInInspector] public float trailMinimumVertexDistance = 10f;
    }
}