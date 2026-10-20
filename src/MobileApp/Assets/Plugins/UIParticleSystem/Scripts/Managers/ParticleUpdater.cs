using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using TierStudios.UIParticleSystem.Core;
using TierStudios.UIParticleSystem.Enums;
using TierStudios.UIParticleSystem.Settings;

namespace TierStudios.UIParticleSystem.Managers
{
    public class ParticleUpdater : System.IDisposable
    {
        private readonly Core.UIParticleSystem _system;
        private readonly AttractorManager _attractorManager;
        private readonly CollisionManager _collisionManager;
        private readonly TrailManager _trailManager;
        private readonly TextureSheetManager _textureSheetManager;

        private NativeArray<ParticleData> _dataBuffer;
        private int _dataBufferCapacity;

        private NativeArray<float> _sizeOverLifeSamples;
        private NativeArray<float> _sizeXOverLifeSamples;
        private NativeArray<float> _sizeYOverLifeSamples;
        private NativeArray<float> _velLinXSamples;
        private NativeArray<float> _velLinYSamples;
        private NativeArray<float> _velOrbSamples;
        private NativeArray<float> _velLinXMinSamples;
        private NativeArray<float> _velLinYMinSamples;
        private NativeArray<float> _velOrbMinSamples;
        private NativeArray<float> _limitSamples;
        private NativeArray<float> _limitMinSamples;
        private NativeArray<float> _limitXSamples;
        private NativeArray<float> _limitXMinSamples;
        private NativeArray<float> _limitYSamples;
        private NativeArray<float> _limitYMinSamples;
        private NativeArray<float> _limitZSamples;
        private NativeArray<float> _limitZMinSamples;
        private NativeArray<float> _dragSamples;
        private NativeArray<float> _dragMinSamples;

        private NativeArray<float> _forceXSamples;
        private NativeArray<float> _forceXMinSamples;
        private NativeArray<float> _forceYSamples;
        private NativeArray<float> _forceYMinSamples;

        private NativeArray<Color> _gradient1Samples;
        private NativeArray<Color> _gradient2Samples;

        private NativeArray<Color> _colorBySpeedGradient1Samples;
        private NativeArray<Color> _colorBySpeedGradient2Samples;

        private NativeArray<float> _sizeBySpeedSamples;
        private NativeArray<float> _sizeXBySpeedSamples;
        private NativeArray<float> _sizeYBySpeedSamples;

        private bool _sampledDataValid;
        private bool _disposed;

        private readonly List<int> _deadIndices = new List<int>(64);

        private const int CURVE_SAMPLES = 64;
        private const int GRADIENT_SAMPLES = 64;
        private const int JOB_BATCH_SIZE = 32;

        public TrailManager GetTrailManager() => _trailManager;

        public ParticleUpdater(Core.UIParticleSystem system)
        {
            _system = system;
            _attractorManager = new AttractorManager(system);
            _collisionManager = new CollisionManager(system);
            _trailManager = new TrailManager(system);
            _textureSheetManager = new TextureSheetManager(system);
        }

        public void InvalidateSampledData() => _sampledDataValid = false;

        #region Ana Update Döngüsü

        public void UpdateParticles(float deltaTime)
        {
            var active = _system.ActiveParticles;
            int count = active.Count;
            bool useTrails = _system.TrailSettings.useTrails && _system.CanUseTrails;
            if (count == 0)
            {
                FlushEmptyTrails(useTrails);
                return;
            }

            EnsureSampledData();

            bool useAttractor = _system.AttractorSettings.useAttractor;
            bool useSubEmitters = _system.SubEmitterSettings.EnableSubEmitters
                               && _system.SubEmitterManagerInstance != null;

            _deadIndices.Clear();

            for (int i = count - 1; i >= 0; i--)
            {
                var p = active[i];
                if (p == null) { _deadIndices.Add(i); continue; }

                p.UpdateAge(deltaTime);

                if (!p.isAlive)
                    _deadIndices.Add(i);
            }

            for (int i = 0; i < _deadIndices.Count; i++)
            {
                int idx = _deadIndices[i];
                var dying = active[idx];
                if (dying != null)
                {
                    _system.TriggerParticleDied(dying, idx);
                    dying.trail?.Clear();
                    _system.Pool?.Return(dying);
                }
                active.RemoveAt(idx);
            }

            count = active.Count;
            if (count == 0)
            {
                FlushEmptyTrails(useTrails);
                return;
            }

            EnsureDataBufferCapacity(count);

            for (int i = 0; i < count; i++)
                _dataBuffer[i] = ToData(active[i]);

            var job = BuildJob(deltaTime);
            var handle = job.Schedule(count, JOB_BATCH_SIZE);
            handle.Complete();

            for (int i = 0; i < count; i++)
                FromData(_dataBuffer[i], active[i]);

            // Attractor must run AFTER Velocity Over Lifetime — VOL overwrites velocity each frame.
            if (useAttractor)
            {
                _attractorManager.InvalidatePositionCache();
                _deadIndices.Clear();

                for (int i = count - 1; i >= 0; i--)
                {
                    var p = active[i];
                    if (p == null) { _deadIndices.Add(i); continue; }

                    if (p.isAlive)
                        _attractorManager.ApplyAttractor(p, deltaTime);

                    if (!p.isAlive)
                        _deadIndices.Add(i);
                }

                for (int i = 0; i < _deadIndices.Count; i++)
                {
                    int idx = _deadIndices[i];
                    var dying = active[idx];
                    if (dying != null)
                    {
                        _system.TriggerParticleDied(dying, idx);
                        dying.trail?.Clear();
                        _system.Pool?.Return(dying);
                    }
                    active.RemoveAt(idx);
                }

                count = active.Count;
                if (count == 0)
                {
                    FlushEmptyTrails(useTrails);
                    return;
                }
            }

            ApplyCurrentInheritVelocity(active, count);

            bool useCollision = _system.CollisionSettings != null && _system.CollisionSettings.useCollision;
            if (useCollision)
            {
                _deadIndices.Clear();
                for (int i = 0; i < count; i++)
                {
                    var p = active[i];
                    if (p == null)
                    {
                        _deadIndices.Add(i);
                        continue;
                    }

                    if (_collisionManager.Resolve(p, deltaTime))
                        _system.TriggerParticleCollision(p, i);

                    if (!p.isAlive)
                        _deadIndices.Add(i);
                }

                for (int i = _deadIndices.Count - 1; i >= 0; i--)
                {
                    int idx = _deadIndices[i];
                    var dying = active[idx];
                    if (dying != null)
                    {
                        _system.TriggerParticleDied(dying, idx);
                        dying.trail?.Clear();
                        _system.Pool?.Return(dying);
                    }
                    active.RemoveAt(idx);
                }

                count = active.Count;
                if (count == 0)
                {
                    FlushEmptyTrails(useTrails);
                    return;
                }
            }

            for (int i = 0; i < count; i++)
            {
                var p = active[i];

                if (useTrails)
                    _trailManager.UpdateParticleTrail(p, deltaTime);

                if (useSubEmitters)
                    _system.SubEmitterManagerInstance.OnParticleUpdate(p);
            }

            if (useTrails)
                _trailManager.RebuildTrailMesh();
        }

        private void FlushEmptyTrails(bool useTrails)
        {
            if (useTrails)
                _trailManager.ClearTrailVisuals();
        }

        #endregion

        #region Job Builder

        private ParticleUpdateJob BuildJob(float deltaTime)
        {
            var ns = _system.NoiseSettings;
            var ss = _system.SizeSettings;
            var vs = _system.VelocityOverLifetimeSettings;
            var lvs = _system.LimitVelocityOverLifetimeSettings;
            var fls = _system.ForceOverLifetimeSettings;
            var cs = _system.ColorSettings;
            var rs = _system.RendererSettings;
            var rot = _system.RotationSettings;
            var ts = _system.TextureSheetSettings;
            var at = _system.AttractorSettings;
            var cbs = _system.ColorBySpeedSettings;
            var sbs = _system.SizeBySpeedSettings;

            bool isStretch = rs.renderMode == UIRenderMode.StretchedBillboard;

            return new ParticleUpdateJob
            {
                particles = _dataBuffer,

                deltaTime = deltaTime,

                useGravity = _system.useGravity ? (byte)1 : (byte)0,
                gravity = _system.gravity,

                useNoise = (ns.useNoise && _system.CanUseNoise) ? (byte)1 : (byte)0,
                separateAxisNoise = ns.separateAxisNoise ? (byte)1 : (byte)0,
                animateNoise = ns.animateNoise ? (byte)1 : (byte)0,
                noiseStrength = ns.noiseStrength,
                noiseStrengthX = ns.noiseStrengthX,
                noiseStrengthY = ns.noiseStrengthY,
                noiseFrequency = ns.noiseFrequency,
                noiseFrequencyX = ns.noiseFrequencyX,
                noiseFrequencyY = ns.noiseFrequencyY,
                noiseSpeed = ns.noiseSpeed,
                noiseOffsetX = ns.noiseOffset.x,
                noiseOffsetY = ns.noiseOffset.y,
                noiseType = (int)ns.noiseType,
                noiseTurbulenceOctaves = _system.MobileOptimizationMode ? 2 : 4,

                useRotationOverLifetime = rot.useRotationOverLifetime ? (byte)1 : (byte)0,
                alignToDirection = rot.alignToDirection ? (byte)1 : (byte)0,
                useRotationBySpeed = rot.useRotationBySpeed ? (byte)1 : (byte)0,
                rotationBySpeed = rot.rotationBySpeed,
                rotationBySpeedMin = rot.rotationBySpeedMin,
                rotationBySpeedMax = rot.rotationBySpeedMax,

                useStretchedBillboard = isStretch ? (byte)1 : (byte)0,
                lengthScale = rs.lengthScale,
                speedStretch = rs.speedStretch,

                useSizeOverLifetime = ss.useSizeOverLifetime ? (byte)1 : (byte)0,
                use2DSize = ss.use2DSize ? (byte)1 : (byte)0,
                use2DSizeOverLifetime = ss.use2DSizeOverLifetime ? (byte)1 : (byte)0,
                size2DMultiplierX = ss.size2DMultiplier.x,
                size2DMultiplierY = ss.size2DMultiplier.y,
                sizeOverLifeSamples = _sizeOverLifeSamples,
                sizeXOverLifeSamples = _sizeXOverLifeSamples,
                sizeYOverLifeSamples = _sizeYOverLifeSamples,

                useSizeBySpeed = sbs.useSizeBySpeed ? (byte)1 : (byte)0,
                sizeSpeedSeparateAxes = sbs.separateAxes ? (byte)1 : (byte)0,
                sizeSpeedMin = sbs.minSpeed,
                sizeSpeedMax = sbs.maxSpeed,
                sizeSpeedMultiplier = sbs.sizeMultiplier,
                sizeSpeed2DMultiplierX = sbs.size2DMultiplier.x,
                sizeSpeed2DMultiplierY = sbs.size2DMultiplier.y,
                sizeBySpeedSamples = _sizeBySpeedSamples,
                sizeXBySpeedSamples = _sizeXBySpeedSamples,
                sizeYBySpeedSamples = _sizeYBySpeedSamples,

                useGradientColor = cs.useGradientColor ? (byte)1 : (byte)0,
                fadeOut = cs.fadeOut ? (byte)1 : (byte)0,
                attractorFadeOnApproach = (at.useAttractor && at.attractorFadeOnApproach) ? (byte)1 : (byte)0,
                gradient1Samples = _gradient1Samples,
                gradient2Samples = _gradient2Samples,

                useColorBySpeed = cbs.useColorBySpeed ? (byte)1 : (byte)0,
                colorSpeedMin = cbs.minSpeed,
                colorSpeedMax = cbs.maxSpeed,
                colorBySpeedGradient1Samples = _colorBySpeedGradient1Samples,
                colorBySpeedGradient2Samples = _colorBySpeedGradient2Samples,

                useVelocityOverLifetime = vs.useVelocityOverLifetime ? (byte)1 : (byte)0,
                useLinearVelocity = vs.useLinearVelocity ? (byte)1 : (byte)0,
                useLinearVelocityCurves = vs.useLinearVelocityCurves ? (byte)1 : (byte)0,
                linearVelocityX = vs.linearVelocity.x,
                linearVelocityY = vs.linearVelocity.y,
                useRandomLinearVelocity = vs.useRandomLinearVelocity ? (byte)1 : (byte)0,
                useOrbitalVelocity = vs.useOrbitalVelocity ? (byte)1 : (byte)0,
                useOrbitalVelocityCurve = vs.useOrbitalVelocityCurve ? (byte)1 : (byte)0,
                orbitalVelocity = vs.orbitalVelocity,
                orbitalCenterX = vs.orbitalCenter.x,
                orbitalCenterY = vs.orbitalCenter.y,
                useRandomOrbitalVelocity = vs.useRandomOrbitalVelocity ? (byte)1 : (byte)0,
                velocityUseLocalSpace = vs.useLocalSpace ? (byte)1 : (byte)0,
                systemTransformMatrix = _system.Transform.localToWorldMatrix,
                velLinearXSamples = _velLinXSamples,
                velLinearYSamples = _velLinYSamples,
                velOrbitalSamples = _velOrbSamples,
                velLinearXMinSamples = _velLinXMinSamples,
                velLinearYMinSamples = _velLinYMinSamples,
                velOrbitalMinSamples = _velOrbMinSamples,

                useLimitVelocityOverLifetime = lvs.useLimitVelocityOverLifetime ? (byte)1 : (byte)0,
                limitSeparateAxes = lvs.separateAxes ? (byte)1 : (byte)0,
                limitMode = lvs.limitMode,
                limitConstant = lvs.limit,
                limitMin = lvs.limitMin,
                limitXMode = lvs.limitXMode,
                limitXConstant = lvs.limitX,
                limitXMin = lvs.limitXMin,
                limitYMode = lvs.limitYMode,
                limitYConstant = lvs.limitY,
                limitYMin = lvs.limitYMin,
                limitZMode = lvs.limitZMode,
                limitZConstant = lvs.limitZ,
                limitZMin = lvs.limitZMin,
                limitDampen = lvs.dampen,
                limitSamples = _limitSamples,
                limitMinSamples = _limitMinSamples,
                limitXSamples = _limitXSamples,
                limitXMinSamples = _limitXMinSamples,
                limitYSamples = _limitYSamples,
                limitYMinSamples = _limitYMinSamples,
                limitZSamples = _limitZSamples,
                limitZMinSamples = _limitZMinSamples,
                dragMode = lvs.dragMode,
                dragConstant = lvs.drag,
                dragMin = lvs.dragMin,
                dragMultiplyBySize = lvs.multiplyDragBySize ? (byte)1 : (byte)0,
                dragMultiplyByVelocity = lvs.multiplyDragByVelocity ? (byte)1 : (byte)0,
                dragSamples = _dragSamples,
                dragMinSamples = _dragMinSamples,

                useForceOverLifetime = fls.useForceOverLifetime ? (byte)1 : (byte)0,
                forceXMode = fls.mode,
                forceXConstant = fls.x,
                forceXMin = fls.xMin,
                forceYMode = fls.mode,
                forceYConstant = fls.y,
                forceYMin = fls.yMin,
                forceUseLocalSpace = fls.useLocalSpace ? (byte)1 : (byte)0,
                forceXSamples = _forceXSamples,
                forceXMinSamples = _forceXMinSamples,
                forceYSamples = _forceYSamples,
                forceYMinSamples = _forceYMinSamples,

                useAnimation = (ts.useTextureSheetAnimation && ts.animationSpeed > 0f) ? (byte)1 : (byte)0,
                animationSpeed = ts.animationSpeed,
                loopAnimation = ts.loopAnimation ? (byte)1 : (byte)0,
                animationTimeMode = (int)ts.timeMode,
            };
        }

        #endregion

        #region Inherit Velocity (Current)

        private void ApplyCurrentInheritVelocity(List<Particle> active, int count)
        {
            var iv = _system.InheritVelocitySettings;
            if (iv == null || !iv.useInheritVelocity || iv.mode != InheritVelocityMode.Current)
                return;

            if (_system.EmissionSettings.simulationSpace != SimulationSpace.World)
                return;

            Vector3 delta = _system.GetEmitterDeltaInSimSpace() * iv.multiplier;
            if (delta.sqrMagnitude < 0.0000001f)
                return;

            for (int i = 0; i < count; i++)
            {
                var p = active[i];
                if (p == null) continue;

                p.position += delta;
                p.points[0] += delta;
                p.points[1] += delta;
                p.points[2] += delta;
                p.points[3] += delta;
            }
        }

        #endregion

        #region Particle ↔ ParticleData Dönüşüm

        private static ParticleData ToData(Particle src)
        {
            return new ParticleData
            {
                position = src.position,
                velocity = src.velocity,
                originalVelocity = src.originalVelocity,
                lifetime = src.lifetime,
                age = src.age,
                maxLifetime = src.maxLifetime,
                isAlive = src.isAlive ? (byte)1 : (byte)0,
                size = src.size,
                initialSize = src.initialSize,
                size2D = src.size2D,
                initialSize2D = src.initialSize2D,
                initialColor = src.initialColor,
                rotation = src.rotation,
                angularVelocity = src.angularVelocity,
                attractorFadeAlpha = src.attractorFadeAlpha,
                noiseOffset = src.noiseOffset,
                noiseTime = src.noiseTime,
                noiseSeed = src.noiseSeed,
                stretchedRotation = src.stretchedRotation,
                velocityRandomLinearOffset = src.velocityRandomLinearOffset,
                velocityRandomOrbitalOffset = src.velocityRandomOrbitalOffset,
                limitVelocityRandomLerp = src.limitVelocityRandomLerp,
                limitVelocityRandomAxesLerp = src.limitVelocityRandomAxesLerp,
                dragRandomLerp = src.dragRandomLerp,
                forceRandomLerpX = src.forceRandomLerpX,
                forceRandomLerpY = src.forceRandomLerpY,
                currentFrame = src.currentFrame,
                animationTime = src.animationTime,
                startFrame = src.startFrame,
                endFrame = src.endFrame,
                hasAnimation = src.hasAnimation ? (byte)1 : (byte)0,
                useGradient = src.useGradient ? (byte)1 : (byte)0,
                useSecondGradient = src.useSecondGradient ? (byte)1 : (byte)0,
            };
        }

        private static void FromData(in ParticleData src, Particle dst)
        {
            dst.position = src.position;
            dst.velocity = src.velocity;
            dst.age = src.age;
            dst.lifetime = src.lifetime;
            dst.isAlive = src.isAlive == 1;
            dst.size2D = src.size2D;
            dst.color = src.color;
            dst.rotation = src.rotation;
            dst.noiseTime = src.noiseTime;
            dst.animationTime = src.animationTime;
            dst.currentFrame = src.currentFrame;
            dst.stretchedRotation = src.stretchedRotation;
            dst.stretchedWidth = src.stretchedWidth;
            dst.stretchedHeight = src.stretchedHeight;
            dst.attractorFadeAlpha = src.attractorFadeAlpha;
            dst.points[0] = src.point0;
            dst.points[1] = src.point1;
            dst.points[2] = src.point2;
            dst.points[3] = src.point3;
        }

        #endregion

        #region Buffer Yönetimi

        private void EnsureDataBufferCapacity(int count)
        {
            bool valid = _dataBuffer.IsCreated;
            if (valid && _dataBufferCapacity >= count) return;

            int newCap = Mathf.Max(_dataBufferCapacity * 2, count + 32);
            if (_dataBuffer.IsCreated) _dataBuffer.Dispose();
            _dataBuffer = new NativeArray<ParticleData>(
                newCap, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            _dataBufferCapacity = newCap;
        }

        private static void EnsureArrayCreated<T>(ref NativeArray<T> arr, int size) where T : struct
        {
            if (!arr.IsCreated)
            {
                arr = new NativeArray<T>(size, Allocator.Persistent);
            }
        }

        private void EnsureSampledData()
        {
            EnsureArrayCreated(ref _sizeOverLifeSamples, CURVE_SAMPLES);
            EnsureArrayCreated(ref _sizeXOverLifeSamples, CURVE_SAMPLES);
            EnsureArrayCreated(ref _sizeYOverLifeSamples, CURVE_SAMPLES);
            EnsureArrayCreated(ref _velLinXSamples, CURVE_SAMPLES);
            EnsureArrayCreated(ref _velLinYSamples, CURVE_SAMPLES);
            EnsureArrayCreated(ref _velOrbSamples, CURVE_SAMPLES);
            EnsureArrayCreated(ref _velLinXMinSamples, CURVE_SAMPLES);
            EnsureArrayCreated(ref _velLinYMinSamples, CURVE_SAMPLES);
            EnsureArrayCreated(ref _velOrbMinSamples, CURVE_SAMPLES);

            EnsureArrayCreated(ref _limitSamples, CURVE_SAMPLES);
            EnsureArrayCreated(ref _limitMinSamples, CURVE_SAMPLES);
            EnsureArrayCreated(ref _limitXSamples, CURVE_SAMPLES);
            EnsureArrayCreated(ref _limitXMinSamples, CURVE_SAMPLES);
            EnsureArrayCreated(ref _limitYSamples, CURVE_SAMPLES);
            EnsureArrayCreated(ref _limitYMinSamples, CURVE_SAMPLES);
            EnsureArrayCreated(ref _limitZSamples, CURVE_SAMPLES);
            EnsureArrayCreated(ref _limitZMinSamples, CURVE_SAMPLES);
            EnsureArrayCreated(ref _dragSamples, CURVE_SAMPLES);
            EnsureArrayCreated(ref _dragMinSamples, CURVE_SAMPLES);

            EnsureArrayCreated(ref _forceXSamples, CURVE_SAMPLES);
            EnsureArrayCreated(ref _forceXMinSamples, CURVE_SAMPLES);
            EnsureArrayCreated(ref _forceYSamples, CURVE_SAMPLES);
            EnsureArrayCreated(ref _forceYMinSamples, CURVE_SAMPLES);

            EnsureArrayCreated(ref _gradient1Samples, GRADIENT_SAMPLES);
            EnsureArrayCreated(ref _gradient2Samples, GRADIENT_SAMPLES);

            EnsureArrayCreated(ref _colorBySpeedGradient1Samples, GRADIENT_SAMPLES);
            EnsureArrayCreated(ref _colorBySpeedGradient2Samples, GRADIENT_SAMPLES);

            EnsureArrayCreated(ref _sizeBySpeedSamples, CURVE_SAMPLES);
            EnsureArrayCreated(ref _sizeXBySpeedSamples, CURVE_SAMPLES);
            EnsureArrayCreated(ref _sizeYBySpeedSamples, CURVE_SAMPLES);

            if (_sampledDataValid) return;

            _sampledDataValid = true;
            RebuildSampledData();
        }

        public void UpdateParticlesForPrewarm(float deltaTime)
        {
            var active = _system.ActiveParticles;
            int count = active.Count;
            if (count == 0) return;

            EnsureSampledData();

            _deadIndices.Clear();
            for (int i = count - 1; i >= 0; i--)
            {
                var p = active[i];
                if (p == null) { _deadIndices.Add(i); continue; }
                p.UpdateAge(deltaTime);
                if (!p.isAlive) _deadIndices.Add(i);
            }

            for (int i = 0; i < _deadIndices.Count; i++)
            {
                int idx = _deadIndices[i];
                var dying = active[idx];
                if (dying != null)
                {
                    dying.trail?.Clear();
                    _system.Pool?.Return(dying);
                }
                active.RemoveAt(idx);
            }

            count = active.Count;
            if (count == 0) return;

            EnsureDataBufferCapacity(count);

            for (int i = 0; i < count; i++)
                _dataBuffer[i] = ToData(active[i]);

            var job = BuildJob(deltaTime);
            var handle = job.Schedule(count, JOB_BATCH_SIZE);
            handle.Complete();

            for (int i = 0; i < count; i++)
                FromData(_dataBuffer[i], active[i]);

        }

        private void RebuildSampledData()
        {
            var ss = _system.SizeSettings;
            var vs = _system.VelocityOverLifetimeSettings;
            var lvs = _system.LimitVelocityOverLifetimeSettings;
            var cs = _system.ColorSettings;

            float sizeMult = ss.sizeMultiplier;
            if (float.IsNaN(sizeMult) || float.IsInfinity(sizeMult) || sizeMult <= 0.001f) sizeMult = 1f;

            float sizeXMult = ss.size2DMultiplier.x;
            if (float.IsNaN(sizeXMult) || float.IsInfinity(sizeXMult) || sizeXMult <= 0.001f) sizeXMult = 1f;

            float sizeYMult = ss.size2DMultiplier.y;
            if (float.IsNaN(sizeYMult) || float.IsInfinity(sizeYMult) || sizeYMult <= 0.001f) sizeYMult = 1f;

            FillCurve(_sizeOverLifeSamples, ss.sizeOverLifetimeCurve, sizeMult, 1f);
            FillCurve(_sizeXOverLifeSamples, ss.sizeXOverLifetimeCurve, sizeXMult, 1f);
            FillCurve(_sizeYOverLifeSamples, ss.sizeYOverLifetimeCurve, sizeYMult, 1f);
            FillCurve(_velLinXSamples, vs.linearVelocityX, 1f, 0f);
            FillCurve(_velLinYSamples, vs.linearVelocityY, 1f, 0f);
            FillCurve(_velOrbSamples, vs.orbitalVelocityCurve, 1f, 0f);
            FillCurve(_velLinXMinSamples, vs.linearVelocityXMinCurve, 1f, 0f);
            FillCurve(_velLinYMinSamples, vs.linearVelocityYMinCurve, 1f, 0f);
            FillCurve(_velOrbMinSamples, vs.orbitalVelocityMinCurve, 1f, 0f);

            FillCurve(_limitSamples, lvs.limitCurve, 1f, 1000f);
            FillCurve(_limitMinSamples, lvs.limitMinCurve, 1f, 1000f);
            FillCurve(_limitXSamples, lvs.limitXCurve, 1f, 1000f);
            FillCurve(_limitXMinSamples, lvs.limitXMinCurve, 1f, 1000f);
            FillCurve(_limitYSamples, lvs.limitYCurve, 1f, 1000f);
            FillCurve(_limitYMinSamples, lvs.limitYMinCurve, 1f, 1000f);
            FillCurve(_limitZSamples, lvs.limitZCurve, 1f, 1000f);
            FillCurve(_limitZMinSamples, lvs.limitZMinCurve, 1f, 1000f);
            FillCurve(_dragSamples, lvs.dragCurve, 1f, 0f);
            FillCurve(_dragMinSamples, lvs.dragMinCurve, 1f, 0f);

            var fls = _system.ForceOverLifetimeSettings;
            FillCurve(_forceXSamples, fls.xCurve, 1f, 0f);
            FillCurve(_forceXMinSamples, fls.xMinCurve, 1f, 0f);
            FillCurve(_forceYSamples, fls.yCurve, 1f, 0f);
            FillCurve(_forceYMinSamples, fls.yMinCurve, 1f, 0f);

            FillGradient(_gradient1Samples, cs.colorGradient);
            FillGradient(_gradient2Samples, cs.colorGradient2);

            var cbs = _system.ColorBySpeedSettings;
            var sbs = _system.SizeBySpeedSettings;

            FillGradient(_colorBySpeedGradient1Samples, cbs.colorGradient);
            FillGradient(_colorBySpeedGradient2Samples, cbs.colorGradient2);

            float sbsMult = sbs.sizeMultiplier;
            if (float.IsNaN(sbsMult) || float.IsInfinity(sbsMult) || sbsMult <= 0.001f) sbsMult = 1f;

            float sbsXMult = sbs.size2DMultiplier.x;
            if (float.IsNaN(sbsXMult) || float.IsInfinity(sbsXMult) || sbsXMult <= 0.001f) sbsXMult = 1f;

            float sbsYMult = sbs.size2DMultiplier.y;
            if (float.IsNaN(sbsYMult) || float.IsInfinity(sbsYMult) || sbsYMult <= 0.001f) sbsYMult = 1f;

            FillCurve(_sizeBySpeedSamples, sbs.sizeCurve, sbsMult, 1f);
            FillCurve(_sizeXBySpeedSamples, sbs.sizeXCurve, sbsXMult, 1f);
            FillCurve(_sizeYBySpeedSamples, sbs.sizeYCurve, sbsYMult, 1f);
        }

        private static void FillCurve(NativeArray<float> arr, AnimationCurve curve, float multiplier = 1f, float defaultValue = 1f)
        {
            if (float.IsNaN(multiplier) || float.IsInfinity(multiplier))
                multiplier = 1f;

            float inv = 1f / (CURVE_SAMPLES - 1);
            bool isValid = curve != null && curve.keys != null && curve.keys.Length > 0;
            for (int i = 0; i < CURVE_SAMPLES; i++)
            {
                float val = isValid ? curve.Evaluate(i * inv) : defaultValue;
                if (float.IsNaN(val) || float.IsInfinity(val))
                    val = defaultValue;
                arr[i] = val * multiplier;
            }
        }

        private static void FillGradient(NativeArray<Color> arr, Gradient gradient)
        {
            float inv = 1f / (GRADIENT_SAMPLES - 1);
            for (int i = 0; i < GRADIENT_SAMPLES; i++)
                arr[i] = gradient != null ? gradient.Evaluate(i * inv) : Color.white;
        }

        private void DisposeSampledData()
        {
            if (_sizeOverLifeSamples.IsCreated) _sizeOverLifeSamples.Dispose(); _sizeOverLifeSamples = default;
            if (_sizeXOverLifeSamples.IsCreated) _sizeXOverLifeSamples.Dispose(); _sizeXOverLifeSamples = default;
            if (_sizeYOverLifeSamples.IsCreated) _sizeYOverLifeSamples.Dispose(); _sizeYOverLifeSamples = default;
            if (_velLinXSamples.IsCreated) _velLinXSamples.Dispose(); _velLinXSamples = default;
            if (_velLinYSamples.IsCreated) _velLinYSamples.Dispose(); _velLinYSamples = default;
            if (_velOrbSamples.IsCreated) _velOrbSamples.Dispose(); _velOrbSamples = default;
            if (_velLinXMinSamples.IsCreated) _velLinXMinSamples.Dispose(); _velLinXMinSamples = default;
            if (_velLinYMinSamples.IsCreated) _velLinYMinSamples.Dispose(); _velLinYMinSamples = default;
            if (_velOrbMinSamples.IsCreated) _velOrbMinSamples.Dispose(); _velOrbMinSamples = default;
            
            if (_limitSamples.IsCreated) _limitSamples.Dispose(); _limitSamples = default;
            if (_limitMinSamples.IsCreated) _limitMinSamples.Dispose(); _limitMinSamples = default;
            if (_limitXSamples.IsCreated) _limitXSamples.Dispose(); _limitXSamples = default;
            if (_limitXMinSamples.IsCreated) _limitXMinSamples.Dispose(); _limitXMinSamples = default;
            if (_limitYSamples.IsCreated) _limitYSamples.Dispose(); _limitYSamples = default;
            if (_limitYMinSamples.IsCreated) _limitYMinSamples.Dispose(); _limitYMinSamples = default;
            if (_limitZSamples.IsCreated) _limitZSamples.Dispose(); _limitZSamples = default;
            if (_limitZMinSamples.IsCreated) _limitZMinSamples.Dispose(); _limitZMinSamples = default;
            if (_dragSamples.IsCreated) _dragSamples.Dispose(); _dragSamples = default;
            if (_dragMinSamples.IsCreated) _dragMinSamples.Dispose(); _dragMinSamples = default;

            if (_forceXSamples.IsCreated) _forceXSamples.Dispose(); _forceXSamples = default;
            if (_forceXMinSamples.IsCreated) _forceXMinSamples.Dispose(); _forceXMinSamples = default;
            if (_forceYSamples.IsCreated) _forceYSamples.Dispose(); _forceYSamples = default;
            if (_forceYMinSamples.IsCreated) _forceYMinSamples.Dispose(); _forceYMinSamples = default;

            if (_gradient1Samples.IsCreated) _gradient1Samples.Dispose(); _gradient1Samples = default;
            if (_gradient2Samples.IsCreated) _gradient2Samples.Dispose(); _gradient2Samples = default;

            if (_colorBySpeedGradient1Samples.IsCreated) _colorBySpeedGradient1Samples.Dispose(); _colorBySpeedGradient1Samples = default;
            if (_colorBySpeedGradient2Samples.IsCreated) _colorBySpeedGradient2Samples.Dispose(); _colorBySpeedGradient2Samples = default;

            if (_sizeBySpeedSamples.IsCreated) _sizeBySpeedSamples.Dispose(); _sizeBySpeedSamples = default;
            if (_sizeXBySpeedSamples.IsCreated) _sizeXBySpeedSamples.Dispose(); _sizeXBySpeedSamples = default;
            if (_sizeYBySpeedSamples.IsCreated) _sizeYBySpeedSamples.Dispose(); _sizeYBySpeedSamples = default;
        }

        #endregion

        #region Cleanup / Dispose

        public void Cleanup()
        {
            _trailManager?.Cleanup();
            _deadIndices.Clear();
            Dispose();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            DisposeSampledData();

            if (_dataBuffer.IsCreated) _dataBuffer.Dispose();
            _dataBufferCapacity = 0;
        }

        #endregion
    }
}