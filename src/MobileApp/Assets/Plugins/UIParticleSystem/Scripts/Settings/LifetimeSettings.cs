using TierStudios.UIParticleSystem.Enums;

namespace TierStudios.UIParticleSystem.Settings
{
    [System.Serializable]
    public class LifetimeSettings
    {
        public bool useRandomLifetime = false;
        public float lifetime = 5f;
        public float minLifetime = 1f;
        public float maxLifetime = 5f;
    }
}