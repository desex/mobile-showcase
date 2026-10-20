using UnityEngine;
using TierStudios.UIParticleSystem.Core;
using TierStudios.UIParticleSystem.Settings;
using TierStudios.UIParticleSystem.Utilities;

namespace TierStudios.UIParticleSystem.Managers
{
    public class SoundEffectManager
    {
        private Core.UIParticleSystem particleSystem;
        private SoundEffectSettings settings;
        private AudioSourcePool audioPool;
        private float lastUpdateTime;
        private const float CLEANUP_INTERVAL = 1f;

        public SoundEffectManager(Core.UIParticleSystem system)
        {
            particleSystem = system;
            settings = system.SoundEffectSettings;

            if (settings != null && settings.EnableSoundEffects)
            {
                int poolSize = Mathf.Clamp(settings.MaxSimultaneousSounds, 1, 50);
                audioPool = new AudioSourcePool(system.Transform, poolSize);
            }
        }

        public void OnParticleBirth(Particle particle)
        {
            if (!CanPlaySound()) return;

            if (settings.SoundTrigger == SoundEffectTrigger.OnParticleBirth)
                PlaySound(GetParticleWorldPosition(particle));
        }

        public void OnParticleDeath(Particle particle)
        {
            if (!CanPlaySound()) return;

            if (settings.SoundTrigger == SoundEffectTrigger.OnParticleDeath)
                PlaySound(GetParticleWorldPosition(particle));
        }

        public void OnParticleCollision(Particle particle)
        {
            if (!CanPlaySound()) return;

            if (settings.SoundTrigger == SoundEffectTrigger.OnParticleCollision)
                PlaySound(GetParticleWorldPosition(particle));
        }
        private Vector3 GetParticleWorldPosition(Particle particle)
        {
            if (particle == null) return particleSystem.Transform.position;

            if (particleSystem.EmissionSettings.simulationSpace == TierStudios.UIParticleSystem.Enums.SimulationSpace.Local)
                return particleSystem.Transform.TransformPoint(particle.position);

            return particle.position;
        }

        public void OnSystemStart()
        {
            if (!CanPlaySound())
                return;

            settings.ResetPlayState();

            if (settings.SoundTrigger == SoundEffectTrigger.OnSystemStart)
            {
                PlaySound(particleSystem.Transform.position);
            }
        }

        public void OnSystemStop()
        {
            if (!CanPlaySound())
                return;

            if (settings.SoundTrigger == SoundEffectTrigger.OnSystemStop)
            {
                PlaySound(particleSystem.Transform.position);
            }
        }

        private bool CanPlaySound()
        {
            if (settings == null || !settings.EnableSoundEffects)
                return false;

            if (settings.AudioClips == null || settings.AudioClips.Count == 0)
                return false;

            if (!Application.isPlaying)
                return false;

            if (particleSystem == null || particleSystem.gameObject == null || !particleSystem.gameObject.activeInHierarchy)
                return false;

            if (!settings.CanPlay(Time.time))
                return false;

            if (audioPool == null)
            {
                int poolSize = Mathf.Clamp(settings.MaxSimultaneousSounds, 1, 50);
                audioPool = new AudioSourcePool(particleSystem.Transform, poolSize);
            }

            if (audioPool.GetActiveCount() >= settings.MaxSimultaneousSounds)
                return false;

            return true;
        }

        private void PlaySound(Vector3 position)
        {
            AudioClip clip = settings.GetNextClip();

            if (clip == null)
                return;

            AudioSource source = audioPool.Get();

            if (source == null)
                return;

            if (source.gameObject == null || !source.gameObject.activeInHierarchy)
                return;

            source.clip = clip;
            source.volume = settings.GetVolume();
            source.pitch = settings.GetPitch();
            source.loop = settings.Loop;
            source.spatialBlend = settings.SpatialBlend;
            source.minDistance = settings.MinDistance;
            source.maxDistance = settings.MaxDistance;
            source.outputAudioMixerGroup = settings.AudioMixerGroup;

            source.transform.position = position;

            if (source.enabled && source.gameObject.activeInHierarchy)
            {
                source.Play();
                settings.MarkAsPlayed(Time.time);
            }
        }

        public void Update(float deltaTime)
        {
            if (audioPool == null)
                return;

            lastUpdateTime += deltaTime;

            if (lastUpdateTime >= CLEANUP_INTERVAL)
            {
                audioPool.UpdateActiveSources();
                lastUpdateTime = 0f;
            }
        }

        public void Clear()
        {
            if (audioPool != null)
            {
                audioPool.Clear();
            }

            if (settings != null)
            {
                settings.ResetPlayState();
            }
        }

        public void StopAllSounds()
        {
            if (audioPool != null)
            {
                audioPool.Clear();
            }
        }

        public int GetActiveSoundCount()
        {
            return audioPool?.GetActiveCount() ?? 0;
        }
    }
}