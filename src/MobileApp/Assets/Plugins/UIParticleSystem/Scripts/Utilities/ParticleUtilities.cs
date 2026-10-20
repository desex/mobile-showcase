using UnityEngine;
using UnityEngine.UI;
using TierStudios.UIParticleSystem.Core;
using TierStudios.UIParticleSystem.Enums;
using TierStudios.UIParticleSystem.Managers;
using TierStudios.UIParticleSystem.Settings;

namespace TierStudios.UIParticleSystem.Utilities
{
    public static class ParticleUtilities
    {

        private static TextureSheetManager sharedTextureSheetManager;
        private static Core.UIParticleSystem sharedTextureSheetSystem;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            sharedTextureSheetManager = null;
            sharedTextureSheetSystem = null;
        }

        private static TextureSheetManager GetSharedTextureSheetManager(Core.UIParticleSystem system)
        {
            if (sharedTextureSheetManager == null || sharedTextureSheetSystem != system)
            {
                sharedTextureSheetManager = new TextureSheetManager(system);
                sharedTextureSheetSystem = system;
            }
            return sharedTextureSheetManager;
        }

        public static void EmitParticle(Core.UIParticleSystem system, Vector3? worldPositionOverride = null)
        {
            if (system.ActiveParticles.Count >= system.EffectiveMaxParticles)
                return;

            bool hasTextureSheet = system.TextureSheetSettings.useTextureSheetAnimation
                                   && system.TextureSheetSettings.textureSheet != null;
            bool hasSprite = system.ParticleSprite != null;

            if (!hasTextureSheet && !hasSprite)
            {
                Debug.LogWarning("UIParticleSystem: Neither Particle Sprite nor Texture Sheet is assigned!");
                return;
            }

            Particle particle = system.Pool.Get();

            Vector2 position2D = GetRandomPositionInShape(system);
            Vector3 startPos;

            if (system.EmissionSettings.simulationSpace == SimulationSpace.Local)
            {
                if (worldPositionOverride.HasValue)
                    position2D += (Vector2)system.Transform.InverseTransformPoint(worldPositionOverride.Value);

                startPos = position2D;
            }
            else
            {
                startPos = worldPositionOverride.HasValue
                    ? worldPositionOverride.Value + system.Transform.TransformVector(position2D)
                    : system.Transform.TransformPoint(position2D);
            }

            Vector2 particleSize2D;
            float particleSize;

            if (system.SizeSettings.use2DSize)
            {
                particleSize2D = system.SizeSettings.use2DSizeRange
                    ? new Vector2(
                        Random.Range(system.SizeSettings.minSize2D.x, system.SizeSettings.maxSize2D.x),
                        Random.Range(system.SizeSettings.minSize2D.y, system.SizeSettings.maxSize2D.y))
                    : system.SizeSettings.size2D;

                particleSize = Mathf.Max(particleSize2D.x, particleSize2D.y);
            }
            else
            {
                particleSize = system.SizeSettings.useSizeRange
                    ? Random.Range(system.SizeSettings.minSize, system.SizeSettings.maxSize)
                    : system.SizeSettings.size;
                particleSize2D = new Vector2(particleSize, particleSize);
            }

            Vector2 localVelocity = GetVelocityBasedOnShape(system, position2D);
            Vector3 particleVelocity = system.EmissionSettings.simulationSpace == SimulationSpace.Local
                ? (Vector3)localVelocity
                : system.Transform.TransformDirection(localVelocity);

            var inheritSettings = system.InheritVelocitySettings;
            if (inheritSettings != null && inheritSettings.useInheritVelocity
                && inheritSettings.mode == InheritVelocityMode.Initial)
            {
                particleVelocity += system.GetEmitterVelocityInSimSpace() * inheritSettings.multiplier;
            }

            bool useStretch = system.RendererSettings.renderMode == UIRenderMode.StretchedBillboard;
            float particleRotation, particleRotationSpeed;

            if (useStretch)
            {
                Vector2 vel2D = new Vector2(particleVelocity.x, particleVelocity.y);
                float speed = vel2D.magnitude;
                particleRotation = speed > 0.001f
                    ? Mathf.Atan2(vel2D.y, vel2D.x) * Mathf.Rad2Deg - 90f
                    : 0f;
                particleRotationSpeed = 0f;
            }
            else
            {
                particleRotation = system.RotationSettings.useRandomRotation
                    ? Random.Range(system.RotationSettings.minRotation, system.RotationSettings.maxRotation)
                    : system.RotationSettings.initialRotation;

                particleRotationSpeed = system.RotationSettings.useRotationOverLifetime
                    ? (system.RotationSettings.useRandomRotationSpeed
                        ? Random.Range(system.RotationSettings.minRotationSpeed, system.RotationSettings.maxRotationSpeed)
                        : system.RotationSettings.rotationSpeed)
                    : 0f;
            }

            var textureSheetManager = GetSharedTextureSheetManager(system);
            int startFrame = 0, endFrame = 0, initialFrame = 0;
            float initialAnimationTime = 0f;

            if (hasTextureSheet)
            {
                textureSheetManager.GetAnimationFrameRange(out startFrame, out endFrame);

                if (system.TextureSheetSettings.useRandomSingleFrame)
                {
                    initialFrame = Random.Range(startFrame, endFrame + 1);
                    startFrame = endFrame = initialFrame;
                }
                else if (system.TextureSheetSettings.randomStartFrame)
                {
                    initialFrame = Random.Range(startFrame, endFrame + 1);
                    int offset = initialFrame - startFrame;
                    if (system.TextureSheetSettings.timeMode == AnimationTimeMode.FramesPerSecond
                        && system.TextureSheetSettings.animationSpeed > 0f)
                        initialAnimationTime = offset / system.TextureSheetSettings.animationSpeed;
                }
                else
                {
                    initialFrame = startFrame;
                }
            }

            CalculateParticleColors(system,
                out Color initialColor, out Color endColor,
                out bool useGradient, out Gradient assignedGradient, out bool useSecondGradient);

            float particleLifetime = system.LifetimeSettings.useRandomLifetime
                ? Random.Range(system.LifetimeSettings.minLifetime, system.LifetimeSettings.maxLifetime)
                : system.LifetimeSettings.lifetime;

            if (system.TrailSettings.useTrails && system.CanUseTrails)
            {
                try { InitializeOptimizedTrailForParticle(system, particle, position2D, initialColor); }
                catch (System.Exception e) { Debug.LogError($"Trail init error: {e.Message}"); particle.trail = null; }
            }

            particle.noiseSeed = Random.Range(-1000f, 1000f);
            particle.originalVelocity = particleVelocity;

            if (system.SizeSettings.use2DSize)
                particle.Initialize(startPos, particleVelocity, particleLifetime, particleSize2D, initialColor);
            else
                particle.Initialize(startPos, particleVelocity, particleLifetime, particleSize, initialColor);

            VelocityOverLifetimeManager.InitializeParticleRandomValues(particle, system.VelocityOverLifetimeSettings);

            particle.rotation = particleRotation;
            particle.angularVelocity = particleRotationSpeed;
            particle.rotationSpeed = particleRotationSpeed;
            particle.initialScale = Vector3.one;
            particle.endColor = endColor;
            particle.useGradient = useGradient;
            particle.assignedGradient = assignedGradient;
            particle.useSecondGradient = useSecondGradient;
            particle.animationTime = initialAnimationTime;
            particle.currentFrame = initialFrame;
            particle.startFrame = startFrame;
            particle.endFrame = endFrame;
            particle.hasAnimation = hasTextureSheet;

            system.ActiveParticles.Add(particle);

            int particleIndex = system.ActiveParticles.Count - 1;
            system.TriggerParticleSpawned(particle, particleIndex);
        }

        private static void CalculateParticleColors(Core.UIParticleSystem system, out Color initialColor, out Color endColor, out bool useGradient, out Gradient assignedGradient, out bool useSecondGradient)
        {
            assignedGradient = null;
            useSecondGradient = false;

            if (system.ColorSettings.useRandomColors && !system.ColorSettings.useGradientColor)
            {
                initialColor = Random.value > 0.5f ? system.ColorSettings.particleColor : system.ColorSettings.particleColorEnd;
            }
            else
            {
                initialColor = system.ColorSettings.particleColor;
            }

            endColor = initialColor;

            if (system.ColorSettings.useGradientColor)
            {
                Gradient selectedGradient = system.ColorSettings.colorGradient;

                if (system.ColorSettings.useRandomColors)
                {
                    useSecondGradient = Random.value > 0.5f;
                    selectedGradient = useSecondGradient ? system.ColorSettings.colorGradient2 : system.ColorSettings.colorGradient;
                }

                assignedGradient = selectedGradient;
                useGradient = true;
            }
            else
            {
                useGradient = false;
            }
        }

        private static void InitializeOptimizedTrailForParticle(Core.UIParticleSystem system, Particle particle, Vector2 startPosition, Color particleColor)
        {
            var settings = system.TrailSettings;
            Color startColor, endColor;

            switch (settings.trailColorMode)
            {
                case Enums.TrailColorMode.Color:
                    startColor = endColor = settings.trailColor;
                    break;
                case Enums.TrailColorMode.RandomColor:
                    Color rc = Random.value < 0.5f ? settings.trailColor : settings.trailColorSecondary;
                    startColor = endColor = rc;
                    break;
                case Enums.TrailColorMode.Gradient:
                    startColor = settings.trailGradient.Evaluate(0f);
                    endColor = settings.trailGradient.Evaluate(1f);
                    break;
                case Enums.TrailColorMode.RandomGradient:
                    Gradient rg = Random.value < 0.5f ? settings.trailGradient : settings.trailGradientSecondary;
                    startColor = rg.Evaluate(0f);
                    endColor = rg.Evaluate(1f);
                    break;
                default:
                    startColor = particleColor;
                    endColor = new Color(particleColor.r, particleColor.g, particleColor.b, 0f);
                    break;
            }

            int trailCapacity = settings.trailMaxPoints;
            if (particle.trail == null)
                particle.trail = new ParticleTrail(trailCapacity);

            particle.trail.Clear();
            particle.trail.startColor = startColor;
            particle.trail.endColor = endColor;
            particle.trail.width = settings.trailWidth;
            particle.trail.lastUpdateTime = 0f;
            particle.trail.AddPoint(startPosition);
        }

        public static Vector2 GetRandomPositionInShape(Core.UIParticleSystem system)
        {
            Vector2 position = Vector2.zero;

            switch (system.EmissionSettings.emissionShape)
            {
                case EmissionShapeType.Rectangle:
                    if (system.EmissionSettings.emitFromEdge)
                    {
                        position = GetRandomPositionOnRectangleEdge(system.EmissionSettings.emissionAreaSize);
                    }
                    else
                    {
                        float randomX = Random.Range(-system.EmissionSettings.emissionAreaSize.x / 2, system.EmissionSettings.emissionAreaSize.x / 2);
                        float randomY = Random.Range(-system.EmissionSettings.emissionAreaSize.y / 2, system.EmissionSettings.emissionAreaSize.y / 2);
                        position = new Vector2(randomX, randomY);
                    }
                    break;

                case EmissionShapeType.Circle:
                    if (system.EmissionSettings.emitFromEdge)
                    {
                        float angle = Random.Range(0f, Mathf.PI * 2f);
                        position = new Vector2(Mathf.Cos(angle) * system.EmissionSettings.circleRadius, Mathf.Sin(angle) * system.EmissionSettings.circleRadius);
                    }
                    else
                    {
                        float angle = Random.Range(0f, Mathf.PI * 2f);
                        float radius = Random.Range(0f, system.EmissionSettings.circleRadius);
                        position = new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
                    }
                    break;

                case EmissionShapeType.Line:
                    position = new Vector2(Random.Range(-system.EmissionSettings.lineLength / 2, system.EmissionSettings.lineLength / 2), 0);
                    break;

                case EmissionShapeType.Cone:
                case EmissionShapeType.Point:
                    position = Vector2.zero;
                    break;

                case EmissionShapeType.Edge:
                    position = GetRandomPositionOnRectangleEdge(system.EmissionSettings.emissionAreaSize);
                    break;

                case EmissionShapeType.Sprite:
                    position = SpriteShapeSampler.GetRandomPoint(
                        system.EmissionSettings.shapeSprite,
                        system.EmissionSettings.shapeSpriteThreshold,
                        system.EmissionSettings.emitFromEdge,
                        system.EmissionSettings.shapeSpriteSize);
                    break;

                case EmissionShapeType.Image:
                    position = GetRandomPositionOnImage(system);
                    break;

                case EmissionShapeType.Text:
                    position = GetRandomPositionOnText(system);
                    break;
            }

            return position;
        }

        private static Vector2 GetRandomPositionOnImage(Core.UIParticleSystem system)
        {
            Image image = system.EmissionSettings.shapeImage;
            if (image == null || image.sprite == null || image.rectTransform == null)
                return Vector2.zero;

            if (!SpriteShapeSampler.TryGetRandomRelativePoint(
                    image.sprite,
                    system.EmissionSettings.shapeSpriteThreshold,
                    system.EmissionSettings.emitFromEdge,
                    out Vector2 relative))
                return Vector2.zero;

            Rect spriteRect = image.sprite.rect;
            Vector2 pivotN = spriteRect.width > 0f && spriteRect.height > 0f
                ? new Vector2(image.sprite.pivot.x / spriteRect.width, image.sprite.pivot.y / spriteRect.height)
                : new Vector2(0.5f, 0.5f);
            Vector2 normalized = relative + pivotN;

            Rect drawRect = GetImageDrawingRect(image);
            Vector3 imageLocal = new Vector3(
                drawRect.x + normalized.x * drawRect.width,
                drawRect.y + normalized.y * drawRect.height,
                0f);

            Vector3 world = image.rectTransform.TransformPoint(imageLocal);
            return system.Transform.InverseTransformPoint(world);
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

        private static Vector2 GetRandomPositionOnText(Core.UIParticleSystem system)
        {
            Graphic graphic = system.EmissionSettings.shapeText;
            if (graphic == null || graphic.rectTransform == null)
                return Vector2.zero;

            if (!TextShapeSampler.TryGetRandomLocalPoint(
                    graphic,
                    system.EmissionSettings.shapeSpriteThreshold,
                    system.EmissionSettings.emitFromEdge,
                    out Vector2 local))
                return Vector2.zero;

            Vector3 world = graphic.rectTransform.TransformPoint(local);
            return system.Transform.InverseTransformPoint(world);
        }

        private static Vector2 GetRandomPositionOnRectangleEdge(Vector2 areaSize)
        {
            float halfWidth = areaSize.x / 2;
            float halfHeight = areaSize.y / 2;

            int edge = Random.Range(0, 4);

            switch (edge)
            {
                case 0:
                    return new Vector2(Random.Range(-halfWidth, halfWidth), halfHeight);
                case 1:
                    return new Vector2(halfWidth, Random.Range(-halfHeight, halfHeight));
                case 2:
                    return new Vector2(Random.Range(-halfWidth, halfWidth), -halfHeight);
                case 3:
                    return new Vector2(-halfWidth, Random.Range(-halfHeight, halfHeight));
                default:
                    return Vector2.zero;
            }
        }

        public static Vector2 GetVelocityBasedOnShape(Core.UIParticleSystem system, Vector2 position)

        {
            Vector2 velocity;
            float currentSpeed = system.UseRandomSpeed ? Random.Range(system.MinSpeed, system.MaxSpeed) : system.Speed;

            switch (system.EmissionSettings.emissionShape)
            {
                case EmissionShapeType.Circle:
                    if (system.EmissionSettings.emitFromEdge)
                    {
                        Vector2 direction = position.normalized;
                        velocity = direction * currentSpeed;
                    }
                    else if (!system.EmissionSettings.randomizeDirection)
                    {
                        velocity = system.EmissionSettings.emissionDirection.normalized * currentSpeed;
                    }
                    else
                    {
                        float angle = Random.Range(0f, Mathf.PI * 2f);
                        velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * currentSpeed;
                    }
                    break;

                case EmissionShapeType.Cone:
                    float coneRandomAngle = Random.Range(-system.EmissionSettings.coneAngle / 2, system.EmissionSettings.coneAngle / 2);
                    Vector2 coneDirection = system.EmissionSettings.emissionDirection.normalized;
                    velocity = Quaternion.Euler(0, 0, coneRandomAngle) * coneDirection * currentSpeed;
                    break;

                case EmissionShapeType.Point:
                    if (!system.EmissionSettings.randomizeDirection)
                    {
                        velocity = system.EmissionSettings.emissionDirection.normalized * currentSpeed;
                    }
                    else
                    {
                        float angle = Random.Range(0f, Mathf.PI * 2f);
                        velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * currentSpeed;
                    }
                    break;

                default:
                    if (!system.EmissionSettings.randomizeDirection)
                    {
                        velocity = system.EmissionSettings.emissionDirection.normalized * currentSpeed;
                    }
                    else
                    {
                        float angle = Random.Range(0f, Mathf.PI * 2f);
                        velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * currentSpeed;
                    }
                    break;
            }

            if (!system.EmissionSettings.randomizeDirection && system.EmissionSettings.directionVariance > 0)
            {
                float variance = Random.Range(-system.EmissionSettings.directionVariance, system.EmissionSettings.directionVariance);
                velocity = Quaternion.Euler(0, 0, variance) * velocity;
            }

            return velocity;
        }

        public static void ClearAllParticles(Core.UIParticleSystem system)
        {
            if (system.ActiveParticles == null) return;

            for (int i = system.ActiveParticles.Count - 1; i >= 0; i--)
            {
                var particle = system.ActiveParticles[i];
                if (particle == null) continue;

                system.TriggerParticleDied(particle, i);
                particle.trail?.Clear();
                system.Pool?.Return(particle);
            }

            system.ActiveParticles.Clear();
        }

#if UNITY_EDITOR
        public static void DrawEmissionGizmos(Core.UIParticleSystem system)
        {
            Gizmos.matrix = system.Transform.localToWorldMatrix;
            Gizmos.color = new Color(0, 1, 0, 0.3f);

            switch (system.EmissionSettings.emissionShape)
            {
                case EmissionShapeType.Rectangle:
                    if (system.EmissionSettings.emitFromEdge)
                    {
                        Vector3 topLeft = new Vector3(-system.EmissionSettings.emissionAreaSize.x / 2, system.EmissionSettings.emissionAreaSize.y / 2, 0);
                        Vector3 topRight = new Vector3(system.EmissionSettings.emissionAreaSize.x / 2, system.EmissionSettings.emissionAreaSize.y / 2, 0);
                        Vector3 bottomLeft = new Vector3(-system.EmissionSettings.emissionAreaSize.x / 2, -system.EmissionSettings.emissionAreaSize.y / 2, 0);
                        Vector3 bottomRight = new Vector3(system.EmissionSettings.emissionAreaSize.x / 2, -system.EmissionSettings.emissionAreaSize.y / 2, 0);

                        Gizmos.DrawLine(topLeft, topRight);
                        Gizmos.DrawLine(topRight, bottomRight);
                        Gizmos.DrawLine(bottomRight, bottomLeft);
                        Gizmos.DrawLine(bottomLeft, topLeft);
                    }
                    else
                    {
                        Gizmos.DrawCube(Vector3.zero, new Vector3(system.EmissionSettings.emissionAreaSize.x, system.EmissionSettings.emissionAreaSize.y, 0));
                    }
                    break;

                case EmissionShapeType.Circle:
                    if (system.EmissionSettings.emitFromEdge)
                    {
                        DrawCircleGizmo(Vector3.zero, system.EmissionSettings.circleRadius, 32);
                    }
                    else
                    {
                        Gizmos.DrawSphere(Vector3.zero, system.EmissionSettings.circleRadius);
                    }
                    break;

                case EmissionShapeType.Line:
                    Vector3 lineStart = new Vector3(-system.EmissionSettings.lineLength / 2, 0, 0);
                    Vector3 lineEnd = new Vector3(system.EmissionSettings.lineLength / 2, 0, 0);
                    Gizmos.DrawLine(lineStart, lineEnd);

                    Gizmos.color = new Color(1, 1, 0, 0.8f);
                    Gizmos.DrawSphere(lineStart, 2f);
                    Gizmos.DrawSphere(lineEnd, 2f);
                    break;

                case EmissionShapeType.Cone:
                    DrawConeGizmo(system, Vector3.zero, system.EmissionSettings.coneAngle, system.EmissionSettings.coneRange);
                    break;

                case EmissionShapeType.Point:
                    Gizmos.color = new Color(1, 0, 0, 0.8f);
                    Gizmos.DrawSphere(Vector3.zero, 5f);
                    break;

                case EmissionShapeType.Edge:
                    Vector3 topLeft2 = new Vector3(-system.EmissionSettings.emissionAreaSize.x / 2, system.EmissionSettings.emissionAreaSize.y / 2, 0);
                    Vector3 topRight2 = new Vector3(system.EmissionSettings.emissionAreaSize.x / 2, system.EmissionSettings.emissionAreaSize.y / 2, 0);
                    Vector3 bottomLeft2 = new Vector3(-system.EmissionSettings.emissionAreaSize.x / 2, -system.EmissionSettings.emissionAreaSize.y / 2, 0);
                    Vector3 bottomRight2 = new Vector3(system.EmissionSettings.emissionAreaSize.x / 2, -system.EmissionSettings.emissionAreaSize.y / 2, 0);

                    Gizmos.color = new Color(1, 0.5f, 0, 0.8f);
                    Gizmos.DrawLine(topLeft2, topRight2);
                    Gizmos.DrawLine(topRight2, bottomRight2);
                    Gizmos.DrawLine(bottomRight2, bottomLeft2);
                    Gizmos.DrawLine(bottomLeft2, topLeft2);
                    break;

                case EmissionShapeType.Sprite:
                    DrawSpriteShapeGizmo(system.EmissionSettings);
                    break;

                case EmissionShapeType.Image:
                    DrawImageShapeGizmo(system);
                    break;

                case EmissionShapeType.Text:
                    DrawTextShapeGizmo(system);
                    break;
            }

            if (system.EmissionSettings.emissionShape != EmissionShapeType.Point &&
                system.EmissionSettings.emissionShape != EmissionShapeType.Line &&
                system.EmissionSettings.emissionShape != EmissionShapeType.Cone &&
                !system.EmissionSettings.randomizeDirection)
            {
                Gizmos.color = new Color(0, 0, 1, 0.8f);
                Vector3 directionVector = new Vector3(system.EmissionSettings.emissionDirection.x, system.EmissionSettings.emissionDirection.y, 0).normalized * 30f;
                Gizmos.DrawRay(Vector3.zero, directionVector);

                Vector3 arrowHead1 = Quaternion.Euler(0, 0, 30) * (-directionVector.normalized * 10f);
                Vector3 arrowHead2 = Quaternion.Euler(0, 0, -30) * (-directionVector.normalized * 10f);
                Vector3 arrowTip = directionVector;

                Gizmos.DrawLine(arrowTip, arrowTip + arrowHead1);
                Gizmos.DrawLine(arrowTip, arrowTip + arrowHead2);
            }
        }

        private static void DrawSpriteShapeGizmo(EmissionSettings settings)
        {
            if (settings.shapeSprite == null)
            {
                Gizmos.color = new Color(1, 0, 0, 0.8f);
                Gizmos.DrawSphere(Vector3.zero, 5f);
                return;
            }

            Vector2 size = settings.shapeSpriteSize;
            Rect spriteRect = settings.shapeSprite.rect;
            Vector2 pivotN = spriteRect.width > 0f && spriteRect.height > 0f
                ? new Vector2(settings.shapeSprite.pivot.x / spriteRect.width, settings.shapeSprite.pivot.y / spriteRect.height)
                : new Vector2(0.5f, 0.5f);

            Vector3 bottomLeft = new Vector3(-pivotN.x * size.x, -pivotN.y * size.y, 0);
            Vector3 bottomRight = new Vector3((1f - pivotN.x) * size.x, -pivotN.y * size.y, 0);
            Vector3 topLeft = new Vector3(-pivotN.x * size.x, (1f - pivotN.y) * size.y, 0);
            Vector3 topRight = new Vector3((1f - pivotN.x) * size.x, (1f - pivotN.y) * size.y, 0);

            Gizmos.color = new Color(0, 1, 0, 0.8f);
            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);
        }

        private static void DrawImageShapeGizmo(Core.UIParticleSystem system)
        {
            Image image = system.EmissionSettings.shapeImage;
            if (image == null || image.rectTransform == null)
            {
                Gizmos.color = new Color(1, 0, 0, 0.8f);
                Gizmos.DrawSphere(Vector3.zero, 5f);
                return;
            }

            Matrix4x4 previous = Gizmos.matrix;
            Gizmos.matrix = image.rectTransform.localToWorldMatrix;
            Rect drawRect = GetImageDrawingRect(image);

            Vector3 bottomLeft = new Vector3(drawRect.xMin, drawRect.yMin, 0f);
            Vector3 bottomRight = new Vector3(drawRect.xMax, drawRect.yMin, 0f);
            Vector3 topLeft = new Vector3(drawRect.xMin, drawRect.yMax, 0f);
            Vector3 topRight = new Vector3(drawRect.xMax, drawRect.yMax, 0f);

            Gizmos.color = image.sprite != null ? new Color(0, 1, 0, 0.8f) : new Color(1, 0.5f, 0, 0.8f);
            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);
            Gizmos.matrix = previous;
        }

        private static void DrawTextShapeGizmo(Core.UIParticleSystem system)
        {
            Graphic graphic = system.EmissionSettings.shapeText;
            if (graphic == null || graphic.rectTransform == null)
            {
                Gizmos.color = new Color(1, 0, 0, 0.8f);
                Gizmos.DrawSphere(Vector3.zero, 5f);
                return;
            }

            Matrix4x4 previous = Gizmos.matrix;
            Gizmos.matrix = graphic.rectTransform.localToWorldMatrix;
            Rect rect = graphic.rectTransform.rect;

            Vector3 bottomLeft = new Vector3(rect.xMin, rect.yMin, 0f);
            Vector3 bottomRight = new Vector3(rect.xMax, rect.yMin, 0f);
            Vector3 topLeft = new Vector3(rect.xMin, rect.yMax, 0f);
            Vector3 topRight = new Vector3(rect.xMax, rect.yMax, 0f);

            Gizmos.color = TextShapeSampler.IsSupported(graphic)
                ? new Color(0, 1, 0, 0.8f)
                : new Color(1, 0.5f, 0, 0.8f);
            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);
            Gizmos.matrix = previous;
        }

        private static void DrawCircleGizmo(Vector3 center, float radius, int segments)
        {
            float angle = 0f;
            float angleStep = 360f / segments;
            Vector3 lastPoint = center + new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle) * radius, Mathf.Sin(Mathf.Deg2Rad * angle) * radius, 0);

            for (int i = 1; i <= segments; i++)
            {
                angle += angleStep;
                Vector3 nextPoint = center + new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle) * radius, Mathf.Sin(Mathf.Deg2Rad * angle) * radius, 0);
                Gizmos.DrawLine(lastPoint, nextPoint);
                lastPoint = nextPoint;
            }
        }

        private static void DrawConeGizmo(Core.UIParticleSystem system, Vector3 origin, float angle, float range)
        {
            Vector3 direction = new Vector3(system.EmissionSettings.emissionDirection.x, system.EmissionSettings.emissionDirection.y, 0).normalized;
            Vector3 leftDir = Quaternion.Euler(0, 0, angle * 0.5f) * direction;
            Vector3 rightDir = Quaternion.Euler(0, 0, -angle * 0.5f) * direction;

            Gizmos.DrawLine(origin, origin + leftDir * range);
            Gizmos.DrawLine(origin, origin + rightDir * range);

            int segments = 20;
            float angleStep = angle / segments;
            Vector3 lastPoint = origin + leftDir * range;

            for (int i = 1; i <= segments; i++)
            {
                float currentAngle = angle * 0.5f - i * angleStep;
                Vector3 nextDir = Quaternion.Euler(0, 0, currentAngle) * direction;
                Vector3 nextPoint = origin + nextDir * range;
                Gizmos.DrawLine(lastPoint, nextPoint);
                lastPoint = nextPoint;
            }
        }

        public static void DrawCollisionGizmos(Core.UIParticleSystem system)
        {
            CollisionSettings settings = system.CollisionSettings;
            if (settings == null || !settings.useCollision)
                return;

            Matrix4x4 previous = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.identity;

            if (settings.useBounds)
            {
                RectTransform bounds = settings.boundsTarget != null
                    ? settings.boundsTarget
                    : system.GetComponent<RectTransform>();
                DrawRectTransformGizmo(bounds, new Color(0f, 1f, 0f, 0.85f));
            }

            if (settings.obstacles != null)
            {
                for (int i = 0; i < settings.obstacles.Count; i++)
                    DrawRectTransformGizmo(settings.obstacles[i], new Color(1f, 0.5f, 0f, 0.85f));
            }

            Gizmos.matrix = previous;
        }

        private static void DrawRectTransformGizmo(RectTransform rectTransform, Color color)
        {
            if (rectTransform == null)
                return;

            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            Gizmos.color = color;
            Gizmos.DrawLine(corners[0], corners[1]);
            Gizmos.DrawLine(corners[1], corners[2]);
            Gizmos.DrawLine(corners[2], corners[3]);
            Gizmos.DrawLine(corners[3], corners[0]);
        }
#endif
    }
}