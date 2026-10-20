using UnityEngine;
using UnityEngine.Events;
using TierStudios.UIParticleSystem.Core;

namespace TierStudios.UIParticleSystem.Settings
{
    [System.Serializable]
    public class ParticleEventSettings
    {
        [Header("Event Settings")]
        [SerializeField] private bool enableEvents = false;
        [SerializeField] private bool logEventsToConsole = false;

        [Header("Particle Events")]
        [Space(5)]
        public ParticleSpawnEvent onParticleSpawned = new ParticleSpawnEvent();

        [Space(5)]
        public ParticleDeathEvent onParticleDied = new ParticleDeathEvent();

        [Space(5)]
        public ParticleCollisionEvent onParticleCollision = new ParticleCollisionEvent();

        [Header("System Events")]
        [Space(5)]
        public SystemEvent onEmissionStarted = new SystemEvent();

        [Space(5)]
        public SystemEvent onEmissionStopped = new SystemEvent();

        [Space(5)]
        public SystemEvent onSystemPaused = new SystemEvent();

        [Space(5)]
        public SystemEvent onSystemResumed = new SystemEvent();

        [Space(5)]
        public SystemEvent onParticlesCleared = new SystemEvent();

        public bool EnableEvents => enableEvents;

        public bool LogEventsToConsole => logEventsToConsole;
    }

    [System.Serializable]
    public class ParticleSpawnEvent : UnityEvent<ParticleEventData>
    {
    }

    [System.Serializable]
    public class ParticleDeathEvent : UnityEvent<ParticleEventData>
    {
    }

    [System.Serializable]
    public class ParticleCollisionEvent : UnityEvent<ParticleEventData>
    {
    }

    [System.Serializable]
    public class SystemEvent : UnityEvent<TierStudios.UIParticleSystem.Core.UIParticleSystem>
    {
    }

    [System.Serializable]
    public class ParticleEventData
    {
        public Vector2 position;
        public Color color;
        public float size;
        public float lifetime;
        public float age;
        public int particleIndex;
        public float timestamp;

        [System.NonSerialized]
        public Particle particle;

        public ParticleEventData(Particle particle, int index = -1)
        {
            this.particle = particle;

            if (particle != null)
            {
                position = particle.position;
                color = particle.color;
                size = particle.size;
                lifetime = particle.maxLifetime;
                age = particle.age;
                particleIndex = index;
                timestamp = Time.time;
            }
            else
            {
                position = Vector2.zero;
                color = Color.white;
                size = 1f;
                lifetime = 1f;
                age = 0f;
                particleIndex = -1;
                timestamp = Time.time;
            }
        }

        public ParticleEventData(Vector2 pos, Color col, float sz, float life, float currentAge, int index = -1)
        {
            position = pos;
            color = col;
            size = sz;
            lifetime = life;
            age = currentAge;
            particleIndex = index;
            timestamp = Time.time;
            particle = null;
        }

        public void UpdateData(Particle particle, int index = -1)
        {
            this.particle = particle;

            if (particle != null)
            {
                position = particle.position;
                color = particle.color;
                size = particle.size;
                lifetime = particle.maxLifetime;
                age = particle.age;
                particleIndex = index;
                timestamp = Time.time;
            }
        }
    }
}