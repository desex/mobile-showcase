using UnityEngine;

namespace TierStudios.UIParticleSystem.Settings
{
    [System.Serializable]
    public class ColorBySpeedSettings
    {
        public bool useColorBySpeed = false;
        public bool useRandomColors = false;
        public Gradient colorGradient = new Gradient();
        public Gradient colorGradient2 = new Gradient();
        public float minSpeed = 0f;
        public float maxSpeed = 10f;
    }
}
