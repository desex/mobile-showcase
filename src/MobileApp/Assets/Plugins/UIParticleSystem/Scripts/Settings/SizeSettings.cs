using UnityEngine;
using TierStudios.UIParticleSystem.Enums;

namespace TierStudios.UIParticleSystem.Settings
{
    [System.Serializable]
    public class SizeSettings
    {
        public float size = 50f;
        public bool useSizeRange = false;
        public float minSize = 50f;
        public float maxSize = 100f;

        public bool use2DSize = false;
        public Vector2 size2D = new Vector2(50f, 50f);
        public bool use2DSizeRange = false;
        public Vector2 minSize2D = new Vector2(50f, 50f);
        public Vector2 maxSize2D = new Vector2(100f, 100f);

        public bool useSizeOverLifetime = false;
        public AnimationCurve sizeOverLifetimeCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
        public float sizeMultiplier = 1f;

        public bool use2DSizeOverLifetime = false;
        public AnimationCurve sizeXOverLifetimeCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
        public AnimationCurve sizeYOverLifetimeCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
        public Vector2 size2DMultiplier = Vector2.one;
    }
}