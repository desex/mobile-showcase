using UnityEngine;
using TierStudios.UIParticleSystem.Core;
using TierStudios.UIParticleSystem.Settings;
using TierStudios.UIParticleSystem.Enums;
using TierStudios.UIParticleSystem.Utilities;

namespace TierStudios.UIParticleSystem.Managers
{
    public class EmissionManager
    {
        private TierStudios.UIParticleSystem.Core.UIParticleSystem particleSystem;
        private float systemRuntime = 0f;
        private bool systemCompleted = false;
        private float startDelayTimer = 0f;
        private bool isDelayCompleted = false;
        private Vector3 lastPosition;
        private Vector3 inheritLastPosition;
        private bool inheritHasLastPosition;
        private bool wasSelected = false;
        private float lastUpdateTime = 0f;

        public Vector3 EmitterWorldDelta { get; private set; }
        public Vector3 EmitterWorldVelocity { get; private set; }

        private int particlesToEmit = 0;
        private float emissionAccumulator = 0f;

        private int lastParticleCount = 0;

        private Transform cachedSystemTransform;

        private bool TryGetTransform(out Transform t)
        {
            if (cachedSystemTransform == null && particleSystem != null)
                cachedSystemTransform = particleSystem.Transform;
            t = cachedSystemTransform;
            return t != null;
        }

        public float LastUpdateTime
        {
            get => lastUpdateTime;
            set => lastUpdateTime = value;
        }

        public EmissionManager(TierStudios.UIParticleSystem.Core.UIParticleSystem system)
        {
            particleSystem = system;
            cachedSystemTransform = particleSystem != null ? particleSystem.Transform : null;
            if (cachedSystemTransform != null)
                lastPosition = cachedSystemTransform.position;
            lastUpdateTime = Time.time;
            ResetEmitterMotion();
        }

        public void ResetEmitterMotion()
        {
            if (cachedSystemTransform == null && particleSystem != null)
                cachedSystemTransform = particleSystem.Transform;
            if (cachedSystemTransform == null)
                return;

            inheritLastPosition = cachedSystemTransform.position;
            inheritHasLastPosition = true;
            EmitterWorldDelta = Vector3.zero;
            EmitterWorldVelocity = Vector3.zero;
        }

        public void UpdateEmitterMotion(float deltaTime)
        {
            if (cachedSystemTransform == null && particleSystem != null)
                cachedSystemTransform = particleSystem.Transform;
            if (cachedSystemTransform == null)
                return;

            Vector3 pos = cachedSystemTransform.position;
            if (!inheritHasLastPosition)
            {
                inheritLastPosition = pos;
                inheritHasLastPosition = true;
                EmitterWorldDelta = Vector3.zero;
                EmitterWorldVelocity = Vector3.zero;
                return;
            }

            EmitterWorldDelta = pos - inheritLastPosition;
            EmitterWorldVelocity = deltaTime > 0.0001f ? EmitterWorldDelta / deltaTime : Vector3.zero;
            inheritLastPosition = pos;
        }

        public Vector3 GetEmitterVelocityInSimSpace()
        {
            Vector3 vel = EmitterWorldVelocity;
            if (particleSystem != null &&
                particleSystem.EmissionSettings.simulationSpace == SimulationSpace.Local &&
                TryGetTransform(out Transform t))
                vel = t.InverseTransformVector(vel);
            return vel;
        }

        public Vector3 GetEmitterDeltaInSimSpace()
        {
            Vector3 delta = EmitterWorldDelta;
            if (particleSystem != null &&
                particleSystem.EmissionSettings.simulationSpace == SimulationSpace.Local &&
                TryGetTransform(out Transform t))
                delta = t.InverseTransformVector(delta);
            return delta;
        }

        public void StartEmitting()
        {
            ClearParticlesOnly();

            particleSystem.IsEmitting = true;
            particleSystem.IsPaused = false;
            systemRuntime = 0f;
            systemCompleted = false;

            if (TryGetTransform(out Transform t))
                lastPosition = t.position;
            ResetEmitterMotion();

            particlesToEmit = 0;
            emissionAccumulator = 0f;

            ResetBursts();

            startDelayTimer = 0f;
            isDelayCompleted = particleSystem.EmissionSettings.startDelay <= 0;
            lastParticleCount = 0;

            if (particleSystem.EmissionSettings.prewarm && ShouldPerformPrewarm())
            {
                PerformPrewarm();
            }
        }

        public void StopEmitting()
        {
            particleSystem.IsEmitting = false;
            systemCompleted = true;
            particlesToEmit = 0;
            emissionAccumulator = 0f;
        }

        public void RestartSystem()
        {
            particleSystem.IsEmitting = true;
            particleSystem.IsPaused = false;
            systemRuntime = 0f;
            systemCompleted = false;
            startDelayTimer = 0f;
            isDelayCompleted = particleSystem.EmissionSettings.startDelay <= 0;
            if (TryGetTransform(out Transform t))
                lastPosition = t.position;
            ResetEmitterMotion();
            particlesToEmit = 0;
            emissionAccumulator = 0f;
            lastParticleCount = 0;
            ResetBursts();

            ClearParticlesOnly();

            if (particleSystem.EmissionSettings.prewarm && ShouldPerformPrewarm())
                PerformPrewarm();
        }

        private bool ShouldPerformPrewarm()
        {
            bool hasEmissionRate = particleSystem.EmissionSettings.emissionRate > 0;
            bool hasBursts = particleSystem.EmissionSettings.bursts != null && particleSystem.EmissionSettings.bursts.Count > 0;
            bool noStartDelay = particleSystem.EmissionSettings.startDelay <= 0;

            return (hasEmissionRate || hasBursts) && noStartDelay;
        }

        private void ClearParticlesOnly()
        {
            var particles = particleSystem.ActiveParticles;
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                var particle = particles[i];
                if (particle == null) continue;

                particle.trail?.Clear();
                particleSystem.Pool?.Return(particle);
            }

            particles.Clear();
        }

        public void UpdateEmission(float deltaTime)
        {
            if (!isDelayCompleted)
            {
                UpdateStartDelay(deltaTime);
                return;
            }

            UpdateSystemDuration(deltaTime);

            if (!systemCompleted || particleSystem.EmissionSettings.looping)
            {
                UpdateEmissionRateOptimized(deltaTime);
                CheckBursts();

                lastParticleCount = particleSystem.ActiveParticles.Count;
            }
        }

        private void UpdateStartDelay(float deltaTime)
        {
            if (particleSystem.EmissionSettings.startDelay > 0 && !isDelayCompleted)
            {
                startDelayTimer += deltaTime;
                if (startDelayTimer >= particleSystem.EmissionSettings.startDelay)
                {
                    isDelayCompleted = true;

                    if (particleSystem.EmissionSettings.prewarm && ShouldPerformPrewarm())
                        PerformPrewarm();
                }
            }
            else
                isDelayCompleted = true;
        }

        private void UpdateSystemDuration(float deltaTime)
        {
            if (particleSystem.EmissionSettings.duration > 0)
            {
                systemRuntime += deltaTime;

                if (systemRuntime >= particleSystem.EmissionSettings.duration)
                {
                    if (particleSystem.EmissionSettings.looping)
                    {
                        systemRuntime = 0f;
                        systemCompleted = false;
                        startDelayTimer = 0f;
                        isDelayCompleted = particleSystem.EmissionSettings.startDelay <= 0;
                        ResetBursts();
                    }
                    else
                        systemCompleted = true;
                }
            }
        }

        private void UpdateEmissionRateOptimized(float deltaTime)
        {
            var emissionRate = particleSystem.EmissionSettings.emissionRate;
            var maxParticles = particleSystem.EffectiveMaxParticles;
            var activeCount = particleSystem.ActiveParticles.Count;

            if (particleSystem.EmissionSettings.emissionRateOverDistance > 0f)
                HandleDistanceBasedEmission();

            if (emissionRate <= 0)
                return;

            emissionAccumulator += emissionRate * deltaTime;
            particlesToEmit = Mathf.FloorToInt(emissionAccumulator);
            emissionAccumulator -= particlesToEmit;

            var particlesToCreate = Mathf.Min(particlesToEmit, maxParticles - activeCount);

            for (int i = 0; i < particlesToCreate; i++)
            {
                ParticleUtilities.EmitParticle(particleSystem);
            }

            particlesToEmit = 0;
        }

        private void HandleDistanceBasedEmission()
        {
            if (!TryGetTransform(out Transform t))
                return;

            Vector3 currentPosition = t.position;
            float distanceMoved = Vector3.Distance(currentPosition, lastPosition);

            if (distanceMoved < 0.001f) return;

            float rateOverDistance = particleSystem.EmissionSettings.emissionRateOverDistance;
            if (rateOverDistance <= 0f)
            {
                lastPosition = currentPosition;
                return;
            }

            float stepSize = 1f / rateOverDistance;

            if (distanceMoved >= stepSize)
            {
                var maxParticles = particleSystem.EffectiveMaxParticles;
                var activeCount = particleSystem.ActiveParticles.Count;

                if (activeCount < maxParticles)
                    ParticleUtilities.EmitParticle(particleSystem, currentPosition);

                lastPosition = currentPosition;
            }
        }

        private void CheckBursts()
        {
            var bursts = particleSystem.EmissionSettings.bursts;
            int burstCount = bursts.Count;

            for (int i = 0; i < burstCount; i++)
            {
                Burst burst = bursts[i];

                bool shouldTrigger = false;

                if (burst.time <= 0.001f)
                    shouldTrigger = !burst.triggered && systemRuntime <= 0.1f;
                else
                    shouldTrigger = !burst.triggered && systemRuntime >= burst.time;

                if (shouldTrigger)
                {
                    if (UnityEngine.Random.value <= burst.probability)
                    {
                        var maxParticles = particleSystem.EffectiveMaxParticles;
                        var activeCount = particleSystem.ActiveParticles.Count;
                        var particlesToCreate = Mathf.Min(burst.count, maxParticles - activeCount);

                        for (int j = 0; j < particlesToCreate; j++)
                        {
                            ParticleUtilities.EmitParticle(particleSystem);
                        }
                    }
                    burst.triggered = true;
                }
            }
        }

        private void ResetBursts()
        {
            var bursts = particleSystem.EmissionSettings.bursts;
            int burstCount = bursts.Count;
            for (int i = 0; i < burstCount; i++)
            {
                bursts[i].triggered = false;
            }
        }

        private void PerformPrewarm()
        {
            float prewarmDuration = CalculatePrewarmDuration();
            const float simulationStep = 0.02f;
            int maxIterations = Mathf.FloorToInt(prewarmDuration / simulationStep);
            int effectiveMax = particleSystem.EffectiveMaxParticles;

            float savedRuntime = systemRuntime;
            float savedAccumulator = emissionAccumulator;
            systemRuntime = 0f;
            emissionAccumulator = 0f;

            ResetBursts();

            for (int iteration = 0; iteration < maxIterations; iteration++)
            {
                UpdateEmissionRateOptimized(simulationStep);
                CheckBursts();

                particleSystem.SimulatePrewarmStep(simulationStep);

                systemRuntime += simulationStep;

                if (particleSystem.ActiveParticles.Count >= effectiveMax * 0.8f)
                    break;
            }

            systemRuntime = 0f;
            emissionAccumulator = 0f;
            ResetBursts();
        }

        private float CalculatePrewarmDuration()
        {
            if (particleSystem.EmissionSettings.duration > 0)
                return Mathf.Clamp(particleSystem.EmissionSettings.duration * 0.5f, 0.5f, 3f);

            if (particleSystem.EmissionSettings.emissionRate > 0)
            {
                float timeToFill = particleSystem.EffectiveMaxParticles / particleSystem.EmissionSettings.emissionRate;
                return Mathf.Clamp(timeToFill * 0.7f, 0.5f, 2f);
            }

            if (particleSystem.EmissionSettings.bursts != null && particleSystem.EmissionSettings.bursts.Count > 0)
                return 1f;

            return 1f;
        }

#if UNITY_EDITOR
        public void HandleEditorSelection(bool selected)
        {
            if (selected)
            {
                if (!wasSelected)
                {
                    wasSelected = true;
                    lastUpdateTime = (float)UnityEditor.EditorApplication.timeSinceStartup;

                    if (particleSystem.EmissionSettings.previewInEditor)
                    {
                        bool isMultiEditing = UnityEditor.Selection.gameObjects != null &&
                                            UnityEditor.Selection.gameObjects.Length > 1;

                        if (isMultiEditing)
                        {
                            if (!particleSystem.IsEmitting)
                                StartEmitting();
                            else if (particleSystem.IsPaused)
                                particleSystem.ResumeSystem();
                        }
                        else
                        {
                            if (particleSystem.IsPaused)
                                particleSystem.ResumeSystem();
                            else if (!particleSystem.IsEmitting)
                                StartEmitting();
                        }
                    }
                }
            }
            else
            {
                if (wasSelected)
                {
                    wasSelected = false;

                    bool isMultiEditing = UnityEditor.Selection.gameObjects != null &&
                                        UnityEditor.Selection.gameObjects.Length > 1;

                    if (!isMultiEditing && particleSystem.EmissionSettings.pauseWhenDeselected)
                        particleSystem.PauseSystem();
                    else if (!isMultiEditing)
                    {
                        StopEmitting();
                        ParticleUtilities.ClearAllParticles(particleSystem);
                    }
                }
            }
        }
#endif
    }
}