using UnityEngine;

namespace TierStudios.UIParticleSystem.Managers
{
    public class PerformanceMonitor
    {
        private Core.UIParticleSystem system;
        private int lastParticleCount = 0;
        private float lastPerformanceCheck = 0f;

        public PerformanceMonitor(Core.UIParticleSystem particleSystem)
        {
            system = particleSystem;
        }

        public void CheckPerformance()
        {
#if UNITY_EDITOR
            if (Time.time - lastPerformanceCheck > 1f)
            {
                if (system.TextureSheetSettings.useTextureSheetAnimation && system.ActiveParticles.Count > 1000)
                {
                    Debug.LogWarning($"UIParticleSystem: High particle count with animation: {system.ActiveParticles.Count}. " +
                                    $"Consider reducing max particles or animation speed.");
                }

                lastParticleCount = system.ActiveParticles.Count;
                lastPerformanceCheck = Time.time;
            }
#endif
        }
    }
}