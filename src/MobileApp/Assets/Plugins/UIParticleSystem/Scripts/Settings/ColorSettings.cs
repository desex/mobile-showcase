using UnityEngine;
using TierStudios.UIParticleSystem.Enums;

namespace TierStudios.UIParticleSystem.Settings
{
    [System.Serializable]
    public class ColorSettings
    {
        public bool useRandomColors = false;
        public bool useGradientColor = false;
        public Color particleColor = Color.white;
        public Color particleColorEnd = Color.white;

        public Gradient colorGradient = new Gradient();
        public Gradient colorGradient2 = new Gradient();

        public bool fadeOut = false;
    }
}