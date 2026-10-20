using TierStudios.UIParticleSystem.Core;
using TierStudios.UIParticleSystem.Settings;
using UnityEngine;

namespace TierStudios.UIParticleSystem.Managers
{
    public class VelocityOverLifetimeManager
    {
        private Core.UIParticleSystem system;

        public VelocityOverLifetimeManager(Core.UIParticleSystem particleSystem)
        {
            system = particleSystem;
        }

        public static void InitializeParticleRandomValues(Particle particle, VelocityOverLifetimeSettings settings)
        {
            if (!settings.useVelocityOverLifetime)
                return;

            if (settings.useLinearVelocity && settings.useRandomLinearVelocity)
            {
                if (settings.useLinearVelocityCurves)
                {
                    particle.velocityRandomLinearOffset = new Vector2(Random.value, Random.value);
                }
                else
                {
                    particle.velocityRandomLinearOffset = new Vector2(
                        Random.Range(settings.linearVelocityMin.x, settings.linearVelocityMax.x),
                        Random.Range(settings.linearVelocityMin.y, settings.linearVelocityMax.y)
                    );
                }
            }

            if (settings.useOrbitalVelocity && settings.useRandomOrbitalVelocity)
            {
                if (settings.useOrbitalVelocityCurve)
                {
                    particle.velocityRandomOrbitalOffset = Random.value;
                }
                else
                {
                    particle.velocityRandomOrbitalOffset = Random.Range(
                        settings.orbitalVelocityMin,
                        settings.orbitalVelocityMax
                    );
                }
            }
        }

        private const float UI_VELOCITY_MULTIPLIER = 100f;

        public void Apply(Particle particle, float deltaTime)
        {
            VelocityOverLifetimeSettings settings = system.VelocityOverLifetimeSettings;
            if (!settings.useVelocityOverLifetime)
                return;

            float normalizedAge = particle.GetNormalizedAge();

            if (settings.useLinearVelocity)
            {
                Vector2 linearVel = EvaluateLinearVelocity(settings, particle, normalizedAge) * UI_VELOCITY_MULTIPLIER;
                Vector3 linearForce = settings.useLocalSpace
                    ? (Vector3)linearVel
                    : system.transform.TransformDirection(linearVel);

                particle.velocity = particle.originalVelocity + linearForce;
            }

            if (settings.useOrbitalVelocity)
            {
                float speed = EvaluateOrbitalSpeed(settings, particle, normalizedAge) * UI_VELOCITY_MULTIPLIER;

                Vector2 center = settings.orbitalCenter;
                Vector2 relativePos = (Vector2)particle.position - center;
                float distance = relativePos.magnitude;

                if (distance > 0.001f)
                {
                    Vector2 tangent = new Vector2(-relativePos.y, relativePos.x) / distance;
                    Vector2 orbitalOffset = tangent * speed * deltaTime;

                    particle.position += (Vector3)orbitalOffset;
                }
            }
        }

        private static Vector2 EvaluateLinearVelocity(VelocityOverLifetimeSettings settings, Particle particle, float normalizedAge)
        {
            if (settings.useLinearVelocityCurves)
            {
                if (settings.useRandomLinearVelocity)
                {
                    float x = Mathf.Lerp(settings.linearVelocityXMinCurve.Evaluate(normalizedAge), settings.linearVelocityX.Evaluate(normalizedAge), particle.velocityRandomLinearOffset.x);
                    float y = Mathf.Lerp(settings.linearVelocityYMinCurve.Evaluate(normalizedAge), settings.linearVelocityY.Evaluate(normalizedAge), particle.velocityRandomLinearOffset.y);
                    return new Vector2(x, y);
                }
                else
                {
                    return new Vector2(
                        settings.linearVelocityX.Evaluate(normalizedAge),
                        settings.linearVelocityY.Evaluate(normalizedAge)
                    );
                }
            }

            if (settings.useRandomLinearVelocity)
            {
                return particle.velocityRandomLinearOffset * normalizedAge;
            }

            return settings.linearVelocity * normalizedAge;
        }

        private static float EvaluateOrbitalSpeed(VelocityOverLifetimeSettings settings, Particle particle, float normalizedAge)
        {
            if (settings.useOrbitalVelocityCurve)
            {
                if (settings.useRandomOrbitalVelocity)
                {
                    return Mathf.Lerp(settings.orbitalVelocityMinCurve.Evaluate(normalizedAge), settings.orbitalVelocityCurve.Evaluate(normalizedAge), particle.velocityRandomOrbitalOffset);
                }
                else
                {
                    return settings.orbitalVelocityCurve.Evaluate(normalizedAge) * settings.orbitalVelocity;
                }
            }

            if (settings.useRandomOrbitalVelocity)
            {
                return particle.velocityRandomOrbitalOffset;
            }

            return settings.orbitalVelocity;
        }
    }
}