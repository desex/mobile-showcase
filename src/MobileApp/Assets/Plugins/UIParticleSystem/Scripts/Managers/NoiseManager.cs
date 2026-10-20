using UnityEngine;
using TierStudios.UIParticleSystem.Enums;

namespace TierStudios.UIParticleSystem.Managers
{
    public class NoiseManager
    {
        private Core.UIParticleSystem system;

        private struct NoiseCache
        {
            public float lastX;
            public float lastY;
            public float lastTime;
            public Vector2 cachedResult;
            public bool isValid;
        }

        private NoiseCache cache;
        private const float CACHE_TOLERANCE = 0.01f;

        public NoiseManager(Core.UIParticleSystem particleSystem)
        {
            system = particleSystem;
            cache = new NoiseCache { isValid = false };
        }

        public Vector2 GetNoiseForce(Vector2 position, float time, float particleNoiseSeed)
        {
            if (!system.NoiseSettings.useNoise || system.NoiseSettings.noiseStrength <= 0f)
                return Vector2.zero;

            bool isMobile = system.MobileOptimizationMode;
            float cacheTolerance = isMobile ? CACHE_TOLERANCE * 5f : CACHE_TOLERANCE;

            Vector2 force = Vector2.zero;
            float currentTime = system.NoiseSettings.animateNoise ? time * system.NoiseSettings.noiseSpeed : 0f;

            if (system.NoiseSettings.separateAxisNoise)
            {
                float noiseX = GetNoiseOptimized(
                    (position.x + system.NoiseSettings.noiseOffset.x + particleNoiseSeed) * system.NoiseSettings.noiseFrequencyX * 0.01f,
                    (position.y + system.NoiseSettings.noiseOffset.y) * system.NoiseSettings.noiseFrequencyX * 0.01f,
                    currentTime,
                    system.NoiseSettings.noiseType
                );

                float noiseY = GetNoiseOptimized(
                    (position.x + system.NoiseSettings.noiseOffset.x) * system.NoiseSettings.noiseFrequencyY * 0.01f,
                    (position.y + system.NoiseSettings.noiseOffset.y + particleNoiseSeed + 1000f) * system.NoiseSettings.noiseFrequencyY * 0.01f,
                    currentTime,
                    system.NoiseSettings.noiseType
                );

                force.x = noiseX * system.NoiseSettings.noiseStrengthX;
                force.y = noiseY * system.NoiseSettings.noiseStrengthY;
            }
            else
            {
                float noiseX = GetNoiseOptimized(
                    (position.x + system.NoiseSettings.noiseOffset.x + particleNoiseSeed) * system.NoiseSettings.noiseFrequency * 0.01f,
                    (position.y + system.NoiseSettings.noiseOffset.y) * system.NoiseSettings.noiseFrequency * 0.01f,
                    currentTime,
                    system.NoiseSettings.noiseType
                );

                float noiseY = GetNoiseOptimized(
                    (position.x + system.NoiseSettings.noiseOffset.x + particleNoiseSeed + 1000f) * system.NoiseSettings.noiseFrequency * 0.01f,
                    (position.y + system.NoiseSettings.noiseOffset.y + 1000f) * system.NoiseSettings.noiseFrequency * 0.01f,
                    currentTime,
                    system.NoiseSettings.noiseType
                );

                force.x = noiseX * system.NoiseSettings.noiseStrength;
                force.y = noiseY * system.NoiseSettings.noiseStrength;
            }

            return force;
        }

        private float GetNoiseOptimized(float x, float y, float time, NoiseType type)
        {
            switch (type)
            {
                case NoiseType.Perlin:
                    return Mathf.PerlinNoise(x + time, y + time) * 2f - 1f;

                case NoiseType.Turbulence:
                    int octaves = system.MobileOptimizationMode ? 2 : 4;
                    return GetTurbulenceNoise(x, y, time, octaves);

                case NoiseType.Ridge:
                    float ridge = Mathf.PerlinNoise(x + time, y + time);
                    ridge = 1f - Mathf.Abs(ridge * 2f - 1f);
                    return ridge * 2f - 1f;

                case NoiseType.Billow:
                    float billow = Mathf.PerlinNoise(x + time, y + time);
                    billow = Mathf.Abs(billow * 2f - 1f);
                    return billow * 2f - 1f;

                default:
                    return 0f;
            }
        }

        private float GetTurbulenceNoise(float x, float y, float time, int octaves)
        {
            float turbulence = 0f;
            float amplitude = 1f;
            float frequency = 1f;

            for (int i = 0; i < octaves; i++)
            {
                turbulence += Mathf.PerlinNoise(x * frequency + time, y * frequency + time) * amplitude;
                amplitude *= 0.5f;
                frequency *= 2f;
            }

            return Mathf.Clamp(turbulence - 0.5f, -1f, 1f);
        }
    }
}