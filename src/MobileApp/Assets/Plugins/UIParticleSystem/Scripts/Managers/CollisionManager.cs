using UnityEngine;
using UnityEngine.UI;
using TierStudios.UIParticleSystem.Core;
using TierStudios.UIParticleSystem.Enums;
using TierStudios.UIParticleSystem.Settings;

namespace TierStudios.UIParticleSystem.Managers
{
    public class CollisionManager
    {
        private readonly Core.UIParticleSystem _system;

        public CollisionManager(Core.UIParticleSystem system)
        {
            _system = system;
        }

        public bool Resolve(Particle particle, float deltaTime)
        {
            CollisionSettings settings = _system.CollisionSettings;
            if (settings == null || !settings.useCollision || particle == null || !particle.isAlive)
                return false;

            Vector3 oldPosition = particle.position;
            bool collided = false;

            if (settings.useBounds)
            {
                RectTransform bounds = settings.boundsTarget != null
                    ? settings.boundsTarget
                    : _system.GetComponent<RectTransform>();
                if (bounds != null && ResolveAgainst(particle, bounds, settings, deltaTime, true))
                    collided = true;
            }

            var obstacles = settings.obstacles;
            if (obstacles != null)
            {
                RectTransform boundsRt = settings.useBounds
                    ? (settings.boundsTarget != null ? settings.boundsTarget : _system.GetComponent<RectTransform>())
                    : null;

                for (int i = 0; i < obstacles.Count; i++)
                {
                    RectTransform obstacle = obstacles[i];
                    if (obstacle == null || obstacle == boundsRt || !obstacle.gameObject.activeInHierarchy)
                        continue;
                    if (ResolveAgainst(particle, obstacle, settings, deltaTime, false))
                        collided = true;
                }
            }

            if (!collided)
                return false;

            Vector3 delta = particle.position - oldPosition;
            particle.points[0] += delta;
            particle.points[1] += delta;
            particle.points[2] += delta;
            particle.points[3] += delta;

            particle.collisionCount++;
            ApplyLifetimeLoss(particle, settings);

            if (settings.killOnCollision)
                particle.isAlive = false;
            else if (settings.maxCollisionCount > 0 && particle.collisionCount >= settings.maxCollisionCount)
                particle.isAlive = false;
            else if (settings.minKillSpeed > 0f && particle.velocity.magnitude < settings.minKillSpeed)
                particle.isAlive = false;

            return true;
        }

        private bool ResolveAgainst(Particle particle, RectTransform collider, CollisionSettings settings, float deltaTime, bool insideBounds)
        {
            bool isLocal = _system.EmissionSettings.simulationSpace == SimulationSpace.Local;
            Transform systemTransform = _system.Transform;

            Vector3 worldPos = isLocal ? systemTransform.TransformPoint(particle.position) : particle.position;
            Vector3 worldVel = isLocal ? systemTransform.TransformVector(particle.velocity) : particle.velocity;

            float dt = Mathf.Max(deltaTime, 0.0001f);
            Vector3 prevWorld = worldPos - worldVel * dt;

            Vector2 localPos = collider.InverseTransformPoint(worldPos);
            Vector2 localVel = collider.InverseTransformVector(worldVel);
            Vector2 prevLocal = collider.InverseTransformPoint(prevWorld);

            float worldRadius = GetCollisionRadius(particle, settings);
            Vector3 lossy = collider.lossyScale;
            float radiusX = worldRadius / Mathf.Max(0.0001f, Mathf.Abs(lossy.x));
            float radiusY = worldRadius / Mathf.Max(0.0001f, Mathf.Abs(lossy.y));

            Rect rect = GetColliderRect(collider);
            if (rect.width <= 0.01f || rect.height <= 0.01f)
                return false;

            bool hit = insideBounds
                ? ResolveInside(ref localPos, ref localVel, rect, radiusX, radiusY, settings)
                : ResolveOutside(ref localPos, ref localVel, prevLocal, rect, radiusX, radiusY, settings);

            if (!hit)
                return false;

            Vector3 newWorldPos = collider.TransformPoint(localPos);
            Vector3 newWorldVel = collider.TransformVector(localVel);

            particle.position = isLocal ? systemTransform.InverseTransformPoint(newWorldPos) : newWorldPos;
            Vector3 newVel = isLocal ? systemTransform.InverseTransformVector(newWorldVel) : newWorldVel;
            particle.velocity = newVel;
            particle.originalVelocity = newVel;
            return true;
        }

        private static bool ResolveInside(ref Vector2 pos, ref Vector2 vel, Rect rect, float radiusX, float radiusY, CollisionSettings settings)
        {
            float minX = rect.xMin + radiusX;
            float maxX = rect.xMax - radiusX;
            float minY = rect.yMin + radiusY;
            float maxY = rect.yMax - radiusY;

            if (minX > maxX)
                minX = maxX = rect.center.x;
            if (minY > maxY)
                minY = maxY = rect.center.y;

            bool hitX = false;
            bool hitY = false;
            int signX = 0;
            int signY = 0;

            if (pos.x < minX)
            {
                pos.x = minX;
                signX = 1;
                hitX = true;
            }
            else if (pos.x > maxX)
            {
                pos.x = maxX;
                signX = -1;
                hitX = true;
            }

            if (pos.y < minY)
            {
                pos.y = minY;
                signY = 1;
                hitY = true;
            }
            else if (pos.y > maxY)
            {
                pos.y = maxY;
                signY = -1;
                hitY = true;
            }

            if (!hitX && !hitY)
                return false;

            Reflect(ref vel, hitX, signX, hitY, signY, settings.bounce);
            ApplyBounceModifiers(ref vel, hitX, hitY, settings);
            return true;
        }

        private static bool ResolveOutside(ref Vector2 pos, ref Vector2 vel, Vector2 prev, Rect rect, float radiusX, float radiusY, CollisionSettings settings)
        {
            const float Skin = 0.75f;

            float minX = rect.xMin - radiusX;
            float maxX = rect.xMax + radiusX;
            float minY = rect.yMin - radiusY;
            float maxY = rect.yMax + radiusY;

            bool currInside = pos.x >= minX && pos.x <= maxX && pos.y >= minY && pos.y <= maxY;
            bool prevInside = prev.x >= minX && prev.x <= maxX && prev.y >= minY && prev.y <= maxY;

            int axis;
            int sign;
            bool swept = TrySweepAabb(prev, pos, minX, maxX, minY, maxY, out float tHit, out axis, out sign);

            if (prevInside || (currInside && !swept))
            {
                GetMinPenetrationAxis(pos, minX, maxX, minY, maxY, out axis, out sign);
            }
            else if (!swept || tHit > 1.0001f)
            {
                return false;
            }

            bool hitX = axis == 0;
            bool hitY = axis == 1;

            if (hitX)
                pos.x = (sign < 0 ? minX : maxX) + sign * Skin;
            else
                pos.y = (sign < 0 ? minY : maxY) + sign * Skin;

            Reflect(ref vel, hitX, sign, hitY, sign, settings.bounce);
            ApplyBounceModifiers(ref vel, hitX, hitY, settings);
            return true;
        }

        private static void Reflect(ref Vector2 vel, bool hitX, int signX, bool hitY, int signY, float bounce)
        {
            bounce = Mathf.Max(0f, bounce);

            if (hitX && signX != 0)
            {
                float outward = signX;
                if (vel.x * outward < 0f)
                    vel.x = -vel.x * bounce;
                else if (bounce > 0.001f && Mathf.Abs(vel.x) < 0.01f)
                    vel.x = outward * bounce;
            }

            if (hitY && signY != 0)
            {
                float outward = signY;
                if (vel.y * outward < 0f)
                    vel.y = -vel.y * bounce;
                else if (bounce > 0.001f && Mathf.Abs(vel.y) < 0.01f)
                    vel.y = outward * bounce;
            }
        }

        private static void GetMinPenetrationAxis(Vector2 pos, float minX, float maxX, float minY, float maxY, out int axis, out int sign)
        {
            float penLeft = pos.x - minX;
            float penRight = maxX - pos.x;
            float penBottom = pos.y - minY;
            float penTop = maxY - pos.y;

            float minPen = penLeft;
            axis = 0;
            sign = -1;
            if (penRight < minPen) { minPen = penRight; axis = 0; sign = 1; }
            if (penBottom < minPen) { minPen = penBottom; axis = 1; sign = -1; }
            if (penTop < minPen) { axis = 1; sign = 1; }
        }

        private static bool TrySweepAabb(Vector2 a, Vector2 b, float minX, float maxX, float minY, float maxY,
            out float tHit, out int axis, out int sign)
        {
            tHit = 0f;
            axis = 0;
            sign = 1;

            float tmin = 0f;
            float tmax = 1f;
            int enterAxis = 0;
            int enterSign = 1;

            if (!SweepSlab(a.x, b.x - a.x, minX, maxX, 0, ref tmin, ref tmax, ref enterAxis, ref enterSign))
                return false;
            if (!SweepSlab(a.y, b.y - a.y, minY, maxY, 1, ref tmin, ref tmax, ref enterAxis, ref enterSign))
                return false;

            if (tmin > tmax || tmin > 1f || tmax < 0f)
                return false;

            tHit = tmin;
            axis = enterAxis;
            sign = enterSign;
            return true;
        }

        private static bool SweepSlab(float start, float dir, float min, float max, int slabAxis,
            ref float tmin, ref float tmax, ref int enterAxis, ref int enterSign)
        {
            const float eps = 0.000001f;
            if (Mathf.Abs(dir) < eps)
                return start >= min && start <= max;

            float t1 = (min - start) / dir;
            float t2 = (max - start) / dir;
            int s1 = -1;
            int s2 = 1;
            if (t1 > t2)
            {
                float tmp = t1;
                t1 = t2;
                t2 = tmp;
                int st = s1;
                s1 = s2;
                s2 = st;
            }

            if (t1 > tmin)
            {
                tmin = t1;
                enterAxis = slabAxis;
                enterSign = s1;
            }

            if (t2 < tmax)
                tmax = t2;

            return tmin <= tmax;
        }

        private static Rect GetColliderRect(RectTransform collider)
        {
            var image = collider.GetComponent<Image>();
            if (image != null && image.enabled && image.sprite != null)
                return GetImageDrawingRect(image);

            return collider.rect;
        }

        private static Rect GetImageDrawingRect(Image image)
        {
            Rect rect = image.rectTransform.rect;
            if (!image.preserveAspect || image.sprite == null)
                return rect;

            Rect spriteRect = image.sprite.rect;
            if (spriteRect.height <= 0f || rect.height <= 0f)
                return rect;

            float aspect = spriteRect.width / spriteRect.height;
            float rectAspect = rect.width / rect.height;
            if (aspect > rectAspect)
            {
                float height = rect.width / aspect;
                rect.y += (rect.height - height) * 0.5f;
                rect.height = height;
            }
            else
            {
                float width = rect.height * aspect;
                rect.x += (rect.width - width) * 0.5f;
                rect.width = width;
            }

            return rect;
        }

        private static void ApplyBounceModifiers(ref Vector2 vel, bool hitX, bool hitY, CollisionSettings settings)
        {
            if (settings.friction > 0f)
            {
                float keep = 1f - settings.friction;
                if (hitX && !hitY) vel.y *= keep;
                else if (hitY && !hitX) vel.x *= keep;
                else
                {
                    vel.x *= keep;
                    vel.y *= keep;
                }
            }

            if (settings.dampen > 0f)
                vel *= 1f - settings.dampen;

            if (settings.bounceSpread > 0.001f)
            {
                float angle = Random.Range(-settings.bounceSpread, settings.bounceSpread);
                vel = Quaternion.Euler(0f, 0f, angle) * vel;
            }
        }

        private static float GetCollisionRadius(Particle particle, CollisionSettings settings)
        {
            float size = particle.size;
            if (particle.size2D.x > 0f || particle.size2D.y > 0f)
                size = Mathf.Max(particle.size2D.x, particle.size2D.y);
            if (size < 1f)
                size = 1f;
            return Mathf.Max(0f, size * 0.5f * settings.radiusScale);
        }

        private static void ApplyLifetimeLoss(Particle particle, CollisionSettings settings)
        {
            if (settings.lifetimeLoss <= 0f)
                return;

            float remaining = Mathf.Max(0f, particle.maxLifetime - particle.age);
            remaining *= 1f - settings.lifetimeLoss;
            particle.maxLifetime = particle.age + remaining;
            particle.lifetime = remaining;
            if (remaining <= 0f)
                particle.isAlive = false;
        }
    }
}
