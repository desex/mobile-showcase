using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TierStudios.UIParticleSystem.Core;
using TierStudios.UIParticleSystem.Settings;
using TierStudios.UIParticleSystem.Enums;

namespace TierStudios.UIParticleSystem.Editor
{
    public static class UIParticleSystemCloner
    {
        private const float DEFAULT_SCALE = 50f;
        private const string LOG_PREFIX = "[UIParticleSystem Cloner]";


        [MenuItem("GameObject/Clone to UI Particle System", false, 20)]
        private static void CloneMenuCommand(MenuCommand menuCommand)
        {
            GameObject selected = menuCommand.context as GameObject;
            if (selected == null)
                selected = Selection.activeGameObject;

            if (selected == null) return;

            var ps = selected.GetComponent<ParticleSystem>();
            if (ps == null) return;

            Clone(ps);
        }

        [MenuItem("GameObject/Clone to UI Particle System", true)]
        private static bool CloneMenuValidate()
        {
            if (Selection.activeGameObject == null) return false;
            return Selection.activeGameObject.GetComponent<ParticleSystem>() != null;
        }

        public static Core.UIParticleSystem Clone(ParticleSystem source, float sizeScale = DEFAULT_SCALE)
        {
            if (source == null)
            {
                Debug.LogError($"{LOG_PREFIX} Source ParticleSystem is null.");
                return null;
            }

            Canvas canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();

                if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
                {
                    GameObject esObj = new GameObject("EventSystem");
                    esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                    esObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                }
            }

            GameObject rootGo = new GameObject($"{source.gameObject.name} (UI Clone)");
            rootGo.AddComponent<RectTransform>();
            GameObjectUtility.SetParentAndAlign(rootGo, canvas.gameObject);

            var rootRect = rootGo.GetComponent<RectTransform>();
            rootRect.anchoredPosition = Vector2.zero;
            rootRect.sizeDelta = new Vector2(200, 200);

            var uiPS = rootGo.AddComponent<Core.UIParticleSystem>();

            var report = new List<CloneReportWindow.ReportEntry>();

            ApplyMainModule(source, uiPS, sizeScale, report);
            ApplyEmissionModule(source, uiPS, report);
            ApplyShapeModule(source, uiPS, sizeScale, report);
            ApplyColorOverLifetime(source, uiPS, report);
            ApplyColorBySpeed(source, uiPS, sizeScale, report);
            ApplySizeOverLifetime(source, uiPS, report);
            ApplySizeBySpeed(source, uiPS, sizeScale, report);
            ApplyRotationOverLifetime(source, uiPS, report);
            ApplyRotationBySpeed(source, uiPS, sizeScale, report);
            ApplyNoiseModule(source, uiPS, sizeScale, report);
            ApplyVelocityOverLifetime(source, uiPS, sizeScale, report);
            ApplyLimitVelocityOverLifetime(source, uiPS, sizeScale, report);
            ApplyInheritVelocity(source, uiPS, sizeScale, report);
            ApplyForceOverLifetime(source, uiPS, sizeScale, report);
            ApplyCollisionModule(source, uiPS, sizeScale, report);
            ApplyTrailsModule(source, uiPS, report);
            ApplyTextureSheetAnimation(source, uiPS, report);

            bool spriteAssigned = ApplyRendererModule(source, uiPS, sizeScale, report);

            if (!spriteAssigned)
            {
                uiPS.LoadDefaultSprite();
            }

            CloneChildParticleSystems(source.transform, rootGo.transform, canvas, sizeScale, uiPS, report);

            CollectSkippedModules(source, report);

            Undo.RegisterCreatedObjectUndo(rootGo, "Clone to UI Particle System");
            Selection.activeGameObject = rootGo;

            CloneReportWindow.Show(source.gameObject.name, rootGo.name, report);

            return uiPS;
        }

        private static void AddReport(List<CloneReportWindow.ReportEntry> report, string module, string detail, CloneReportWindow.EntryType type)
        {
            report.Add(new CloneReportWindow.ReportEntry
            {
                module = module,
                detail = detail,
                type = type
            });
        }

        private static bool ApplyRendererModule(ParticleSystem source, Core.UIParticleSystem target, float scale, List<CloneReportWindow.ReportEntry> report)
        {
            var renderer = source.GetComponent<ParticleSystemRenderer>();
            if (renderer == null) return false;

            var so = new SerializedObject(target);
            bool spriteSet = false;

            if (renderer.sharedMaterial != null)
            {
                Texture2D tex = FindMaterialTexture(renderer.sharedMaterial);

                if (tex != null)
                {
                    Sprite sprite = ConvertTextureToSprite(tex);
                    if (sprite != null)
                    {
                        so.FindProperty("particleSprite").objectReferenceValue = sprite;
                        spriteSet = true;
                        AddReport(report, "Renderer — Sprite", $"Texture '{tex.name}' converted to Sprite", CloneReportWindow.EntryType.Success);
                    }
                }

                string shaderName = renderer.sharedMaterial.shader.name.ToLowerInvariant();
                bool isAdditive = shaderName.Contains("additive") ||
                                  shaderName.Contains("add") ||
                                  (renderer.sharedMaterial.HasProperty("_Mode") &&
                                   renderer.sharedMaterial.GetFloat("_Mode") > 1.5f);

                so.FindProperty("blendMode").enumValueIndex = isAdditive ? 1 : 0;

                if (isAdditive)
                    AddReport(report, "Renderer — Blend Mode", $"Additive (detected from shader '{renderer.sharedMaterial.shader.name}')", CloneReportWindow.EntryType.Success);
            }

            if (renderer.renderMode == ParticleSystemRenderMode.Stretch)
            {
                var rendSettings = so.FindProperty("rendererSettings");
                rendSettings.FindPropertyRelative("renderMode").enumValueIndex = 1;

                rendSettings.FindPropertyRelative("speedStretch").floatValue = Mathf.Clamp(renderer.velocityScale, 0f, 1f);
                rendSettings.FindPropertyRelative("lengthScale").floatValue = Mathf.Clamp(renderer.lengthScale, 0.01f, 10f);
                AddReport(report, "Renderer — Stretched Billboard", $"speedStretch={renderer.velocityScale:F4}, lengthScale={renderer.lengthScale:F2}", CloneReportWindow.EntryType.Success);
            }

            so.ApplyModifiedPropertiesWithoutUndo();
            target.UpdateParticlesMaterial();
            return spriteSet;
        }

        private static Texture2D FindMaterialTexture(Material material)
        {
            if (material == null) return null;

            string[] texturePropertyNames = new[]
            {
                "_MainTex",
                "_BaseMap",
                "_BaseColorMap",
                "_EmissionMap",
                "_BumpMap"
            };

            if (material.mainTexture is Texture2D mainTex)
                return mainTex;

            foreach (string propName in texturePropertyNames)
            {
                if (material.HasProperty(propName))
                {
                    Texture tex = material.GetTexture(propName);
                    if (tex is Texture2D tex2D)
                        return tex2D;
                }
            }

            Shader shader = material.shader;
            int propCount = shader.GetPropertyCount();
            for (int i = 0; i < propCount; i++)
            {
                if (shader.GetPropertyType(i) == UnityEngine.Rendering.ShaderPropertyType.Texture)
                {
                    string propName = shader.GetPropertyName(i);
                    Texture tex = material.GetTexture(propName);
                    if (tex is Texture2D tex2D)
                        return tex2D;
                }
            }

            return null;
        }

        private static void ApplyMainModule(ParticleSystem source, Core.UIParticleSystem target, float scale, List<CloneReportWindow.ReportEntry> report)
        {
            var main = source.main;
            var so = new SerializedObject(target);

            var emission = so.FindProperty("emissionSettings");
            emission.FindPropertyRelative("duration").floatValue = main.duration;
            emission.FindPropertyRelative("looping").boolValue = main.loop;
            emission.FindPropertyRelative("prewarm").boolValue = main.prewarm;
            emission.FindPropertyRelative("emitOnStart").boolValue = main.playOnAwake;
            emission.FindPropertyRelative("startDelay").floatValue = ReadMinMaxCurveConstant(main.startDelay);
            emission.FindPropertyRelative("maxParticles").intValue = Mathf.Min(main.maxParticles, 1000);

            int simSpace = main.simulationSpace == ParticleSystemSimulationSpace.World ? 1 : 0;
            emission.FindPropertyRelative("simulationSpace").enumValueIndex = simSpace;

            so.FindProperty("simulationSpeed").floatValue = main.simulationSpeed;
            so.FindProperty("useUnscaledTime").boolValue = main.useUnscaledTime;

            var lifetime = so.FindProperty("lifetimeSettings");
            ApplyMinMaxCurveToRange(main.startLifetime, lifetime,
                "lifetime", "useRandomLifetime", "minLifetime", "maxLifetime");

            ApplyMinMaxCurveToSpeed(main.startSpeed, so, scale);
            ApplyStartSize(main, so, scale);
            ApplyStartColor(main.startColor, so);
            ApplyStartRotation(main, so);

            float gravMod = ReadMinMaxCurveConstant(main.gravityModifier);
            if (Mathf.Abs(gravMod) > 0.001f)
            {
                so.FindProperty("useGravity").boolValue = true;
                float gravityY = Physics.gravity.y * gravMod * scale;
                so.FindProperty("gravity").vector2Value = new Vector2(0f, gravityY);
                AddReport(report, "Main — Gravity", $"modifier={gravMod:F2} → (0, {gravityY:F1})", CloneReportWindow.EntryType.Success);
            }

            so.ApplyModifiedPropertiesWithoutUndo();
            AddReport(report, "Main Module", $"duration={main.duration:F1}, loop={main.loop}, maxParticles={main.maxParticles}", CloneReportWindow.EntryType.Success);
        }

        private static void ApplyEmissionModule(ParticleSystem source, Core.UIParticleSystem target, List<CloneReportWindow.ReportEntry> report)
        {
            var em = source.emission;
            if (!em.enabled) return;

            var so = new SerializedObject(target);
            var emission = so.FindProperty("emissionSettings");
            if (emission == null) return;

            var rateProp = emission.FindPropertyRelative("emissionRate");
            if (rateProp != null)
                rateProp.floatValue = ReadMinMaxCurveConstant(em.rateOverTime);

            float distRate = ReadMinMaxCurveConstant(em.rateOverDistance);
            if (distRate > 0.001f)
            {
                var useDistProp = emission.FindPropertyRelative("useEmissionRateOverDistance");
                if (useDistProp != null)
                    useDistProp.boolValue = true;

                var distProp = emission.FindPropertyRelative("emissionRateOverDistance");
                if (distProp != null)
                    distProp.floatValue = distRate;
            }

            int burstCount = em.burstCount;
            if (burstCount > 0)
            {
                var bursts = emission.FindPropertyRelative("bursts");
                if (bursts != null)
                {
                    int clampedCount = Mathf.Min(burstCount, 5);
                    bursts.arraySize = clampedCount;

                    var srcBursts = new ParticleSystem.Burst[burstCount];
                    em.GetBursts(srcBursts);

                    for (int i = 0; i < clampedCount; i++)
                    {
                        var burst = bursts.GetArrayElementAtIndex(i);
                        burst.FindPropertyRelative("time").floatValue = srcBursts[i].time;
                        burst.FindPropertyRelative("count").intValue = (int)ReadMinMaxCurveConstant(srcBursts[i].count);
                        burst.FindPropertyRelative("probability").floatValue = srcBursts[i].probability;
                    }

                    if (burstCount > 5)
                        AddReport(report, "Emission — Bursts", $"{burstCount} bursts found, only first 5 cloned (max limit)", CloneReportWindow.EntryType.Warning);
                }
            }

            so.ApplyModifiedPropertiesWithoutUndo();
            AddReport(report, "Emission", $"rate={ReadMinMaxCurveConstant(em.rateOverTime):F1}, bursts={burstCount}", CloneReportWindow.EntryType.Success);
        }

        private static void ApplyShapeModule(ParticleSystem source, Core.UIParticleSystem target, float scale, List<CloneReportWindow.ReportEntry> report)
        {
            var shape = source.shape;
            if (!shape.enabled) return;

            var so = new SerializedObject(target);
            var emission = so.FindProperty("emissionSettings");

            switch (shape.shapeType)
            {
                case ParticleSystemShapeType.Cone:
                case ParticleSystemShapeType.ConeVolume:
                    emission.FindPropertyRelative("emissionShape").enumValueIndex = (int)EmissionShapeType.Cone;
                    emission.FindPropertyRelative("coneAngle").floatValue = shape.angle * 2f;
                    emission.FindPropertyRelative("coneRange").floatValue = shape.radius * scale;
                    emission.FindPropertyRelative("emissionDirection").vector2Value = Vector2.up;
                    AddReport(report, "Shape — Cone", $"angle={shape.angle * 2f:F0}°, range={shape.radius * scale:F0}", CloneReportWindow.EntryType.Success);
                    break;

                case ParticleSystemShapeType.Sphere:
                case ParticleSystemShapeType.Hemisphere:
                    emission.FindPropertyRelative("emissionShape").enumValueIndex = (int)EmissionShapeType.Circle;
                    emission.FindPropertyRelative("circleRadius").floatValue = shape.radius * scale;
                    emission.FindPropertyRelative("randomizeDirection").boolValue = true;
                    AddReport(report, "Shape — Circle", $"Sphere → Circle (radius={shape.radius * scale:F0})", CloneReportWindow.EntryType.Success);
                    break;

                case ParticleSystemShapeType.Circle:
                    emission.FindPropertyRelative("emissionShape").enumValueIndex = (int)EmissionShapeType.Circle;
                    emission.FindPropertyRelative("circleRadius").floatValue = shape.radius * scale;
                    emission.FindPropertyRelative("randomizeDirection").boolValue = true;
                    AddReport(report, "Shape — Circle", $"radius={shape.radius * scale:F0}", CloneReportWindow.EntryType.Success);
                    break;

                case ParticleSystemShapeType.Box:
                case ParticleSystemShapeType.BoxShell:
                    bool isEdge = shape.shapeType == ParticleSystemShapeType.BoxShell;
                    emission.FindPropertyRelative("emissionShape").enumValueIndex =
                        isEdge ? (int)EmissionShapeType.Edge : (int)EmissionShapeType.Rectangle;
                    emission.FindPropertyRelative("emissionAreaSize").vector2Value =
                        new Vector2(shape.scale.x * scale, shape.scale.y * scale);
                    if (isEdge)
                        emission.FindPropertyRelative("emitFromEdge").boolValue = true;
                    AddReport(report, $"Shape — {(isEdge ? "Edge" : "Rectangle")}", $"{shape.scale.x * scale:F0}x{shape.scale.y * scale:F0}", CloneReportWindow.EntryType.Success);
                    break;

                case ParticleSystemShapeType.BoxEdge:
                    emission.FindPropertyRelative("emissionShape").enumValueIndex = (int)EmissionShapeType.Line;
                    emission.FindPropertyRelative("lineLength").floatValue = shape.scale.x * scale;
                    AddReport(report, "Shape — Line", $"BoxEdge → Line (length={shape.scale.x * scale:F0})", CloneReportWindow.EntryType.Success);
                    break;

                case ParticleSystemShapeType.Sprite:
                case ParticleSystemShapeType.SpriteRenderer:
                    ApplySpriteShape(shape, emission, scale, report);
                    break;

                default:
                    emission.FindPropertyRelative("emissionShape").enumValueIndex = (int)EmissionShapeType.Point;
                    emission.FindPropertyRelative("randomizeDirection").boolValue = true;
                    AddReport(report, "Shape", $"{shape.shapeType} not supported → Point fallback", CloneReportWindow.EntryType.Warning);
                    break;
            }

            if (shape.randomDirectionAmount > 0.01f)
            {
                emission.FindPropertyRelative("directionVariance").floatValue = shape.randomDirectionAmount * 180f;
            }

            if (shape.alignToDirection)
            {
                so.FindProperty("rotationSettings").FindPropertyRelative("alignToDirection").boolValue = true;
                AddReport(report, "Align To Direction", "enabled from Shape module", CloneReportWindow.EntryType.Success);
            }

            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ApplySpriteShape(ParticleSystem.ShapeModule shape, SerializedProperty emission, float scale, List<CloneReportWindow.ReportEntry> report)
        {
            Sprite sprite = shape.sprite;
            if (shape.shapeType == ParticleSystemShapeType.SpriteRenderer && shape.spriteRenderer != null)
                sprite = shape.spriteRenderer.sprite;

            emission.FindPropertyRelative("emissionShape").enumValueIndex = (int)EmissionShapeType.Sprite;
            emission.FindPropertyRelative("shapeSprite").objectReferenceValue = sprite;

            Vector2 uiSize;
            if (sprite != null && sprite.pixelsPerUnit > 0f)
            {
                uiSize = new Vector2(
                    (sprite.rect.width / sprite.pixelsPerUnit) * shape.scale.x * scale,
                    (sprite.rect.height / sprite.pixelsPerUnit) * shape.scale.y * scale);
            }
            else
            {
                uiSize = new Vector2(
                    Mathf.Max(1f, shape.scale.x * scale),
                    Mathf.Max(1f, shape.scale.y * scale));
            }

            emission.FindPropertyRelative("shapeSpriteSize").vector2Value = uiSize;

            string spriteName = sprite != null ? sprite.name : "none";
            AddReport(report, "Shape — Sprite",
                $"sprite={spriteName}, size={uiSize.x:F0}x{uiSize.y:F0}",
                sprite != null ? CloneReportWindow.EntryType.Success : CloneReportWindow.EntryType.Warning);
        }

        private static void ApplyColorOverLifetime(ParticleSystem source, Core.UIParticleSystem target, List<CloneReportWindow.ReportEntry> report)
        {
            var col = source.colorOverLifetime;
            if (!col.enabled) return;

            var so = new SerializedObject(target);
            var colorSettings = so.FindProperty("colorSettings");

            Gradient gradient = ExtractGradient(col.color);
            if (gradient != null)
            {
                colorSettings.FindPropertyRelative("useGradientColor").boolValue = true;
                SetSerializedGradient(colorSettings.FindPropertyRelative("colorGradient"), gradient);

                var alphaKeys = gradient.alphaKeys;
                if (alphaKeys.Length > 0 && alphaKeys[alphaKeys.Length - 1].alpha < 0.1f)
                {
                    colorSettings.FindPropertyRelative("fadeOut").boolValue = true;
                }

                AddReport(report, "Color Over Lifetime", $"gradient with {gradient.colorKeys.Length} color keys", CloneReportWindow.EntryType.Success);
            }

            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ApplySizeOverLifetime(ParticleSystem source, Core.UIParticleSystem target, List<CloneReportWindow.ReportEntry> report)
        {
            var sol = source.sizeOverLifetime;
            if (!sol.enabled) return;

            var so = new SerializedObject(target);
            var sizeSettings = so.FindProperty("sizeSettings");

            sizeSettings.FindPropertyRelative("useSizeOverLifetime").boolValue = true;

            if (sol.separateAxes)
            {
                sizeSettings.FindPropertyRelative("use2DSizeOverLifetime").boolValue = true;
                sizeSettings.FindPropertyRelative("use2DSize").boolValue = true;

                AnimationCurve curveX = ExtractCurve(sol.x);
                AnimationCurve curveY = ExtractCurve(sol.y);

                if (curveX != null)
                    sizeSettings.FindPropertyRelative("sizeXOverLifetimeCurve").animationCurveValue = curveX;
                if (curveY != null)
                    sizeSettings.FindPropertyRelative("sizeYOverLifetimeCurve").animationCurveValue = curveY;

                AddReport(report, "Size Over Lifetime", "separate axes X, Y", CloneReportWindow.EntryType.Success);
            }
            else
            {
                AnimationCurve curve = ExtractCurve(sol.size);
                if (curve != null)
                    sizeSettings.FindPropertyRelative("sizeOverLifetimeCurve").animationCurveValue = curve;

                AddReport(report, "Size Over Lifetime", "uniform", CloneReportWindow.EntryType.Success);
            }

            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ApplyRotationOverLifetime(ParticleSystem source, Core.UIParticleSystem target, List<CloneReportWindow.ReportEntry> report)
        {
            var rol = source.rotationOverLifetime;
            if (!rol.enabled) return;

            var so = new SerializedObject(target);
            var rotSettings = so.FindProperty("rotationSettings");

            rotSettings.FindPropertyRelative("useRotationOverLifetime").boolValue = true;

            float speed = ReadMinMaxCurveConstant(rol.z) * Mathf.Rad2Deg;

            if (rol.z.mode == ParticleSystemCurveMode.TwoConstants)
            {
                rotSettings.FindPropertyRelative("useRandomRotationSpeed").boolValue = true;
                rotSettings.FindPropertyRelative("minRotationSpeed").floatValue = rol.z.constantMin * Mathf.Rad2Deg;
                rotSettings.FindPropertyRelative("maxRotationSpeed").floatValue = rol.z.constantMax * Mathf.Rad2Deg;
                AddReport(report, "Rotation Over Lifetime", $"random {rol.z.constantMin * Mathf.Rad2Deg:F0}° — {rol.z.constantMax * Mathf.Rad2Deg:F0}°/s", CloneReportWindow.EntryType.Success);
            }
            else
            {
                rotSettings.FindPropertyRelative("rotationSpeed").floatValue = speed;
                AddReport(report, "Rotation Over Lifetime", $"{speed:F0}°/s", CloneReportWindow.EntryType.Success);
            }

            so.ApplyModifiedPropertiesWithoutUndo();
        }


        private static void ApplyNoiseModule(ParticleSystem source, Core.UIParticleSystem target, float scale, List<CloneReportWindow.ReportEntry> report)
        {
            var noise = source.noise;
            if (!noise.enabled) return;

            var so = new SerializedObject(target);
            var noiseSettings = so.FindProperty("noiseSettings");

            noiseSettings.FindPropertyRelative("useNoise").boolValue = true;

            if (noise.separateAxes)
            {
                noiseSettings.FindPropertyRelative("separateAxisNoise").boolValue = true;
                noiseSettings.FindPropertyRelative("noiseStrengthX").floatValue = ReadMinMaxCurveConstant(noise.strengthX) * scale;
                noiseSettings.FindPropertyRelative("noiseStrengthY").floatValue = ReadMinMaxCurveConstant(noise.strengthY) * scale;
                noiseSettings.FindPropertyRelative("noiseFrequencyX").floatValue = noise.frequency;
                noiseSettings.FindPropertyRelative("noiseFrequencyY").floatValue = noise.frequency;
            }
            else
            {
                noiseSettings.FindPropertyRelative("noiseStrength").floatValue = ReadMinMaxCurveConstant(noise.strength) * scale;
                noiseSettings.FindPropertyRelative("noiseFrequency").floatValue = noise.frequency;
            }

            float scrollSpeed = ReadMinMaxCurveConstant(noise.scrollSpeed);
            noiseSettings.FindPropertyRelative("noiseSpeed").floatValue = scrollSpeed;
            noiseSettings.FindPropertyRelative("animateNoise").boolValue = scrollSpeed > 0.01f;

            so.ApplyModifiedPropertiesWithoutUndo();
            AddReport(report, "Noise", $"strength={ReadMinMaxCurveConstant(noise.strength) * scale:F1}, freq={noise.frequency:F2}", CloneReportWindow.EntryType.Success);
        }

        private static void ApplyVelocityOverLifetime(ParticleSystem source, Core.UIParticleSystem target, float scale, List<CloneReportWindow.ReportEntry> report)
        {
            var vol = source.velocityOverLifetime;
            if (!vol.enabled) return;

            var so = new SerializedObject(target);
            var velSettings = so.FindProperty("velocityOverLifetimeSettings");

            velSettings.FindPropertyRelative("useVelocityOverLifetime").boolValue = true;

            bool isLocal = vol.space == ParticleSystemSimulationSpace.Local;
            velSettings.FindPropertyRelative("useLocalSpace").boolValue = isLocal;

            float linX = ReadMinMaxCurveConstant(vol.x) * scale;
            float linY = ReadMinMaxCurveConstant(vol.y) * scale;
            bool hasLinear = Mathf.Abs(linX) > 0.01f || Mathf.Abs(linY) > 0.01f;

            if (hasLinear)
            {
                velSettings.FindPropertyRelative("useLinearVelocity").boolValue = true;

                if (vol.x.mode == ParticleSystemCurveMode.TwoConstants || vol.y.mode == ParticleSystemCurveMode.TwoConstants)
                {
                    velSettings.FindPropertyRelative("useRandomLinearVelocity").boolValue = true;

                    float minX = (vol.x.mode == ParticleSystemCurveMode.TwoConstants ? vol.x.constantMin : vol.x.constant) * scale;
                    float maxX = (vol.x.mode == ParticleSystemCurveMode.TwoConstants ? vol.x.constantMax : vol.x.constant) * scale;
                    float minY = (vol.y.mode == ParticleSystemCurveMode.TwoConstants ? vol.y.constantMin : vol.y.constant) * scale;
                    float maxY = (vol.y.mode == ParticleSystemCurveMode.TwoConstants ? vol.y.constantMax : vol.y.constant) * scale;

                    velSettings.FindPropertyRelative("linearVelocityMin").vector2Value = new Vector2(minX, minY);
                    velSettings.FindPropertyRelative("linearVelocityMax").vector2Value = new Vector2(maxX, maxY);

                    AddReport(report, "Velocity Over Lifetime — Linear", $"Random ({minX:F1},{minY:F1}) — ({maxX:F1},{maxY:F1})", CloneReportWindow.EntryType.Success);
                }

                else if (vol.x.mode == ParticleSystemCurveMode.Curve || vol.y.mode == ParticleSystemCurveMode.Curve)
                {
                    velSettings.FindPropertyRelative("useLinearVelocityCurves").boolValue = true;

                    AnimationCurve curveX = ExtractCurve(vol.x);
                    AnimationCurve curveY = ExtractCurve(vol.y);

                    if (curveX != null)
                        velSettings.FindPropertyRelative("linearVelocityX").animationCurveValue = ScaleCurve(curveX, scale);
                    if (curveY != null)
                        velSettings.FindPropertyRelative("linearVelocityY").animationCurveValue = ScaleCurve(curveY, scale);

                    AddReport(report, "Velocity Over Lifetime — Linear", "Curves (X, Y)", CloneReportWindow.EntryType.Success);
                }

                else
                {
                    velSettings.FindPropertyRelative("linearVelocity").vector2Value = new Vector2(linX, linY);
                    AddReport(report, "Velocity Over Lifetime — Linear", $"({linX:F1}, {linY:F1})", CloneReportWindow.EntryType.Success);
                }
            }

            float orbZ = ReadMinMaxCurveConstant(vol.orbitalZ) * scale;
            bool hasOrbital = Mathf.Abs(orbZ) > 0.01f;

            if (hasOrbital)
            {
                velSettings.FindPropertyRelative("useOrbitalVelocity").boolValue = true;

                Vector2 orbOffset = new Vector2(
                    ReadMinMaxCurveConstant(vol.orbitalOffsetX) * scale,
                    ReadMinMaxCurveConstant(vol.orbitalOffsetY) * scale
                );
                velSettings.FindPropertyRelative("orbitalCenter").vector2Value = orbOffset;

                if (vol.orbitalZ.mode == ParticleSystemCurveMode.TwoConstants)
                {
                    velSettings.FindPropertyRelative("useRandomOrbitalVelocity").boolValue = true;
                    velSettings.FindPropertyRelative("orbitalVelocityMin").floatValue = vol.orbitalZ.constantMin * scale;
                    velSettings.FindPropertyRelative("orbitalVelocityMax").floatValue = vol.orbitalZ.constantMax * scale;
                    AddReport(report, "Velocity Over Lifetime — Orbital", $"Random ({vol.orbitalZ.constantMin * scale:F1} — {vol.orbitalZ.constantMax * scale:F1})", CloneReportWindow.EntryType.Success);
                }

                else if (vol.orbitalZ.mode == ParticleSystemCurveMode.Curve)
                {
                    velSettings.FindPropertyRelative("useOrbitalVelocityCurve").boolValue = true;

                    AnimationCurve orbCurve = ExtractCurve(vol.orbitalZ);
                    if (orbCurve != null)
                        velSettings.FindPropertyRelative("orbitalVelocityCurve").animationCurveValue = ScaleCurve(orbCurve, scale);

                    velSettings.FindPropertyRelative("orbitalVelocity").floatValue = 1f;
                    AddReport(report, "Velocity Over Lifetime — Orbital", "Curve (Z)", CloneReportWindow.EntryType.Success);
                }

                else
                {
                    velSettings.FindPropertyRelative("orbitalVelocity").floatValue = orbZ;
                    AddReport(report, "Velocity Over Lifetime — Orbital", $"Z={orbZ:F1}", CloneReportWindow.EntryType.Success);
                }
            }

            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ApplyTrailsModule(ParticleSystem source, Core.UIParticleSystem target, List<CloneReportWindow.ReportEntry> report)
        {
            var trails = source.trails;
            if (!trails.enabled) return;

            var so = new SerializedObject(target);
            var trailSettings = so.FindProperty("trailSettings");

            trailSettings.FindPropertyRelative("useTrails").boolValue = true;
            trailSettings.FindPropertyRelative("trailLifetime").floatValue = Mathf.Clamp(ReadMinMaxCurveConstant(trails.lifetime), 0.1f, 10f);

            AnimationCurve widthCurve = ExtractCurve(trails.widthOverTrail);
            if (widthCurve != null)
                trailSettings.FindPropertyRelative("trailWidthCurve").animationCurveValue = widthCurve;

            Gradient trailColorGrad = ExtractGradient(trails.colorOverLifetime);
            if (trailColorGrad != null && trailColorGrad.colorKeys.Length > 0)
            {
                trailSettings.FindPropertyRelative("trailColorMode").enumValueIndex = 2; 
                                                                                        
                var gradProp = trailSettings.FindPropertyRelative("trailGradient");
                var gradInfo = typeof(SerializedProperty).GetProperty("gradientValue",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                gradInfo?.SetValue(gradProp, trailColorGrad);
            }
            else
            {
                trailSettings.FindPropertyRelative("trailColorMode").enumValueIndex = 4;
            }

            so.ApplyModifiedPropertiesWithoutUndo();
            AddReport(report, "Trails", $"lifetime={ReadMinMaxCurveConstant(trails.lifetime):F2}", CloneReportWindow.EntryType.Success);
        }

        private static void ApplyTextureSheetAnimation(ParticleSystem source, Core.UIParticleSystem target, List<CloneReportWindow.ReportEntry> report)
        {
            var tsa = source.textureSheetAnimation;
            if (!tsa.enabled) return;

            var renderer = source.GetComponent<ParticleSystemRenderer>();
            if (renderer == null || renderer.sharedMaterial == null || renderer.sharedMaterial.mainTexture == null) return;

            Texture2D tex = renderer.sharedMaterial.mainTexture as Texture2D;
            if (tex == null) return;

            var so = new SerializedObject(target);
            var tsSettings = so.FindProperty("textureSheetSettings");

            tsSettings.FindPropertyRelative("useTextureSheetAnimation").boolValue = true;
            tsSettings.FindPropertyRelative("textureSheet").objectReferenceValue = tex;
            tsSettings.FindPropertyRelative("tilesX").intValue = tsa.numTilesX;
            tsSettings.FindPropertyRelative("tilesY").intValue = tsa.numTilesY;
            tsSettings.FindPropertyRelative("animationFrames").intValue = tsa.numTilesX * tsa.numTilesY;

            if (tsa.cycleCount > 0)
            {
                tsSettings.FindPropertyRelative("animationSpeed").floatValue = Mathf.Max(1f, tsa.cycleCount * 10f);
            }

            tsSettings.FindPropertyRelative("loopAnimation").boolValue = tsa.cycleCount != 1;

            so.ApplyModifiedPropertiesWithoutUndo();
            AddReport(report, "Texture Sheet Animation", $"{tsa.numTilesX}x{tsa.numTilesY}", CloneReportWindow.EntryType.Success);
        }

        private static void CloneChildParticleSystems(Transform sourceParent, Transform targetParent,
          Canvas canvas, float scale, Core.UIParticleSystem rootUIPS, List<CloneReportWindow.ReportEntry> report)
        {
            for (int i = 0; i < sourceParent.childCount; i++)
            {
                Transform child = sourceParent.GetChild(i);
                var childPS = child.GetComponent<ParticleSystem>();

                if (childPS != null)
                {
                    GameObject childGo = new GameObject($"{child.name} (UI Clone)");
                    childGo.AddComponent<RectTransform>();
                    childGo.transform.SetParent(targetParent, false);

                    var childRect = childGo.GetComponent<RectTransform>();
                    childRect.anchoredPosition = new Vector2(child.localPosition.x * scale, child.localPosition.y * scale);
                    childRect.sizeDelta = new Vector2(200, 200);

                    var childUIPS = childGo.AddComponent<Core.UIParticleSystem>();

                    ApplyMainModule(childPS, childUIPS, scale, report);
                    ApplyEmissionModule(childPS, childUIPS, report);
                    ApplyShapeModule(childPS, childUIPS, scale, report);
                    ApplyColorOverLifetime(childPS, childUIPS, report);
                    ApplyColorBySpeed(childPS, childUIPS, scale, report);
                    ApplySizeOverLifetime(childPS, childUIPS, report);
                    ApplySizeBySpeed(childPS, childUIPS, scale, report);
                    ApplyRotationOverLifetime(childPS, childUIPS, report);
                    ApplyRotationBySpeed(childPS, childUIPS, scale, report);
                    ApplyNoiseModule(childPS, childUIPS, scale, report);
                    ApplyVelocityOverLifetime(childPS, childUIPS, scale, report);
                    ApplyLimitVelocityOverLifetime(childPS, childUIPS, scale, report);
                    ApplyInheritVelocity(childPS, childUIPS, scale, report);
                    ApplyForceOverLifetime(childPS, childUIPS, scale, report);
                    ApplyCollisionModule(childPS, childUIPS, scale, report);
                    ApplyTrailsModule(childPS, childUIPS, report);
                    ApplyTextureSheetAnimation(childPS, childUIPS, report);

                    bool childSpriteSet = ApplyRendererModule(childPS, childUIPS, scale, report);
                    if (!childSpriteSet)
                    {
                        childUIPS.LoadDefaultSprite();
                    }

                    var childSO = new SerializedObject(childUIPS);
                    childSO.FindProperty("emissionSettings").FindPropertyRelative("emitOnStart").boolValue = false;
                    childSO.ApplyModifiedPropertiesWithoutUndo();

                    TryLinkSubEmitter(rootUIPS, childUIPS, sourceParent.GetComponent<ParticleSystem>(), childPS, report);

                    CloneChildParticleSystems(child, childGo.transform, canvas, scale, rootUIPS, report);

                    AddReport(report, $"Child — {child.name}", "cloned as sub UI Particle System", CloneReportWindow.EntryType.Success);
                }
                else
                {
                    GameObject childGo = new GameObject(child.name);
                    childGo.AddComponent<RectTransform>();
                    childGo.transform.SetParent(targetParent, false);

                    var childRect = childGo.GetComponent<RectTransform>();
                    childRect.anchoredPosition = new Vector2(child.localPosition.x * scale, child.localPosition.y * scale);

                    CloneChildParticleSystems(child, childGo.transform, canvas, scale, rootUIPS, report);
                }
            }
        }

        private static void TryLinkSubEmitter(Core.UIParticleSystem root, Core.UIParticleSystem child,
            ParticleSystem sourceRoot, ParticleSystem sourceChild, List<CloneReportWindow.ReportEntry> report)
        {
            var subEmitters = sourceRoot.subEmitters;
            if (!subEmitters.enabled) return;

            int subCount = subEmitters.subEmittersCount;
            for (int i = 0; i < subCount; i++)
            {
                var subPS = subEmitters.GetSubEmitterSystem(i);
                if (subPS == sourceChild)
                {
                    var triggerType = subEmitters.GetSubEmitterType(i);

                    var rootSO = new SerializedObject(root);
                    var subSettings = rootSO.FindProperty("subEmitterSettings");
                    subSettings.FindPropertyRelative("enableSubEmitters").boolValue = true;

                    var entries = subSettings.FindPropertyRelative("subEmitters");
                    entries.arraySize++;
                    var entry = entries.GetArrayElementAtIndex(entries.arraySize - 1);

                    int uiTriggerType = 0;
                    switch (triggerType)
                    {
                        case ParticleSystemSubEmitterType.Birth:
                            uiTriggerType = 0;
                            break;
                        case ParticleSystemSubEmitterType.Death:
                            uiTriggerType = 1;
                            break;
                        default:
                            uiTriggerType = 0;
                            break;
                    }

                    entry.FindPropertyRelative("triggerType").enumValueIndex = uiTriggerType;
                    entry.FindPropertyRelative("actionType").enumValueIndex = 0;
                    entry.FindPropertyRelative("targetSystem").objectReferenceValue = child;
                    entry.FindPropertyRelative("probability").floatValue = 1f;
                    entry.FindPropertyRelative("inheritPosition").boolValue = true;

                    rootSO.ApplyModifiedPropertiesWithoutUndo();
                    AddReport(report, $"Sub Emitter — {sourceChild.name}", $"trigger={triggerType}", CloneReportWindow.EntryType.Success);
                    return;
                }
            }
        }

        private static void CollectSkippedModules(ParticleSystem source, List<CloneReportWindow.ReportEntry> report)
        {
            if (source.externalForces.enabled)
                AddReport(report, "External Forces", "Not supported in UI — skipped", CloneReportWindow.EntryType.Skipped);
            if (source.customData.enabled)
                AddReport(report, "Custom Data", "Not supported in UI — skipped", CloneReportWindow.EntryType.Skipped);
        }

        private static float ReadMinMaxCurveConstant(ParticleSystem.MinMaxCurve curve)
        {
            switch (curve.mode)
            {
                case ParticleSystemCurveMode.Constant:
                    return curve.constant;
                case ParticleSystemCurveMode.TwoConstants:
                    return (curve.constantMin + curve.constantMax) * 0.5f;
                case ParticleSystemCurveMode.Curve:
                    return curve.curveMultiplier * (curve.curve != null ? curve.curve.Evaluate(0f) : 1f);
                case ParticleSystemCurveMode.TwoCurves:
                    return curve.curveMultiplier;
                default:
                    return 0f;
            }
        }

        private static AnimationCurve ExtractCurve(ParticleSystem.MinMaxCurve minMaxCurve)
        {
            switch (minMaxCurve.mode)
            {
                case ParticleSystemCurveMode.Curve:
                    if (minMaxCurve.curve != null)
                        return ScaleCurve(minMaxCurve.curve, minMaxCurve.curveMultiplier);
                    break;

                case ParticleSystemCurveMode.TwoCurves:
                    if (minMaxCurve.curveMax != null)
                        return ScaleCurve(minMaxCurve.curveMax, minMaxCurve.curveMultiplier);
                    break;

                case ParticleSystemCurveMode.Constant:
                    return AnimationCurve.Linear(0f, minMaxCurve.constant, 1f, minMaxCurve.constant);

                case ParticleSystemCurveMode.TwoConstants:
                    float avg = (minMaxCurve.constantMin + minMaxCurve.constantMax) * 0.5f;
                    return AnimationCurve.Linear(0f, avg, 1f, avg);
            }

            return null;
        }

        private static AnimationCurve ScaleCurve(AnimationCurve source, float multiplier)
        {
            if (Mathf.Abs(multiplier - 1f) < 0.001f)
                return new AnimationCurve(source.keys);

            var keys = source.keys;
            var scaled = new Keyframe[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                scaled[i] = new Keyframe(keys[i].time, keys[i].value * multiplier,
                    keys[i].inTangent * multiplier, keys[i].outTangent * multiplier);
            }
            return new AnimationCurve(scaled);
        }

        private static Gradient ExtractGradient(ParticleSystem.MinMaxGradient minMaxGradient)
        {
            switch (minMaxGradient.mode)
            {
                case ParticleSystemGradientMode.Gradient:
                    return minMaxGradient.gradient;

                case ParticleSystemGradientMode.TwoGradients:
                    return minMaxGradient.gradientMax;

                case ParticleSystemGradientMode.Color:
                    var g = new Gradient();
                    g.SetKeys(
                        new[] { new GradientColorKey(minMaxGradient.color, 0f), new GradientColorKey(minMaxGradient.color, 1f) },
                        new[] { new GradientAlphaKey(minMaxGradient.color.a, 0f), new GradientAlphaKey(minMaxGradient.color.a, 1f) }
                    );
                    return g;

                case ParticleSystemGradientMode.TwoColors:
                    var g2 = new Gradient();
                    g2.SetKeys(
                        new[] { new GradientColorKey(minMaxGradient.colorMin, 0f), new GradientColorKey(minMaxGradient.colorMax, 1f) },
                        new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
                    );
                    return g2;
            }

            return null;
        }

        private static void SetSerializedGradient(SerializedProperty property, Gradient gradient)
        {
            var bindingFlags = System.Reflection.BindingFlags.Public |
                               System.Reflection.BindingFlags.NonPublic |
                               System.Reflection.BindingFlags.Instance;

            var propertyInfo = typeof(SerializedProperty).GetProperty("gradientValue", bindingFlags);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(property, gradient);
            }
            else
            {
                Debug.LogWarning($"{LOG_PREFIX} Could not set gradient via reflection. Falling back to color extraction.");
            }
        }

        private static void ApplyStartSize(ParticleSystem.MainModule main, SerializedObject so, float scale)
        {
            var sizeSettings = so.FindProperty("sizeSettings");

            if (main.startSize3D)
            {
                sizeSettings.FindPropertyRelative("use2DSize").boolValue = true;

                float sizeX = ReadMinMaxCurveConstant(main.startSizeX) * scale;
                float sizeY = ReadMinMaxCurveConstant(main.startSizeY) * scale;

                if (main.startSizeX.mode == ParticleSystemCurveMode.TwoConstants)
                {
                    sizeSettings.FindPropertyRelative("use2DSizeRange").boolValue = true;
                    sizeSettings.FindPropertyRelative("minSize2D").vector2Value =
                        new Vector2(main.startSizeX.constantMin * scale, main.startSizeY.constantMin * scale);
                    sizeSettings.FindPropertyRelative("maxSize2D").vector2Value =
                        new Vector2(main.startSizeX.constantMax * scale, main.startSizeY.constantMax * scale);
                }
                else
                {
                    sizeSettings.FindPropertyRelative("size2D").vector2Value = new Vector2(sizeX, sizeY);
                }
            }
            else
            {
                if (main.startSize.mode == ParticleSystemCurveMode.TwoConstants)
                {
                    sizeSettings.FindPropertyRelative("useSizeRange").boolValue = true;
                    sizeSettings.FindPropertyRelative("minSize").floatValue = main.startSize.constantMin * scale;
                    sizeSettings.FindPropertyRelative("maxSize").floatValue = main.startSize.constantMax * scale;
                }
                else
                {
                    sizeSettings.FindPropertyRelative("size").floatValue = ReadMinMaxCurveConstant(main.startSize) * scale;
                }
            }
        }

        private static void ApplyStartColor(ParticleSystem.MinMaxGradient startColor, SerializedObject so)
        {
            var colorSettings = so.FindProperty("colorSettings");

            switch (startColor.mode)
            {
                case ParticleSystemGradientMode.Color:
                    colorSettings.FindPropertyRelative("particleColor").colorValue = startColor.color;
                    break;

                case ParticleSystemGradientMode.TwoColors:
                    colorSettings.FindPropertyRelative("useRandomColors").boolValue = true;
                    colorSettings.FindPropertyRelative("particleColor").colorValue = startColor.colorMin;
                    colorSettings.FindPropertyRelative("particleColorEnd").colorValue = startColor.colorMax;
                    break;

                case ParticleSystemGradientMode.Gradient:
                    colorSettings.FindPropertyRelative("useGradientColor").boolValue = true;
                    SetSerializedGradient(colorSettings.FindPropertyRelative("colorGradient"), startColor.gradient);
                    break;

                case ParticleSystemGradientMode.TwoGradients:
                    colorSettings.FindPropertyRelative("useGradientColor").boolValue = true;
                    colorSettings.FindPropertyRelative("useRandomColors").boolValue = true;
                    SetSerializedGradient(colorSettings.FindPropertyRelative("colorGradient"), startColor.gradientMin);
                    SetSerializedGradient(colorSettings.FindPropertyRelative("colorGradient2"), startColor.gradientMax);
                    break;
            }
        }

        private static void ApplyStartRotation(ParticleSystem.MainModule main, SerializedObject so)
        {
            var rotSettings = so.FindProperty("rotationSettings");

            ParticleSystem.MinMaxCurve rotCurve = main.startRotation3D ? main.startRotationZ : main.startRotation;

            if (rotCurve.mode == ParticleSystemCurveMode.TwoConstants)
            {
                rotSettings.FindPropertyRelative("useRandomRotation").boolValue = true;
                rotSettings.FindPropertyRelative("minRotation").floatValue = rotCurve.constantMin * Mathf.Rad2Deg;
                rotSettings.FindPropertyRelative("maxRotation").floatValue = rotCurve.constantMax * Mathf.Rad2Deg;
            }
            else
            {
                float deg = ReadMinMaxCurveConstant(rotCurve) * Mathf.Rad2Deg;
                if (Mathf.Abs(deg) > 0.01f)
                {
                    rotSettings.FindPropertyRelative("initialRotation").floatValue = deg;
                }
            }
        }

        private static void ApplyMinMaxCurveToSpeed(ParticleSystem.MinMaxCurve curve, SerializedObject so, float scale)
        {
            if (curve.mode == ParticleSystemCurveMode.TwoConstants)
            {
                so.FindProperty("useRandomSpeed").boolValue = true;
                so.FindProperty("minSpeed").floatValue = Mathf.Max(curve.constantMin * scale, 0.01f);
                so.FindProperty("maxSpeed").floatValue = Mathf.Max(curve.constantMax * scale, 0.01f);
            }
            else
            {
                so.FindProperty("speed").floatValue = Mathf.Max(ReadMinMaxCurveConstant(curve) * scale, 0.01f);
            }
        }

        private static void ApplyMinMaxCurveToRange(ParticleSystem.MinMaxCurve curve, SerializedProperty parent,
            string constantField, string useRandomField, string minField, string maxField)
        {
            if (curve.mode == ParticleSystemCurveMode.TwoConstants)
            {
                parent.FindPropertyRelative(useRandomField).boolValue = true;
                parent.FindPropertyRelative(minField).floatValue = curve.constantMin;
                parent.FindPropertyRelative(maxField).floatValue = curve.constantMax;
            }
            else
            {
                parent.FindPropertyRelative(constantField).floatValue = ReadMinMaxCurveConstant(curve);
            }
        }

        private static void ApplyColorBySpeed(ParticleSystem source, Core.UIParticleSystem target, float scale, List<CloneReportWindow.ReportEntry> report)
        {
            var cbs = source.colorBySpeed;
            if (!cbs.enabled) return;

            var so = new SerializedObject(target);
            var colorBySpeedSettings = so.FindProperty("colorBySpeedSettings");
            if (colorBySpeedSettings == null) return;

            colorBySpeedSettings.FindPropertyRelative("useColorBySpeed").boolValue = true;

            colorBySpeedSettings.FindPropertyRelative("minSpeed").floatValue = cbs.range.x * scale;
            colorBySpeedSettings.FindPropertyRelative("maxSpeed").floatValue = cbs.range.y * scale;

            switch (cbs.color.mode)
            {
                case ParticleSystemGradientMode.Gradient:
                    colorBySpeedSettings.FindPropertyRelative("useRandomColors").boolValue = false;
                    SetSerializedGradient(colorBySpeedSettings.FindPropertyRelative("colorGradient"), cbs.color.gradient);
                    break;

                case ParticleSystemGradientMode.TwoGradients:
                    colorBySpeedSettings.FindPropertyRelative("useRandomColors").boolValue = true;
                    SetSerializedGradient(colorBySpeedSettings.FindPropertyRelative("colorGradient"), cbs.color.gradientMin);
                    SetSerializedGradient(colorBySpeedSettings.FindPropertyRelative("colorGradient2"), cbs.color.gradientMax);
                    break;

                case ParticleSystemGradientMode.Color:
                    colorBySpeedSettings.FindPropertyRelative("useRandomColors").boolValue = false;
                    var g = new Gradient();
                    g.SetKeys(
                        new[] { new GradientColorKey(cbs.color.color, 0f), new GradientColorKey(cbs.color.color, 1f) },
                        new[] { new GradientAlphaKey(cbs.color.color.a, 0f), new GradientAlphaKey(cbs.color.color.a, 1f) }
                    );
                    SetSerializedGradient(colorBySpeedSettings.FindPropertyRelative("colorGradient"), g);
                    break;

                case ParticleSystemGradientMode.TwoColors:
                    colorBySpeedSettings.FindPropertyRelative("useRandomColors").boolValue = true;
                    var gMin = new Gradient();
                    gMin.SetKeys(
                        new[] { new GradientColorKey(cbs.color.colorMin, 0f), new GradientColorKey(cbs.color.colorMin, 1f) },
                        new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
                    );
                    var gMax = new Gradient();
                    gMax.SetKeys(
                        new[] { new GradientColorKey(cbs.color.colorMax, 0f), new GradientColorKey(cbs.color.colorMax, 1f) },
                        new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
                    );
                    SetSerializedGradient(colorBySpeedSettings.FindPropertyRelative("colorGradient"), gMin);
                    SetSerializedGradient(colorBySpeedSettings.FindPropertyRelative("colorGradient2"), gMax);
                    break;
            }

            so.ApplyModifiedPropertiesWithoutUndo();
            AddReport(report, "Color By Speed", $"range={cbs.range.x * scale:F1} — {cbs.range.y * scale:F1}", CloneReportWindow.EntryType.Success);
        }

        private static void ApplySizeBySpeed(ParticleSystem source, Core.UIParticleSystem target, float scale, List<CloneReportWindow.ReportEntry> report)
        {
            var sbs = source.sizeBySpeed;
            if (!sbs.enabled) return;

            var so = new SerializedObject(target);
            var sizeBySpeedSettings = so.FindProperty("sizeBySpeedSettings");
            if (sizeBySpeedSettings == null) return;

            sizeBySpeedSettings.FindPropertyRelative("useSizeBySpeed").boolValue = true;
            sizeBySpeedSettings.FindPropertyRelative("minSpeed").floatValue = sbs.range.x * scale;
            sizeBySpeedSettings.FindPropertyRelative("maxSpeed").floatValue = sbs.range.y * scale;

            if (sbs.separateAxes)
            {
                sizeBySpeedSettings.FindPropertyRelative("separateAxes").boolValue = true;

                AnimationCurve curveX = ExtractRawCurve(sbs.x);
                AnimationCurve curveY = ExtractRawCurve(sbs.y);

                if (curveX != null)
                    sizeBySpeedSettings.FindPropertyRelative("sizeXCurve").animationCurveValue = curveX;
                if (curveY != null)
                    sizeBySpeedSettings.FindPropertyRelative("sizeYCurve").animationCurveValue = curveY;

                sizeBySpeedSettings.FindPropertyRelative("size2DMultiplier").vector2Value = new Vector2(sbs.x.curveMultiplier, sbs.y.curveMultiplier);

                AddReport(report, "Size By Speed", $"separate axes X, Y, range={sbs.range.x * scale:F1} — {sbs.range.y * scale:F1}", CloneReportWindow.EntryType.Success);
            }
            else
            {
                sizeBySpeedSettings.FindPropertyRelative("separateAxes").boolValue = false;

                AnimationCurve curve = ExtractRawCurve(sbs.size);
                if (curve != null)
                    sizeBySpeedSettings.FindPropertyRelative("sizeCurve").animationCurveValue = curve;

                sizeBySpeedSettings.FindPropertyRelative("sizeMultiplier").floatValue = sbs.size.curveMultiplier;

                AddReport(report, "Size By Speed", $"uniform, range={sbs.range.x * scale:F1} — {sbs.range.y * scale:F1}", CloneReportWindow.EntryType.Success);
            }

            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ApplyLimitVelocityOverLifetime(ParticleSystem source, Core.UIParticleSystem target, float scale, List<CloneReportWindow.ReportEntry> report)
        {
            var lvol = source.limitVelocityOverLifetime;
            if (!lvol.enabled) return;

            var so = new SerializedObject(target);
            var lvs = so.FindProperty("limitVelocityOverLifetimeSettings");
            if (lvs == null) return;

            lvs.FindPropertyRelative("useLimitVelocityOverLifetime").boolValue = true;
            lvs.FindPropertyRelative("dampen").floatValue = lvol.dampen;
            lvs.FindPropertyRelative("multiplyDragBySize").boolValue = lvol.multiplyDragByParticleSize;
            lvs.FindPropertyRelative("multiplyDragByVelocity").boolValue = lvol.multiplyDragByParticleVelocity;

            if (lvol.separateAxes)
            {
                lvs.FindPropertyRelative("separateAxes").boolValue = true;
                MapMinMaxCurveToLimit(lvol.limitX, lvs, "limitXMode", "limitX", "limitXCurve", "limitXMin", "limitXMax", "limitXMinCurve", scale);
                MapMinMaxCurveToLimit(lvol.limitY, lvs, "limitYMode", "limitY", "limitYCurve", "limitYMin", "limitYMax", "limitYMinCurve", scale);
                MapMinMaxCurveToLimit(lvol.limitZ, lvs, "limitZMode", "limitZ", "limitZCurve", "limitZMin", "limitZMax", "limitZMinCurve", scale);
            }
            else
            {
                lvs.FindPropertyRelative("separateAxes").boolValue = false;
                MapMinMaxCurveToLimit(lvol.limit, lvs, "limitMode", "limit", "limitCurve", "limitMin", "limitMax", "limitMinCurve", scale);
            }

            MapMinMaxCurveToDrag(lvol.drag, lvs, "dragMode", "drag", "dragCurve", "dragMin", "dragMax", "dragMinCurve");

            so.ApplyModifiedPropertiesWithoutUndo();
            AddReport(report, "Limit Velocity Over Lifetime", "Fully cloned with custom dampening & drag settings", CloneReportWindow.EntryType.Success);
        }

        private static void ApplyInheritVelocity(ParticleSystem source, Core.UIParticleSystem target, float scale, List<CloneReportWindow.ReportEntry> report)
        {
            var iv = source.inheritVelocity;
            if (!iv.enabled) return;

            var so = new SerializedObject(target);
            var settings = so.FindProperty("inheritVelocitySettings");
            if (settings == null) return;

            settings.FindPropertyRelative("useInheritVelocity").boolValue = true;
            settings.FindPropertyRelative("mode").enumValueIndex = iv.mode == ParticleSystemInheritVelocityMode.Current ? 1 : 0;
            settings.FindPropertyRelative("multiplier").floatValue = ReadMinMaxCurveConstant(iv.curve);

            so.ApplyModifiedPropertiesWithoutUndo();
            AddReport(report, "Inherit Velocity",
                $"{iv.mode}, multiplier={ReadMinMaxCurveConstant(iv.curve):F2}",
                CloneReportWindow.EntryType.Success);
        }

        private static void ApplyRotationBySpeed(ParticleSystem source, Core.UIParticleSystem target, float scale, List<CloneReportWindow.ReportEntry> report)
        {
            var rbs = source.rotationBySpeed;
            if (!rbs.enabled) return;

            var so = new SerializedObject(target);
            var rotSettings = so.FindProperty("rotationSettings");
            if (rotSettings == null) return;

            rotSettings.FindPropertyRelative("useRotationBySpeed").boolValue = true;
            rotSettings.FindPropertyRelative("rotationBySpeed").floatValue = ReadMinMaxCurveConstant(rbs.z) * Mathf.Rad2Deg;
            rotSettings.FindPropertyRelative("rotationBySpeedMin").floatValue = rbs.range.x * scale;
            rotSettings.FindPropertyRelative("rotationBySpeedMax").floatValue = Mathf.Max(rbs.range.x * scale + 0.1f, rbs.range.y * scale);

            so.ApplyModifiedPropertiesWithoutUndo();
            AddReport(report, "Rotation By Speed",
                $"{ReadMinMaxCurveConstant(rbs.z) * Mathf.Rad2Deg:F0}°/s over speed {rbs.range.x * scale:F0}–{rbs.range.y * scale:F0}",
                CloneReportWindow.EntryType.Success);
        }

        private static void ApplyForceOverLifetime(ParticleSystem source, Core.UIParticleSystem target, float scale, List<CloneReportWindow.ReportEntry> report)
        {
            var fol = source.forceOverLifetime;
            if (!fol.enabled) return;

            var so = new SerializedObject(target);
            var fvs = so.FindProperty("forceOverLifetimeSettings");
            if (fvs == null) return;

            fvs.FindPropertyRelative("useForceOverLifetime").boolValue = true;
            fvs.FindPropertyRelative("useLocalSpace").boolValue = fol.space == ParticleSystemSimulationSpace.Local;

            var modeProp = fvs.FindPropertyRelative("mode");

            int uiMode = 0;
            switch (fol.x.mode)
            {
                case ParticleSystemCurveMode.Constant:
                    uiMode = 0;
                    fvs.FindPropertyRelative("x").floatValue = fol.x.constant * scale;
                    fvs.FindPropertyRelative("y").floatValue = fol.y.constant * scale;
                    break;

                case ParticleSystemCurveMode.Curve:
                    uiMode = 1;
                    fvs.FindPropertyRelative("xCurve").animationCurveValue = ScaleCurve(fol.x.curve, fol.x.curveMultiplier * scale);
                    fvs.FindPropertyRelative("yCurve").animationCurveValue = ScaleCurve(fol.y.curve, fol.y.curveMultiplier * scale);
                    break;

                case ParticleSystemCurveMode.TwoConstants:
                    uiMode = 2;
                    fvs.FindPropertyRelative("x").floatValue = fol.x.constantMax * scale;
                    fvs.FindPropertyRelative("xMin").floatValue = fol.x.constantMin * scale;
                    fvs.FindPropertyRelative("y").floatValue = fol.y.constantMax * scale;
                    fvs.FindPropertyRelative("yMin").floatValue = fol.y.constantMin * scale;
                    break;

                case ParticleSystemCurveMode.TwoCurves:
                    uiMode = 3;
                    fvs.FindPropertyRelative("xCurve").animationCurveValue = ScaleCurve(fol.x.curveMax, fol.x.curveMultiplier * scale);
                    fvs.FindPropertyRelative("xMinCurve").animationCurveValue = ScaleCurve(fol.x.curveMin, fol.x.curveMultiplier * scale);
                    fvs.FindPropertyRelative("yCurve").animationCurveValue = ScaleCurve(fol.y.curveMax, fol.y.curveMultiplier * scale);
                    fvs.FindPropertyRelative("yMinCurve").animationCurveValue = ScaleCurve(fol.y.curveMin, fol.y.curveMultiplier * scale);
                    break;
            }

            SetIntOrEnum(modeProp, uiMode);

            so.ApplyModifiedPropertiesWithoutUndo();
            AddReport(report, "Force Over Lifetime", "Fully cloned with custom direction and acceleration curves", CloneReportWindow.EntryType.Success);
        }

        private static void ApplyCollisionModule(ParticleSystem source, Core.UIParticleSystem target, float scale, List<CloneReportWindow.ReportEntry> report)
        {
            var col = source.collision;
            if (!col.enabled) return;

            var so = new SerializedObject(target);
            var cs = so.FindProperty("collisionSettings");
            if (cs == null) return;

            cs.FindPropertyRelative("useCollision").boolValue = true;
            cs.FindPropertyRelative("useBounds").boolValue = true;
            cs.FindPropertyRelative("bounce").floatValue = Mathf.Clamp01(ReadMinMaxCurveConstant(col.bounce));
            cs.FindPropertyRelative("dampen").floatValue = Mathf.Clamp01(ReadMinMaxCurveConstant(col.dampen));
            cs.FindPropertyRelative("lifetimeLoss").floatValue = Mathf.Clamp01(ReadMinMaxCurveConstant(col.lifetimeLoss));
            cs.FindPropertyRelative("radiusScale").floatValue = Mathf.Clamp(col.radiusScale, 0f, 2f);
            cs.FindPropertyRelative("minKillSpeed").floatValue = Mathf.Max(0f, col.minKillSpeed * scale);

            so.ApplyModifiedPropertiesWithoutUndo();

            if (col.type == ParticleSystemCollisionType.World)
            {
                AddReport(report, "Collision",
                    $"World colliders not supported in UI — Bounds (own rect) used. bounce={ReadMinMaxCurveConstant(col.bounce):F2}, dampen={ReadMinMaxCurveConstant(col.dampen):F2}",
                    CloneReportWindow.EntryType.Warning);
            }
            else
            {
                AddReport(report, "Collision",
                    $"Planes → Bounds. bounce={ReadMinMaxCurveConstant(col.bounce):F2}, dampen={ReadMinMaxCurveConstant(col.dampen):F2}",
                    CloneReportWindow.EntryType.Success);
            }
        }

        private static void MapMinMaxCurveToLimit(ParticleSystem.MinMaxCurve srcCurve, SerializedProperty parentProp,
            string modeName, string constName, string curveName, string minName, string maxName, string minCurveName, float scale)
        {
            var modeProp = parentProp.FindPropertyRelative(modeName);
            var constProp = parentProp.FindPropertyRelative(constName);
            var curveProp = parentProp.FindPropertyRelative(curveName);
            var minProp = parentProp.FindPropertyRelative(minName);
            var maxProp = parentProp.FindPropertyRelative(maxName);
            var minCurveProp = parentProp.FindPropertyRelative(minCurveName);

            switch (srcCurve.mode)
            {
                case ParticleSystemCurveMode.Constant:
                    SetIntOrEnum(modeProp, 0);
                    if (constProp != null)
                        constProp.floatValue = srcCurve.constant * scale;
                    break;

                case ParticleSystemCurveMode.Curve:
                    SetIntOrEnum(modeProp, 1);
                    if (curveProp != null)
                        curveProp.animationCurveValue = ScaleCurve(srcCurve.curve, srcCurve.curveMultiplier * scale);
                    break;

                case ParticleSystemCurveMode.TwoConstants:
                    SetIntOrEnum(modeProp, 2);
                    if (minProp != null)
                        minProp.floatValue = srcCurve.constantMin * scale;
                    if (maxProp != null)
                        maxProp.floatValue = srcCurve.constantMax * scale;
                    break;

                case ParticleSystemCurveMode.TwoCurves:
                    SetIntOrEnum(modeProp, 3);
                    if (minCurveProp != null)
                        minCurveProp.animationCurveValue = ScaleCurve(srcCurve.curveMin, srcCurve.curveMultiplier * scale);
                    if (curveProp != null)
                        curveProp.animationCurveValue = ScaleCurve(srcCurve.curveMax, srcCurve.curveMultiplier * scale);
                    break;
            }
        }

        private static void MapMinMaxCurveToDrag(ParticleSystem.MinMaxCurve srcCurve, SerializedProperty parentProp,
            string modeName, string constName, string curveName, string minName, string maxName, string minCurveName)
        {
            var modeProp = parentProp.FindPropertyRelative(modeName);
            var constProp = parentProp.FindPropertyRelative(constName);
            var curveProp = parentProp.FindPropertyRelative(curveName);
            var minProp = parentProp.FindPropertyRelative(minName);
            var maxProp = parentProp.FindPropertyRelative(maxName);
            var minCurveProp = parentProp.FindPropertyRelative(minCurveName);

            switch (srcCurve.mode)
            {
                case ParticleSystemCurveMode.Constant:
                    SetIntOrEnum(modeProp, 0);
                    if (constProp != null)
                        constProp.floatValue = srcCurve.constant;
                    break;

                case ParticleSystemCurveMode.Curve:
                    SetIntOrEnum(modeProp, 1);
                    if (curveProp != null)
                        curveProp.animationCurveValue = ScaleCurve(srcCurve.curve, srcCurve.curveMultiplier);
                    break;

                case ParticleSystemCurveMode.TwoConstants:
                    SetIntOrEnum(modeProp, 2);
                    if (minProp != null)
                        minProp.floatValue = srcCurve.constantMin;
                    if (maxProp != null)
                        maxProp.floatValue = srcCurve.constantMax;
                    break;

                case ParticleSystemCurveMode.TwoCurves:
                    SetIntOrEnum(modeProp, 3);
                    if (minCurveProp != null)
                        minCurveProp.animationCurveValue = ScaleCurve(srcCurve.curveMin, srcCurve.curveMultiplier);
                    if (curveProp != null)
                        curveProp.animationCurveValue = ScaleCurve(srcCurve.curveMax, srcCurve.curveMultiplier);
                    break;
            }
        }

        private static void SetIntOrEnum(SerializedProperty prop, int value)
        {
            if (prop == null)
                return;

            if (prop.propertyType == SerializedPropertyType.Enum)
                prop.enumValueIndex = value;
            else
                prop.intValue = value;
        }

        private static AnimationCurve ExtractRawCurve(ParticleSystem.MinMaxCurve minMaxCurve)
        {
            switch (minMaxCurve.mode)
            {
                case ParticleSystemCurveMode.Curve:
                    if (minMaxCurve.curve != null)
                        return minMaxCurve.curve;
                    break;

                case ParticleSystemCurveMode.TwoCurves:
                    if (minMaxCurve.curveMax != null)
                        return minMaxCurve.curveMax;
                    break;

                case ParticleSystemCurveMode.Constant:
                    return AnimationCurve.Linear(0f, minMaxCurve.constant, 1f, minMaxCurve.constant);

                case ParticleSystemCurveMode.TwoConstants:
                    float avg = (minMaxCurve.constantMin + minMaxCurve.constantMax) * 0.5f;
                    return AnimationCurve.Linear(0f, avg, 1f, avg);
            }

            return null;
        }

        private static Sprite ConvertTextureToSprite(Texture2D texture)
        {
            if (texture == null) return null;

            string path = AssetDatabase.GetAssetPath(texture);
            if (string.IsNullOrEmpty(path)) return null;

            var importer = AssetImporter.GetAtPath(path) as TextureImporter;

            if (importer != null && importer.textureType == TextureImporterType.Sprite)
            {
                Sprite existing = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (existing != null) return existing;
            }

            if (importer != null)
            {
                bool needsReimport = false;

                if (importer.textureType != TextureImporterType.Sprite)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Single;
                    needsReimport = true;
                }

                if (!importer.isReadable)
                {
                    importer.isReadable = true;
                    needsReimport = true;
                }

                if (needsReimport)
                {
                    importer.SaveAndReimport();
                }

                return AssetDatabase.LoadAssetAtPath<Sprite>(path);
            }

            return null;
        }
    }
}