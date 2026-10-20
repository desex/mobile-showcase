using UnityEngine;
using TierStudios.UIParticleSystem.Enums;

namespace TierStudios.UIParticleSystem.Settings
{
    [System.Serializable]
    public class InheritVelocitySettings
    {
        public bool useInheritVelocity = false;
        public InheritVelocityMode mode = InheritVelocityMode.Initial;
        [Tooltip("How much of the emitter velocity or motion is applied. 1 = 100%.")]
        public float multiplier = 1f;
    }
}
