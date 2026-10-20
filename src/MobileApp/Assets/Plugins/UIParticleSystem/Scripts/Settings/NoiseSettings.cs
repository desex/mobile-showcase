using UnityEngine;
using TierStudios.UIParticleSystem.Enums;

namespace TierStudios.UIParticleSystem.Settings
{
    [System.Serializable]
    public class NoiseSettings
    {
        public bool useNoise = false;
        public float noiseStrength = 10f;
        public float noiseFrequency = 1f;
        public Vector2 noiseOffset = Vector2.zero;

        public bool separateAxisNoise = false;
        public float noiseStrengthX = 10f;
        public float noiseStrengthY = 10f;
        public float noiseFrequencyX = 1f;
        public float noiseFrequencyY = 1f;

        public NoiseType noiseType = NoiseType.Perlin;
        public bool animateNoise = true;
        public float noiseSpeed = 1f;
    }
}