using UnityEngine;
using TierStudios.UIParticleSystem.Enums;

namespace TierStudios.UIParticleSystem.Settings
{
    [System.Serializable]
    public class RendererSettings
    {
        public UIRenderMode renderMode = UIRenderMode.Billboard;

        [Tooltip("Speed-based length multiplier. UI space uses high speed values, so keep this very low (e.g. 0.01 - 0.1).")]
        [Range(0f, 1f)]
        public float speedStretch = 0f;

        [Tooltip("Constant length multiplier applied to the particle height. 1 = original size.")]
        [Range(0.01f, 10f)]
        public float lengthScale = 1f;
    }
}