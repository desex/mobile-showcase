using TierStudios.UIParticleSystem.Core;

namespace TierStudios.UIParticleSystem.Settings
{
    public class ParticleEventArgs
    {
        public Particle Particle { get; private set; }

        public TierStudios.UIParticleSystem.Core.UIParticleSystem System { get; private set; }

        public ParticleEventArgs(Particle particle, TierStudios.UIParticleSystem.Core.UIParticleSystem system)
        {
            Particle = particle;
            System = system;
        }

        public void UpdateData(Particle particle, TierStudios.UIParticleSystem.Core.UIParticleSystem system)
        {
            Particle = particle;
            System = system;
        }
    }
}