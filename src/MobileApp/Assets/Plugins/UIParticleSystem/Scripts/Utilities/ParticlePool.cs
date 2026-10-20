using System.Collections.Generic;
using TierStudios.UIParticleSystem.Core;

namespace TierStudios.UIParticleSystem.Utilities
{
    public class ParticlePool
    {
        private readonly Stack<Particle> _pool;
        private readonly int _capacity;
        private int _totalCreated;

        public int PooledCount => _pool.Count;
        public int TotalCreated => _totalCreated;

        public ParticlePool(UnityEngine.Transform _, int capacity = 128)
            : this(capacity) { }

        public ParticlePool(int capacity = 128)
        {
            _capacity = capacity;
            _pool = new Stack<Particle>(capacity);
        }

        public Particle Get()
        {
            Particle p = _pool.Count > 0 ? _pool.Pop() : CreateNew();
            ResetParticle(p);
            return p;
        }

        public void Return(Particle particle)
        {
            if (particle == null) return;
            particle.isAlive = false;

            if (_pool.Count < _capacity)
                _pool.Push(particle);
        }

        public void Clear()
        {
            _pool.Clear();
        }

        public int GetPoolSize() => _pool.Count;
        public int GetTotalCreated() => _totalCreated;

        private Particle CreateNew()
        {
            _totalCreated++;
            return new Particle();
        }

        private static void ResetParticle(Particle p)
        {
            p.position = UnityEngine.Vector3.zero;
            p.velocity = UnityEngine.Vector3.zero;
            p.age = 0f;
            p.lifetime = 0f;
            p.maxLifetime = 0f;
            p.isAlive = true;
            p.size = 1f;
            p.color = UnityEngine.Color.white;
            p.rotation = 0f;
            p.angularVelocity = 0f;
            p.rotationSpeed = 0f;
            p.attractorStrength = 1f;
            p.attractorFadeAlpha = 1f;
            p.currentFrame = 0;
            p.frameTime = 0f;
            p.animationTime = 0f;
            p.stretchedWidth = 0f;
            p.stretchedHeight = 0f;
            p.stretchedRotation = 0f;
            p.velocityRandomLinearOffset = UnityEngine.Vector2.zero;
            p.velocityRandomOrbitalOffset = 0f;
            p.forceRandomLerpX = 0f;
            p.forceRandomLerpY = 0f;
            p.collisionCount = 0;
        }
    }
}