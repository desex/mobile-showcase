using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace TierStudios.UIParticleSystem.Core
{

    public struct ParticleUpdateJob : IJobParallelFor
    {
        public float deltaTime;

        public byte useGravity;
        public Vector2 gravity;

        public byte useNoise;
        public byte separateAxisNoise;
        public byte animateNoise;
        public float noiseStrength;
        public float noiseStrengthX;
        public float noiseStrengthY;
        public float noiseFrequency;
        public float noiseFrequencyX;
        public float noiseFrequencyY;
        public float noiseSpeed;
        public float noiseOffsetX;
        public float noiseOffsetY;
        public int noiseType;
        public int noiseTurbulenceOctaves;

        public byte useRotationOverLifetime;
        public byte alignToDirection;
        public byte useRotationBySpeed;
        public float rotationBySpeed;
        public float rotationBySpeedMin;
        public float rotationBySpeedMax;

        public byte useStretchedBillboard;
        public float lengthScale;
        public float speedStretch;

        public byte useSizeOverLifetime;
        public byte use2DSize;
        public byte use2DSizeOverLifetime;
        public float size2DMultiplierX;
        public float size2DMultiplierY;
        [ReadOnly] public NativeArray<float> sizeOverLifeSamples;
        [ReadOnly] public NativeArray<float> sizeXOverLifeSamples;
        [ReadOnly] public NativeArray<float> sizeYOverLifeSamples;

        public byte useSizeBySpeed;
        public byte sizeSpeedSeparateAxes;
        public float sizeSpeedMin;
        public float sizeSpeedMax;
        public float sizeSpeedMultiplier;
        public float sizeSpeed2DMultiplierX;
        public float sizeSpeed2DMultiplierY;
        [ReadOnly] public NativeArray<float> sizeBySpeedSamples;
        [ReadOnly] public NativeArray<float> sizeXBySpeedSamples;
        [ReadOnly] public NativeArray<float> sizeYBySpeedSamples;

        public byte useGradientColor;
        public byte fadeOut;
        public byte attractorFadeOnApproach;
        [ReadOnly] public NativeArray<Color> gradient1Samples;
        [ReadOnly] public NativeArray<Color> gradient2Samples;

        public byte useColorBySpeed;
        public float colorSpeedMin;
        public float colorSpeedMax;
        [ReadOnly] public NativeArray<Color> colorBySpeedGradient1Samples;
        [ReadOnly] public NativeArray<Color> colorBySpeedGradient2Samples;

        public byte useVelocityOverLifetime;
        public byte useLinearVelocity;
        public byte useLinearVelocityCurves;
        public float linearVelocityX;
        public float linearVelocityY;
        public byte useRandomLinearVelocity;
        public byte useOrbitalVelocity;
        public byte useOrbitalVelocityCurve;
        public float orbitalVelocity;
        public float orbitalCenterX;
        public float orbitalCenterY;
        public byte useRandomOrbitalVelocity;
        public byte velocityUseLocalSpace;
        public Matrix4x4 systemTransformMatrix;
        [ReadOnly] public NativeArray<float> velLinearXSamples;
        [ReadOnly] public NativeArray<float> velLinearYSamples;
        [ReadOnly] public NativeArray<float> velOrbitalSamples;
        [ReadOnly] public NativeArray<float> velLinearXMinSamples;
        [ReadOnly] public NativeArray<float> velLinearYMinSamples;
        [ReadOnly] public NativeArray<float> velOrbitalMinSamples;

        public byte useLimitVelocityOverLifetime;
        public byte limitSeparateAxes;
        public int limitMode;
        public float limitConstant;
        public float limitMin;
        public int limitXMode;
        public float limitXConstant;
        public float limitXMin;
        public int limitYMode;
        public float limitYConstant;
        public float limitYMin;
        public int limitZMode;
        public float limitZConstant;
        public float limitZMin;
        public float limitDampen;
        [ReadOnly] public NativeArray<float> limitSamples;
        [ReadOnly] public NativeArray<float> limitMinSamples;
        [ReadOnly] public NativeArray<float> limitXSamples;
        [ReadOnly] public NativeArray<float> limitXMinSamples;
        [ReadOnly] public NativeArray<float> limitYSamples;
        [ReadOnly] public NativeArray<float> limitYMinSamples;
        [ReadOnly] public NativeArray<float> limitZSamples;
        [ReadOnly] public NativeArray<float> limitZMinSamples;
        public int dragMode;
        public float dragConstant;
        public float dragMin;
        public byte dragMultiplyBySize;
        public byte dragMultiplyByVelocity;
        [ReadOnly] public NativeArray<float> dragSamples;
        [ReadOnly] public NativeArray<float> dragMinSamples;

        public byte useAnimation;
        public float animationSpeed;
        public byte loopAnimation;
        public int animationTimeMode;

        public byte useForceOverLifetime;
        public int  forceXMode;
        public float forceXConstant;
        public float forceXMin;
        public int  forceYMode;
        public float forceYConstant;
        public float forceYMin;
        public byte forceUseLocalSpace;
        [ReadOnly] public NativeArray<float> forceXSamples;
        [ReadOnly] public NativeArray<float> forceXMinSamples;
        [ReadOnly] public NativeArray<float> forceYSamples;
        [ReadOnly] public NativeArray<float> forceYMinSamples;

        public NativeArray<ParticleData> particles;

        public void Execute(int i)
        {
            ParticleData p = particles[i];
            if (p.isAlive == 0) return;

            float t = p.maxLifetime > 0f ? Mathf.Clamp01(p.age / p.maxLifetime) : 0f;

            if (useGravity == 1)
            {
                p.velocity.x += gravity.x * deltaTime;
                p.velocity.y += gravity.y * deltaTime;
            }

            if (useNoise == 1)
                ApplyNoise(ref p);

            if (useVelocityOverLifetime == 1)
                ApplyVelocityOverLifetime(ref p, t);

            if (useForceOverLifetime == 1)
                ApplyForceOverLifetime(ref p, t);

            if (useLimitVelocityOverLifetime == 1)
                ApplyLimitVelocityOverLifetime(ref p, t);

            p.position.x += p.velocity.x * deltaTime;
            p.position.y += p.velocity.y * deltaTime;
            p.position.z += p.velocity.z * deltaTime;

            float vx = p.velocity.x, vy = p.velocity.y;
            float spd = Mathf.Sqrt(vx * vx + vy * vy);

            if (useStretchedBillboard == 1)
            {
                if (spd > 0.001f)
                    p.stretchedRotation = Mathf.Atan2(vy, vx) * Mathf.Rad2Deg - 90f;
            }
            else
            {
                if (useRotationOverLifetime == 1)
                    p.rotation += p.angularVelocity * deltaTime;

                if (useRotationBySpeed == 1)
                {
                    float speedFactor = (rotationBySpeedMax > rotationBySpeedMin)
                        ? Mathf.Clamp01((spd - rotationBySpeedMin) / (rotationBySpeedMax - rotationBySpeedMin))
                        : 0f;
                    p.rotation += rotationBySpeed * speedFactor * deltaTime;
                }
            }

            ComputeSize(ref p, t);

            ComputeColor(ref p, t);

            if (useAnimation == 1)
                UpdateAnimation(ref p, t);

            ComputePoints(ref p);

            particles[i] = p;
        }

        private void ApplyForceOverLifetime(ref ParticleData p, float t)
        {
            float fx = 0f;
            switch (forceXMode)
            {
                case 0: fx = forceXConstant; break;
                case 1: fx = EvalSamples(forceXSamples, t) * forceXConstant; break;
                case 2: fx = forceXMin + (forceXConstant - forceXMin) * p.forceRandomLerpX; break;
                case 3:
                    float fxMin = EvalSamples(forceXMinSamples, t);
                    float fxMax = EvalSamples(forceXSamples, t);
                    fx = fxMin + (fxMax - fxMin) * p.forceRandomLerpX;
                    break;
            }

            float fy = 0f;
            switch (forceYMode)
            {
                case 0: fy = forceYConstant; break;
                case 1: fy = EvalSamples(forceYSamples, t) * forceYConstant; break;
                case 2: fy = forceYMin + (forceYConstant - forceYMin) * p.forceRandomLerpY; break;
                case 3:
                    float fyMin = EvalSamples(forceYMinSamples, t);
                    float fyMax = EvalSamples(forceYSamples, t);
                    fy = fyMin + (fyMax - fyMin) * p.forceRandomLerpY;
                    break;
            }

            if (forceUseLocalSpace == 1)
            {
                Vector3 localForce = new Vector3(fx, fy, 0f);
                Vector3 worldForce = systemTransformMatrix.MultiplyVector(localForce);
                fx = worldForce.x;
                fy = worldForce.y;
            }

            p.velocity.x += fx * deltaTime;
            p.velocity.y += fy * deltaTime;
        }

        private void ApplyNoise(ref ParticleData p)
        {
            float nt = animateNoise == 1 ? p.noiseTime * noiseSpeed : 0f;

            if (separateAxisNoise == 1)
            {
                float nx = GetNoise(
                    (p.position.x + noiseOffsetX + p.noiseSeed) * noiseFrequencyX * 0.01f,
                    (p.position.y + noiseOffsetY) * noiseFrequencyX * 0.01f, nt);
                float ny = GetNoise(
                    (p.position.x + noiseOffsetX) * noiseFrequencyY * 0.01f,
                    (p.position.y + noiseOffsetY + p.noiseSeed + 1000f) * noiseFrequencyY * 0.01f, nt);
                p.velocity.x += nx * noiseStrengthX * deltaTime;
                p.velocity.y += ny * noiseStrengthY * deltaTime;
            }
            else
            {
                float nx = GetNoise(
                    (p.position.x + noiseOffsetX + p.noiseSeed) * noiseFrequency * 0.01f,
                    (p.position.y + noiseOffsetY) * noiseFrequency * 0.01f, nt);
                float ny = GetNoise(
                    (p.position.x + noiseOffsetX + p.noiseSeed + 1000f) * noiseFrequency * 0.01f,
                    (p.position.y + noiseOffsetY + 1000f) * noiseFrequency * 0.01f, nt);
                p.velocity.x += nx * noiseStrength * deltaTime;
                p.velocity.y += ny * noiseStrength * deltaTime;
            }

            p.noiseTime += deltaTime;
        }

        private float GetNoise(float x, float y, float time)
        {
            switch (noiseType)
            {
                case 1:
                    float turb = 0f, amp = 1f, freq = 1f;
                    for (int i = 0; i < noiseTurbulenceOctaves; i++)
                    {
                        turb += Mathf.PerlinNoise(x * freq + time, y * freq + time) * amp;
                        amp *= 0.5f; freq *= 2f;
                    }
                    return Mathf.Clamp(turb - 0.5f, -1f, 1f);

                case 2:
                    float r = Mathf.PerlinNoise(x + time, y + time);
                    return (1f - Mathf.Abs(r * 2f - 1f)) * 2f - 1f;

                case 3:
                    float b = Mathf.PerlinNoise(x + time, y + time);
                    return Mathf.Abs(b * 2f - 1f) * 2f - 1f;

                default:
                    return Mathf.PerlinNoise(x + time, y + time) * 2f - 1f;
            }
        }

        private void ApplyVelocityOverLifetime(ref ParticleData p, float t)
        {
            if (useLinearVelocity == 1)
            {
                float lx, ly;

                if (useLinearVelocityCurves == 1)
                {
                    if (useRandomLinearVelocity == 1)
                    {
                        float minX = EvalSamples(velLinearXMinSamples, t);
                        float maxX = EvalSamples(velLinearXSamples, t);
                        lx = minX + (maxX - minX) * p.velocityRandomLinearOffset.x;

                        float minY = EvalSamples(velLinearYMinSamples, t);
                        float maxY = EvalSamples(velLinearYSamples, t);
                        ly = minY + (maxY - minY) * p.velocityRandomLinearOffset.y;
                    }
                    else
                    {
                        lx = EvalSamples(velLinearXSamples, t);
                        ly = EvalSamples(velLinearYSamples, t);
                    }
                }
                else if (useRandomLinearVelocity == 1)
                {
                    lx = p.velocityRandomLinearOffset.x * t;
                    ly = p.velocityRandomLinearOffset.y * t;
                }
                else
                {
                    lx = linearVelocityX * t;
                    ly = linearVelocityY * t;
                }

                lx *= 100f;
                ly *= 100f;

                if (velocityUseLocalSpace == 0)
                {
                    Vector3 world = systemTransformMatrix.MultiplyVector(new Vector3(lx, ly, 0f));
                    p.velocity = p.originalVelocity + world;
                }
                else
                {
                    p.velocity = p.originalVelocity + new Vector3(lx, ly, 0f);
                }
            }

            if (useOrbitalVelocity == 1)
            {
                float spd;
                if (useOrbitalVelocityCurve == 1)
                {
                    if (useRandomOrbitalVelocity == 1)
                    {
                        float minVal = EvalSamples(velOrbitalMinSamples, t);
                        float maxVal = EvalSamples(velOrbitalSamples, t);
                        spd = minVal + (maxVal - minVal) * p.velocityRandomOrbitalOffset;
                    }
                    else
                    {
                        spd = EvalSamples(velOrbitalSamples, t) * orbitalVelocity;
                    }
                }
                else
                {
                    spd = useRandomOrbitalVelocity == 1 ? p.velocityRandomOrbitalOffset : orbitalVelocity;
                }

                spd *= 100f;

                float rx = p.position.x - orbitalCenterX;
                float ry = p.position.y - orbitalCenterY;
                float dist = Mathf.Sqrt(rx * rx + ry * ry);

                if (dist > 0.001f)
                {
                    float inv = 1f / dist;
                    p.position.x += (-ry * inv) * spd * deltaTime;
                    p.position.y += (rx * inv) * spd * deltaTime;
                }
            }
        }

        private void ApplyLimitVelocityOverLifetime(ref ParticleData p, float t)
        {
            float dragVal = 0f;
            if (dragMode == 0)      dragVal = dragConstant;
            else if (dragMode == 1) dragVal = EvalSamples(dragSamples, t) * dragConstant;
            else if (dragMode == 2) dragVal = dragMin + (dragConstant - dragMin) * p.dragRandomLerp;
            else if (dragMode == 3)
            {
                float dMin = EvalSamples(dragMinSamples, t);
                float dMax = EvalSamples(dragSamples, t);
                dragVal = dMin + (dMax - dMin) * p.dragRandomLerp;
            }

            if (dragVal > 0f)
            {
                float scaledDrag = dragVal;

                if (dragMultiplyBySize == 1)
                    scaledDrag *= Mathf.Clamp01(p.size / 100f);

                if (dragMultiplyByVelocity == 1)
                    scaledDrag *= Mathf.Clamp01(p.velocity.magnitude / 200f);

                float dragFactor = Mathf.Max(0f, 1f - scaledDrag * deltaTime);
                p.velocity *= dragFactor;
            }

            if (limitSeparateAxes == 0)
            {
                float limitVal = 0f;
                if (limitMode == 0)      limitVal = limitConstant;
                else if (limitMode == 1) limitVal = EvalSamples(limitSamples, t) * limitConstant;
                else if (limitMode == 2) limitVal = limitMin + (limitConstant - limitMin) * p.limitVelocityRandomLerp;
                else if (limitMode == 3)
                {
                    float lMin = EvalSamples(limitMinSamples, t);
                    float lMax = EvalSamples(limitSamples, t);
                    limitVal = lMin + (lMax - lMin) * p.limitVelocityRandomLerp;
                }

                limitVal = Mathf.Max(0f, limitVal);

                float dampenClamped = Mathf.Clamp01(limitDampen);
                if (dampenClamped <= 0f) return;

                float speed = p.velocity.magnitude;
                if (speed > limitVal && speed > 0.001f)
                {
                    float targetSpeed = Mathf.Lerp(speed, limitVal, dampenClamped);
                    p.velocity = (p.velocity / speed) * targetSpeed;
                }
            }
            else
            {
                float dampenClamped = Mathf.Clamp01(limitDampen);

                float limitX = 0f;
                if (limitXMode == 0)      limitX = limitXConstant;
                else if (limitXMode == 1) limitX = EvalSamples(limitXSamples, t) * limitXConstant;
                else if (limitXMode == 2) limitX = limitXMin + (limitXConstant - limitXMin) * p.limitVelocityRandomAxesLerp.x;
                else if (limitXMode == 3)
                {
                    float xMin = EvalSamples(limitXMinSamples, t);
                    float xMax = EvalSamples(limitXSamples, t);
                    limitX = xMin + (xMax - xMin) * p.limitVelocityRandomAxesLerp.x;
                }
                limitX = Mathf.Max(0f, limitX);

                if (dampenClamped > 0f)
                {
                    float absVx = Mathf.Abs(p.velocity.x);
                    if (absVx > limitX)
                        p.velocity.x = Mathf.Sign(p.velocity.x) * Mathf.Lerp(absVx, limitX, dampenClamped);
                }

                float limitY = 0f;
                if (limitYMode == 0)      limitY = limitYConstant;
                else if (limitYMode == 1) limitY = EvalSamples(limitYSamples, t) * limitYConstant;
                else if (limitYMode == 2) limitY = limitYMin + (limitYConstant - limitYMin) * p.limitVelocityRandomAxesLerp.y;
                else if (limitYMode == 3)
                {
                    float yMin = EvalSamples(limitYMinSamples, t);
                    float yMax = EvalSamples(limitYSamples, t);
                    limitY = yMin + (yMax - yMin) * p.limitVelocityRandomAxesLerp.y;
                }
                limitY = Mathf.Max(0f, limitY);

                if (dampenClamped > 0f)
                {
                    float absVy = Mathf.Abs(p.velocity.y);
                    if (absVy > limitY)
                        p.velocity.y = Mathf.Sign(p.velocity.y) * Mathf.Lerp(absVy, limitY, dampenClamped);
                }

                float limitZ = 0f;
                if (limitZMode == 0)      limitZ = limitZConstant;
                else if (limitZMode == 1) limitZ = EvalSamples(limitZSamples, t) * limitZConstant;
                else if (limitZMode == 2) limitZ = limitZMin + (limitZConstant - limitZMin) * p.limitVelocityRandomAxesLerp.z;
                else if (limitZMode == 3)
                {
                    float zMin = EvalSamples(limitZMinSamples, t);
                    float zMax = EvalSamples(limitZSamples, t);
                    limitZ = zMin + (zMax - zMin) * p.limitVelocityRandomAxesLerp.z;
                }
                limitZ = Mathf.Max(0f, limitZ);

                if (dampenClamped > 0f)
                {
                    float absVz = Mathf.Abs(p.velocity.z);
                    if (absVz > limitZ)
                        p.velocity.z = Mathf.Sign(p.velocity.z) * Mathf.Lerp(absVz, limitZ, dampenClamped);
                }
            }
        }

        private void ComputeSize(ref ParticleData p, float t)
        {
            Vector2 baseSize;

            if (useSizeOverLifetime == 1)
            {
                if (use2DSize == 1)
                {
                    if (use2DSizeOverLifetime == 1)
                    {
                        baseSize = new Vector2(
                            p.initialSize2D.x * EvalSamples(sizeXOverLifeSamples, t) * size2DMultiplierX,
                            p.initialSize2D.y * EvalSamples(sizeYOverLifeSamples, t) * size2DMultiplierY);
                    }
                    else
                    {
                        float m = EvalSamples(sizeOverLifeSamples, t);
                        baseSize = p.initialSize2D * m;
                    }
                }
                else
                {
                    if (use2DSizeOverLifetime == 1)
                    {
                        baseSize = new Vector2(
                            p.initialSize * EvalSamples(sizeXOverLifeSamples, t) * size2DMultiplierX,
                            p.initialSize * EvalSamples(sizeYOverLifeSamples, t) * size2DMultiplierY);
                    }
                    else
                    {
                        float s = p.initialSize * EvalSamples(sizeOverLifeSamples, t);
                        baseSize = new Vector2(s, s);
                    }
                }
            }
            else
            {
                baseSize = use2DSize == 1 ? p.initialSize2D : new Vector2(p.initialSize, p.initialSize);
            }

            if (useSizeBySpeed == 1)
            {
                float vx = p.velocity.x, vy = p.velocity.y, vz = p.velocity.z;
                float speed = Mathf.Sqrt(vx * vx + vy * vy + vz * vz);
                float speedFactor = (sizeSpeedMax > sizeSpeedMin) ? Mathf.Clamp01((speed - sizeSpeedMin) / (sizeSpeedMax - sizeSpeedMin)) : 0f;

                if (sizeSpeedSeparateAxes == 1)
                {
                    baseSize.x *= EvalSamples(sizeXBySpeedSamples, speedFactor) * sizeSpeed2DMultiplierX;
                    baseSize.y *= EvalSamples(sizeYBySpeedSamples, speedFactor) * sizeSpeed2DMultiplierY;
                }
                else
                {
                    baseSize *= EvalSamples(sizeBySpeedSamples, speedFactor) * sizeSpeedMultiplier;
                }
            }

            if (useStretchedBillboard == 1)
            {
                float vx = p.velocity.x, vy = p.velocity.y;
                float spd = Mathf.Sqrt(vx * vx + vy * vy);
                float bu = Mathf.Min(baseSize.x, baseSize.y);
                float ns = bu > 0.001f ? spd / bu : 0f;
                float h = Mathf.Max(bu * lengthScale, bu * lengthScale + bu * ns * speedStretch);
                p.stretchedWidth = bu;
                p.stretchedHeight = h;
                p.size2D = new Vector2(bu, h);
            }
            else
            {
                p.size2D = baseSize;
            }
        }

        private void ComputeColor(ref ParticleData p, float t)
        {
            Color c = p.initialColor;

            if (useGradientColor == 1)
            {
                Color g = p.useSecondGradient == 1
                    ? EvalGradient(gradient2Samples, t)
                    : EvalGradient(gradient1Samples, t);
                c = new Color(c.r * g.r, c.g * g.g, c.b * g.b, c.a * g.a);
            }

            if (useColorBySpeed == 1)
            {
                float vx = p.velocity.x, vy = p.velocity.y, vz = p.velocity.z;
                float speed = Mathf.Sqrt(vx * vx + vy * vy + vz * vz);
                float speedFactor = (colorSpeedMax > colorSpeedMin) ? Mathf.Clamp01((speed - colorSpeedMin) / (colorSpeedMax - colorSpeedMin)) : 0f;

                Color g = p.useSecondGradient == 1
                    ? EvalGradient(colorBySpeedGradient2Samples, speedFactor)
                    : EvalGradient(colorBySpeedGradient1Samples, speedFactor);
                c = new Color(c.r * g.r, c.g * g.g, c.b * g.b, c.a * g.a);
            }

            if (fadeOut == 1)
                c.a *= 1f - t;

            if (attractorFadeOnApproach == 1)
                c.a *= p.attractorFadeAlpha;

            p.color = c;
        }

        private void UpdateAnimation(ref ParticleData p, float t)
        {
            if (p.hasAnimation == 0 || animationSpeed <= 0f) return;

            int frameCount = p.endFrame - p.startFrame + 1;
            if (frameCount <= 1) return;

            int targetFrame;

            if (animationTimeMode == 1)
            {
                if (loopAnimation == 1)
                {
                    float total = t * animationSpeed;
                    float cycle = total - Mathf.Floor(total);
                    targetFrame = p.startFrame + Mathf.Clamp(Mathf.FloorToInt(cycle * frameCount), 0, frameCount - 1);
                }
                else
                {
                    targetFrame = p.startFrame + Mathf.Clamp(Mathf.FloorToInt(t * frameCount), 0, frameCount - 1);
                }
            }
            else 
            {
                p.animationTime += deltaTime;
                float progress = p.animationTime * animationSpeed;

                targetFrame = loopAnimation == 1
                    ? p.startFrame + (Mathf.FloorToInt(progress) % frameCount)
                    : p.startFrame + Mathf.Min(Mathf.FloorToInt(progress), frameCount - 1);
            }

            p.currentFrame = targetFrame;
        }

        private void ComputePoints(ref ParticleData p)
        {
            float hw = p.size2D.x * 0.5f;
            float hh = p.size2D.y * 0.5f;

            if (float.IsNaN(hw) || float.IsInfinity(hw)) hw = 0f;
            if (float.IsNaN(hh) || float.IsInfinity(hh)) hh = 0f;

            float tlx = -hw, tly = hh;
            float trx = hw, try_ = hh;
            float brx = hw, bry = -hh;
            float blx = -hw, bly = -hh;

            float rot;
            if (useStretchedBillboard == 1)
            {
                rot = p.stretchedRotation;
            }
            else if (alignToDirection == 1)
            {
                float avx = p.velocity.x, avy = p.velocity.y;
                float faceSpeed = Mathf.Sqrt(avx * avx + avy * avy);
                float face = faceSpeed > 0.001f
                    ? Mathf.Atan2(avy, avx) * Mathf.Rad2Deg - 90f
                    : 0f;
                rot = face + p.rotation;
            }
            else
            {
                rot = p.rotation;
            }
            if (float.IsNaN(rot) || float.IsInfinity(rot)) rot = 0f;

            if (rot != 0f)
            {
                float rad = rot * Mathf.Deg2Rad;
                float cos = Mathf.Cos(rad), sin = Mathf.Sin(rad);

                float ax = tlx * cos - tly * sin, ay = tlx * sin + tly * cos; tlx = ax; tly = ay;
                float bx = trx * cos - try_ * sin, by = trx * sin + try_ * cos; trx = bx; try_ = by;
                float cx = brx * cos - bry * sin, cy = brx * sin + bry * cos; brx = cx; bry = cy;
                float dx = blx * cos - bly * sin, dy = blx * sin + bly * cos; blx = dx; bly = dy;
            }

            float px = p.position.x, py = p.position.y;
            if (float.IsNaN(px) || float.IsInfinity(px)) px = 0f;
            if (float.IsNaN(py) || float.IsInfinity(py)) py = 0f;

            p.point0 = new Vector3(px + tlx, py + tly, 0f);
            p.point1 = new Vector3(px + trx, py + try_, 0f);
            p.point2 = new Vector3(px + brx, py + bry, 0f);
            p.point3 = new Vector3(px + blx, py + bly, 0f);
        }

        private static float EvalSamples(NativeArray<float> s, float t)
        {
            if (s.Length == 0) return 1f;
            float idx = Mathf.Clamp01(t) * (s.Length - 1);
            int lo = (int)idx;
            int hi = lo < s.Length - 1 ? lo + 1 : lo;
            float frac = idx - lo;
            return s[lo] + (s[hi] - s[lo]) * frac;
        }

        private static Color EvalGradient(NativeArray<Color> s, float t)
        {
            if (s.Length == 0) return Color.white;
            float idx = Mathf.Clamp01(t) * (s.Length - 1);
            int lo = (int)idx;
            int hi = lo < s.Length - 1 ? lo + 1 : lo;
            float frac = idx - lo;
            Color a = s[lo], b = s[hi];
            return new Color(
                a.r + (b.r - a.r) * frac,
                a.g + (b.g - a.g) * frac,
                a.b + (b.b - a.b) * frac,
                a.a + (b.a - a.a) * frac);
        }
    }
}