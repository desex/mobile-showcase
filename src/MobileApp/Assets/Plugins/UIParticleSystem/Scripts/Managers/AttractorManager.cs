using UnityEngine;
using TierStudios.UIParticleSystem.Core;
using TierStudios.UIParticleSystem.Enums;
using TierStudios.UIParticleSystem.Settings;

namespace TierStudios.UIParticleSystem.Managers
{
    public class AttractorManager
    {
        private Core.UIParticleSystem system;

        private Vector2 cachedAttractorLocalPos;
        private int lastCachedFrame = -1;

        private const float MinDistance = 0.0001f;
        private const float MinDeltaTime = 0.00001f;

        public AttractorManager(Core.UIParticleSystem particleSystem)
        {
            system = particleSystem;
        }

        public void InvalidatePositionCache()
        {
            lastCachedFrame = -1;
        }

        public void ApplyAttractor(Particle particle, float deltaTime)
        {
            AttractorSettings settings = system.AttractorSettings;

            if (!settings.useAttractor || (settings.attractorStrength < 0.001f && settings.attractorType != AttractorType.Direct))
                return;

            if (settings.useAttractorLifetimeLimit)
            {
                float normalizedLifetime = particle.GetNormalizedAge();
                if (normalizedLifetime < settings.attractorMinLifetime ||
                    normalizedLifetime > settings.attractorMaxLifetime)
                    return;
            }

            float safeDt = Mathf.Max(deltaTime, MinDeltaTime);

            if (settings.attractorType == AttractorType.Direct)
            {
                ApplyDirectMove(particle, settings, safeDt);
                return;
            }

            Vector2 attractorPos = GetAttractorPositionInParticleSpace();
            Vector2 particlePos = (Vector2)particle.position;
            Vector2 toAttractor = attractorPos - particlePos;
            float distance = toAttractor.magnitude;

            if (distance > settings.attractorRange)
            {
                particle.attractorFadeAlpha = 1f;
                return;
            }

            if (distance < MinDistance)
            {
                if (settings.attractorKillOnReach)
                {
                    particle.isAlive = false;
                    return;
                }

                particle.attractorFadeAlpha = settings.attractorFadeOnApproach ? 0f : 1f;
                particle.velocity = Vector3.zero;
                return;
            }

            if (settings.attractorKillOnReach)
            {
                if (distance <= settings.attractorKillRadius)
                {
                    particle.isAlive = false;
                    return;
                }

                Vector2 frameMovement = (Vector2)particle.velocity * safeDt;
                float moveLength = frameMovement.magnitude;

                if (moveLength > 0.01f)
                {
                    Vector2 nextPos = particlePos + frameMovement;
                    float closestDist = PointToSegmentDistance(attractorPos, particlePos, nextPos);

                    if (closestDist <= settings.attractorKillRadius)
                    {
                        particle.isAlive = false;
                        return;
                    }
                }
            }

            UpdateFadeAlpha(particle, settings, distance);

            float normalizedDistance = Mathf.Clamp01(distance / Mathf.Max(settings.attractorRange, MinDistance));
            float falloff = settings.attractorFalloff != null
                ? settings.attractorFalloff.Evaluate(normalizedDistance)
                : 1f - normalizedDistance;
            Vector2 dirNormalized = toAttractor / distance;

            switch (settings.attractorType)
            {
                case AttractorType.Attract:
                    ApplyAttractionForce(particle, dirNormalized, distance, settings, falloff, safeDt);
                    break;

                case AttractorType.Repel:
                    ApplyRepelForce(particle, dirNormalized, settings, falloff, safeDt);
                    break;

                case AttractorType.Orbital:
                    ApplyOrbitalForce(particle, dirNormalized, settings, falloff, safeDt);
                    break;

                case AttractorType.Vortex:
                    ApplyAttractionForce(particle, dirNormalized, distance, settings, falloff * 0.5f, safeDt);
                    ApplyOrbitalForce(particle, dirNormalized, settings, falloff, safeDt);
                    break;
            }

            SanitizeParticle(particle);
        }

        private void ApplyDirectMove(Particle particle, AttractorSettings settings, float deltaTime)
        {
            Vector3 targetLocalPos = ResolveDirectTargetLocalPosition(settings);
            if (!IsFinite(targetLocalPos))
                return;

            float distance = Vector3.Distance(particle.position, targetLocalPos);

            if (settings.attractorKillOnReach && distance <= settings.attractorKillRadius)
            {
                particle.isAlive = false;
                return;
            }

            UpdateFadeAlpha(particle, settings, distance);

            float normalizedAge = particle.GetNormalizedAge();
            float lerpWeight = settings.attractorLerp != null
                ? Mathf.Clamp01(settings.attractorLerp.Evaluate(normalizedAge))
                : 1f;

            // Strength 100 ≈ reach target in ~1s at 60fps; always clamp to avoid overshoot/NaN.
            float t = Mathf.Clamp01(lerpWeight * (settings.attractorStrength * 0.01f) * deltaTime * 60f);

            particle.position = Vector3.Lerp(particle.position, targetLocalPos, t);
            particle.velocity = Vector3.Lerp(particle.velocity, Vector3.zero, t);

            SanitizeParticle(particle);
        }

        private Vector3 ResolveDirectTargetLocalPosition(AttractorSettings settings)
        {
            if (settings.attractorTarget != null)
            {
                Vector3 targetLocalPos = system.transform.InverseTransformPoint(settings.attractorTarget.position);
                targetLocalPos.z = 0f;
                return targetLocalPos;
            }

            // Fallback: same position resolution as Attract/Repel (manual / mouse / relative).
            Vector2 pos = GetAttractorPositionInParticleSpace();
            return new Vector3(pos.x, pos.y, 0f);
        }

        private static void UpdateFadeAlpha(Particle particle, AttractorSettings settings, float distance)
        {
            if (settings.attractorKillOnReach && settings.attractorFadeOnApproach)
            {
                if (distance <= settings.attractorFadeStartDistance)
                {
                    float fadeRange = settings.attractorFadeStartDistance - settings.attractorKillRadius;
                    if (fadeRange > 0.001f)
                    {
                        float distFromKill = distance - settings.attractorKillRadius;
                        particle.attractorFadeAlpha = Mathf.Clamp01(distFromKill / fadeRange);
                    }
                    else
                    {
                        particle.attractorFadeAlpha = 0f;
                    }
                }
                else
                {
                    particle.attractorFadeAlpha = 1f;
                }
            }
            else
            {
                particle.attractorFadeAlpha = 1f;
            }
        }

        private void ApplyAttractionForce(Particle particle, Vector2 dirNormalized, float distance,
            AttractorSettings settings, float falloff, float deltaTime)
        {
            float proximityBrake = Mathf.Clamp01(distance / (settings.attractorKillRadius > 0
                ? settings.attractorKillRadius * 3f
                : Mathf.Max(settings.attractorRange * 0.1f, MinDistance)));

            float force = settings.attractorStrength * falloff * proximityBrake;
            if (force < 0.0001f || !IsFinite(force))
                return;

            float maxApproachSpeed = distance / deltaTime;
            if (!IsFinite(maxApproachSpeed))
                maxApproachSpeed = force;

            Vector2 desiredVelocity = dirNormalized * Mathf.Min(force, maxApproachSpeed);
            Vector2 currentVel2D = (Vector2)particle.velocity;
            Vector2 steering = desiredVelocity - currentVel2D;

            float maxSteer = force * deltaTime;
            if (steering.sqrMagnitude > maxSteer * maxSteer && steering.sqrMagnitude > MinDistance)
                steering = steering.normalized * maxSteer;

            if (!IsFinite(steering))
                return;

            // VOL overwrites velocity each frame; apply Δv and same-frame displacement (steering already includes dt).
            particle.velocity += (Vector3)steering;
            particle.position += (Vector3)steering;

            float maxSpeed = Mathf.Max(settings.attractorRange * 2f, force);
            float currentSpeed = particle.velocity.magnitude;
            if (currentSpeed > maxSpeed && currentSpeed > MinDistance)
                particle.velocity = particle.velocity.normalized * maxSpeed;
        }

        private void ApplyRepelForce(Particle particle, Vector2 dirNormalized,
            AttractorSettings settings, float falloff, float deltaTime)
        {
            Vector2 repelForce = -dirNormalized * settings.attractorStrength * falloff * deltaTime;
            if (!IsFinite(repelForce))
                return;

            particle.velocity += (Vector3)repelForce;
            particle.position += (Vector3)repelForce;
        }

        private void ApplyOrbitalForce(Particle particle, Vector2 dirNormalized,
            AttractorSettings settings, float falloff, float deltaTime)
        {
            Vector2 tangent = new Vector2(-dirNormalized.y, dirNormalized.x);
            Vector2 orbitalForce = tangent * settings.orbitalSpeed * falloff * deltaTime;
            if (!IsFinite(orbitalForce))
                return;

            particle.velocity += (Vector3)orbitalForce;
            particle.position += (Vector3)orbitalForce;
        }

        private static void SanitizeParticle(Particle particle)
        {
            if (!IsFinite(particle.position) || !IsFinite(particle.velocity))
            {
                particle.position = Vector3.zero;
                particle.velocity = Vector3.zero;
                particle.isAlive = false;
            }
        }

        private static bool IsFinite(Vector2 v)
        {
            return IsFinite(v.x) && IsFinite(v.y);
        }

        private static bool IsFinite(Vector3 v)
        {
            return IsFinite(v.x) && IsFinite(v.y) && IsFinite(v.z);
        }

        private static bool IsFinite(float v)
        {
            return !float.IsNaN(v) && !float.IsInfinity(v);
        }

        private Vector2 GetAttractorPositionInParticleSpace()
        {
            int currentFrame = Time.frameCount;
            if (lastCachedFrame == currentFrame)
                return cachedAttractorLocalPos;

            Vector2 worldPos = GetWorldAttractorPosition();

            if (system.EmissionSettings.simulationSpace == SimulationSpace.Local)
            {
                cachedAttractorLocalPos = system.transform.InverseTransformPoint(worldPos);
            }
            else
            {
                cachedAttractorLocalPos = worldPos;
            }

            lastCachedFrame = currentFrame;
            return cachedAttractorLocalPos;
        }

        private Vector2 GetWorldAttractorPosition()
        {
            AttractorSettings settings = system.AttractorSettings;

            if (settings.attractorTarget != null)
            {
                return settings.attractorTarget.position;
            }

            if (settings.attractorFollowsMouse)
            {
                return GetMouseWorldPosition();
            }

            if (settings.attractorRelativeToEmitter)
            {
                return system.transform.TransformPoint(settings.attractorPosition);
            }

            return settings.attractorPosition;
        }

        private Vector2 GetMouseWorldPosition()
        {
            Vector3 mousePosition = Input.mousePosition;
            Canvas canvas = system.CachedCanvas;

            if (canvas == null)
                canvas = system.GetComponentInParent<Canvas>();

            if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform as RectTransform,
                    mousePosition,
                    canvas.worldCamera,
                    out Vector2 localPoint
                );
                return canvas.transform.TransformPoint(localPoint);
            }

            return Camera.main != null
                ? Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0f))
                : (Vector2)mousePosition;
        }

        private static float PointToSegmentDistance(Vector2 point, Vector2 segA, Vector2 segB)
        {
            Vector2 ab = segB - segA;
            float abLenSq = ab.sqrMagnitude;

            if (abLenSq < 0.0001f)
                return Vector2.Distance(point, segA);

            float t = Mathf.Clamp01(Vector2.Dot(point - segA, ab) / abLenSq);
            Vector2 closest = segA + t * ab;
            return Vector2.Distance(point, closest);
        }

#if UNITY_EDITOR
        public void DrawAttractorGizmos()
        {
            AttractorSettings settings = system.AttractorSettings;
            if (!settings.useAttractor) return;

            Vector3 center;

            if (settings.attractorType == AttractorType.Direct)
            {
                if (settings.attractorTarget != null)
                    center = settings.attractorTarget.position;
                else
                    center = (Vector3)GetWorldAttractorPosition();

                if (settings.attractorKillOnReach)
                {
                    Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.8f);
                    Gizmos.DrawWireSphere(center, settings.attractorKillRadius);

                    if (settings.attractorFadeOnApproach)
                    {
                        Gizmos.color = new Color(1f, 0.8f, 0.2f, 0.4f);
                        Gizmos.DrawWireSphere(center, settings.attractorFadeStartDistance);
                    }
                }

                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(center + Vector3.up * 10f, center - Vector3.up * 10f);
                Gizmos.DrawLine(center + Vector3.right * 10f, center - Vector3.right * 10f);
            }
            else
            {
                center = (Vector3)GetWorldAttractorPosition();

                Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.3f);
                Gizmos.DrawWireSphere(center, settings.attractorRange);

                if (settings.attractorKillOnReach)
                {
                    Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.8f);
                    Gizmos.DrawWireSphere(center, settings.attractorKillRadius);

                    if (settings.attractorFadeOnApproach)
                    {
                        Gizmos.color = new Color(1f, 0.8f, 0.2f, 0.4f);
                        Gizmos.DrawWireSphere(center, settings.attractorFadeStartDistance);
                    }
                }
            }
        }
#endif
    }
}
