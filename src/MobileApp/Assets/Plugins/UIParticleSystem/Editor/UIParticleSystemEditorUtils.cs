#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TierStudios.UIParticleSystem.Enums;
using TierStudios.UIParticleSystem.Utilities;


namespace TierStudios.UIParticleSystem.Editor
{
    public class UIParticleSystemEditorUtils
    {
        public void ValidateAnimationCurves(Core.UIParticleSystem particleSystem)
        {
            var serializedObject = new SerializedObject(particleSystem);

            var sizeSettings = serializedObject.FindProperty("sizeSettings");
            var sizeOverLifetimeCurve = sizeSettings.FindPropertyRelative("sizeOverLifetimeCurve");

            if (sizeOverLifetimeCurve.animationCurveValue == null || sizeOverLifetimeCurve.animationCurveValue.keys.Length == 0)
            {
                sizeOverLifetimeCurve.animationCurveValue = AnimationCurve.Linear(0f, 1f, 1f, 1f);
            }

            var sizeXOverLifetimeCurve = sizeSettings.FindPropertyRelative("sizeXOverLifetimeCurve");
            if (sizeXOverLifetimeCurve.animationCurveValue == null || sizeXOverLifetimeCurve.animationCurveValue.keys.Length == 0)
            {
                sizeXOverLifetimeCurve.animationCurveValue = AnimationCurve.Linear(0f, 1f, 1f, 0f);
            }

            var sizeYOverLifetimeCurve = sizeSettings.FindPropertyRelative("sizeYOverLifetimeCurve");
            if (sizeYOverLifetimeCurve.animationCurveValue == null || sizeYOverLifetimeCurve.animationCurveValue.keys.Length == 0)
            {
                sizeYOverLifetimeCurve.animationCurveValue = AnimationCurve.Linear(0f, 1f, 1f, 0f);
            }

            var attractorSettings = serializedObject.FindProperty("attractorSettings");
            var attractorFalloff = attractorSettings.FindPropertyRelative("attractorFalloff");

            if (attractorFalloff.animationCurveValue == null || attractorFalloff.animationCurveValue.keys.Length == 0)
            {
                attractorFalloff.animationCurveValue = AnimationCurve.Linear(0f, 1f, 1f, 0f);
            }

            var trailSettings = serializedObject.FindProperty("trailSettings");
            var trailWidthCurve = trailSettings.FindPropertyRelative("trailWidthCurve");

            if (trailWidthCurve.animationCurveValue == null || trailWidthCurve.animationCurveValue.keys.Length == 0)
            {
                trailWidthCurve.animationCurveValue = AnimationCurve.Linear(0f, 1f, 1f, 0f);
            }

            var velocityOverLifetimeSettings = serializedObject.FindProperty("velocityOverLifetimeSettings");

            var linearVelocityX = velocityOverLifetimeSettings.FindPropertyRelative("linearVelocityX");
            if (linearVelocityX.animationCurveValue == null || linearVelocityX.animationCurveValue.keys.Length == 0)
            {
                linearVelocityX.animationCurveValue = AnimationCurve.Linear(0f, 0f, 1f, 0f);
            }

            var linearVelocityY = velocityOverLifetimeSettings.FindPropertyRelative("linearVelocityY");
            if (linearVelocityY.animationCurveValue == null || linearVelocityY.animationCurveValue.keys.Length == 0)
            {
                linearVelocityY.animationCurveValue = AnimationCurve.Linear(0f, 0f, 1f, 0f);
            }

            var orbitalVelocityCurve = velocityOverLifetimeSettings.FindPropertyRelative("orbitalVelocityCurve");
            if (orbitalVelocityCurve.animationCurveValue == null || orbitalVelocityCurve.animationCurveValue.keys.Length == 0)
            {
                orbitalVelocityCurve.animationCurveValue = AnimationCurve.Linear(0f, 0f, 1f, 0f);
            }

            serializedObject.ApplyModifiedProperties();
        }


        public static void CreateUIParticleSystemGameObject(MenuCommand menuCommand)
        {
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
                    GameObject eventSystemObj = new GameObject("EventSystem");
                    eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                    eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                }
            }

            GameObject go = new GameObject("UI Particle System");
            RectTransform rectTransform = go.AddComponent<RectTransform>();
            TierStudios.UIParticleSystem.Core.UIParticleSystem particleSystem = go.AddComponent<TierStudios.UIParticleSystem.Core.UIParticleSystem>();
            particleSystem.LoadDefaultSprite();

            GameObjectUtility.SetParentAndAlign(go, canvas.gameObject);

            GameObject parent = menuCommand.context as GameObject;
            if (parent != null)
            {
                GameObjectUtility.SetParentAndAlign(go, parent);
            }

            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(200, 200);

            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

        #region Settings Rendering Methods

        public enum StartDelayMode
        {
            Constant,
            RandomBetweenConstants
        }

        public void RenderDurationSettings(SerializedProperty emissionSettings)
        {
            SerializedProperty durationProp = emissionSettings.FindPropertyRelative("duration");
            SerializedProperty startDelayModeProp = emissionSettings.FindPropertyRelative("startDelayMode");
            SerializedProperty startDelayProp = emissionSettings.FindPropertyRelative("startDelay");
            SerializedProperty startDelayMinProp = emissionSettings.FindPropertyRelative("startDelayMin");
            SerializedProperty startDelayMaxProp = emissionSettings.FindPropertyRelative("startDelayMax");

            SerializedProperty loopingProp = emissionSettings.FindPropertyRelative("looping");
            SerializedProperty prewarmProp = emissionSettings.FindPropertyRelative("prewarm");
            SerializedProperty emitOnStartProp = emissionSettings.FindPropertyRelative("emitOnStart");

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(durationProp, new GUIContent("Duration", "System duration in seconds (0 = infinite)"));
            if (EditorGUI.EndChangeCheck())
            {
                durationProp.floatValue = Mathf.Max(0f, durationProp.floatValue);
            }

            EditorGUILayout.PropertyField(loopingProp, new GUIContent("Looping"));
            EditorGUILayout.PropertyField(prewarmProp, new GUIContent("Prewarm"));
            
            RenderMinMaxProperty(new GUIContent("Start Delay"), startDelayModeProp, startDelayProp, startDelayMinProp, startDelayMaxProp, 0f, 60f);

            EditorGUILayout.PropertyField(emitOnStartProp, new GUIContent("Play On Awake"));
        }

        public void RenderGravitySettings(SerializedObject serializedObject)
        {
            SerializedProperty useGravityProp = serializedObject.FindProperty("useGravity");
            EditorGUILayout.PropertyField(useGravityProp, new GUIContent("Use Gravity", "Apply gravity force to particles"));

            if (useGravityProp.boolValue)
            {
                SerializedProperty gravityProp = serializedObject.FindProperty("gravity");
                EditorGUILayout.PropertyField(gravityProp, new GUIContent("Gravity", "Gravity force vector"));
            }

            SerializedProperty colorSettings = serializedObject.FindProperty("colorSettings");
            SerializedProperty fadeOutProp = colorSettings.FindPropertyRelative("fadeOut");
            EditorGUILayout.PropertyField(fadeOutProp, new GUIContent("Fade Out", "Fade particles out over lifetime"));
        }

        public void RenderMaxParticlesSettings(SerializedProperty emissionSettings)
        {
            SerializedProperty maxParticlesProp = emissionSettings.FindPropertyRelative("maxParticles");

            if (maxParticlesProp.hasMultipleDifferentValues)
            {

                EditorGUI.showMixedValue = true;

                EditorGUI.BeginChangeCheck();
                int newValue = EditorGUILayout.IntField(new GUIContent("Max Particles", "Maximum number of particles that can be active at once"), 0);

                if (EditorGUI.EndChangeCheck())
                {
                    maxParticlesProp.intValue = Mathf.Clamp(newValue, 0, 1000);
                }

                EditorGUI.showMixedValue = false;
            }
            else
            {
                EditorGUILayout.PropertyField(maxParticlesProp, new GUIContent("Max Particles", "Maximum number of particles that can be active at once"));
                maxParticlesProp.intValue = Mathf.Clamp(maxParticlesProp.intValue, 0, 1000);
            }
        }

        public void RenderSimulationSpaceSettings(SerializedProperty emissionSettings)
        {
            SerializedProperty simulationSpaceProp = emissionSettings.FindPropertyRelative("simulationSpace");
            EditorGUILayout.PropertyField(simulationSpaceProp, new GUIContent("Simulation Space", "Controls whether particles move in local or world space"));
        }

        public void RenderMinMaxProperty(GUIContent label, SerializedProperty useRandomProp, SerializedProperty constantProp, SerializedProperty minProp, SerializedProperty maxProp, float clampMin = float.MinValue, float clampMax = float.MaxValue, bool isColor = false)
        {
            const float popupWidth = 18f;
            const float padding = 4f;

            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            
            Rect popupRect = new Rect(rect.xMax - popupWidth, rect.y, popupWidth, rect.height);
            Rect propRect = new Rect(rect.x, rect.y, rect.width - popupWidth - padding, rect.height);

            Rect fieldBase = EditorGUI.PrefixLabel(propRect, label);
            float fieldWidth = fieldBase.width;

            bool useRandom;
            if (useRandomProp.propertyType == SerializedPropertyType.Enum)
                useRandom = useRandomProp.enumValueIndex == 1;
            else
                useRandom = useRandomProp.boolValue;

            int oldIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            if (!useRandom)
            {
                EditorGUI.showMixedValue = constantProp.hasMultipleDifferentValues;
                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(fieldBase, constantProp, GUIContent.none);
                if (EditorGUI.EndChangeCheck() && !isColor)
                {
                    if (constantProp.propertyType == SerializedPropertyType.Float)
                        constantProp.floatValue = Mathf.Clamp(constantProp.floatValue, clampMin, clampMax);
                    else if (constantProp.propertyType == SerializedPropertyType.Integer)
                        constantProp.intValue = Mathf.Clamp(constantProp.intValue, (int)clampMin, (int)clampMax);
                }
                EditorGUI.showMixedValue = false;
            }
            else
            {
                float half = (fieldWidth - padding) * 0.5f;
                Rect minRect = new Rect(fieldBase.x, fieldBase.y, half, fieldBase.height);
                Rect maxRect = new Rect(fieldBase.x + half + padding, fieldBase.y, half, fieldBase.height);

                EditorGUI.showMixedValue = minProp.hasMultipleDifferentValues;
                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(minRect, minProp, GUIContent.none);
                if (EditorGUI.EndChangeCheck() && !isColor)
                {
                    if (minProp.propertyType == SerializedPropertyType.Float)
                        minProp.floatValue = Mathf.Clamp(minProp.floatValue, clampMin, clampMax);
                    else if (minProp.propertyType == SerializedPropertyType.Integer)
                        minProp.intValue = Mathf.Clamp(minProp.intValue, (int)clampMin, (int)clampMax);
                }
                EditorGUI.showMixedValue = false;

                EditorGUI.showMixedValue = maxProp.hasMultipleDifferentValues;
                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(maxRect, maxProp, GUIContent.none);
                if (EditorGUI.EndChangeCheck() && !isColor)
                {
                    float currentMin = minProp.propertyType == SerializedPropertyType.Float ? minProp.floatValue : minProp.intValue;
                    float minAllowed = Mathf.Max(clampMin, currentMin);
                    if (maxProp.propertyType == SerializedPropertyType.Float)
                        maxProp.floatValue = Mathf.Clamp(maxProp.floatValue, minAllowed, clampMax);
                    else if (maxProp.propertyType == SerializedPropertyType.Integer)
                        maxProp.intValue = Mathf.Clamp(maxProp.intValue, (int)minAllowed, (int)clampMax);
                }
                EditorGUI.showMixedValue = false;
            }

            EditorGUI.indentLevel = oldIndent;

            if (EditorGUI.DropdownButton(popupRect, GUIContent.none, FocusType.Passive, EditorStyles.popup))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Constant"), !useRandom, () =>
                {
                    if (useRandomProp.propertyType == SerializedPropertyType.Enum)
                        useRandomProp.enumValueIndex = 0;
                    else
                        useRandomProp.boolValue = false;
                    useRandomProp.serializedObject.ApplyModifiedProperties();
                });
                menu.AddItem(new GUIContent("Random Between Two Constants"), useRandom, () =>
                {
                    if (useRandomProp.propertyType == SerializedPropertyType.Enum)
                        useRandomProp.enumValueIndex = 1;
                    else
                        useRandomProp.boolValue = true;
                    useRandomProp.serializedObject.ApplyModifiedProperties();
                });
                menu.DropDown(popupRect);
            }
        }

        public void RenderLifetimeControls(SerializedProperty lifetimeSettings)
        {
            SerializedProperty useRandomLifetimeProp = lifetimeSettings.FindPropertyRelative("useRandomLifetime");
            SerializedProperty minLifetimeProp = lifetimeSettings.FindPropertyRelative("minLifetime");
            SerializedProperty maxLifetimeProp = lifetimeSettings.FindPropertyRelative("maxLifetime");
            SerializedProperty lifetimeProp = lifetimeSettings.FindPropertyRelative("lifetime");

            RenderMinMaxProperty(new GUIContent("Start Lifetime", "Particle lifetime (seconds)"), useRandomLifetimeProp, lifetimeProp, minLifetimeProp, maxLifetimeProp, 0.1f, 60f);
        }

        public void RenderSpeedControls(SerializedObject serializedObject)
        {
            SerializedProperty useRandomSpeedProp = serializedObject.FindProperty("useRandomSpeed");
            SerializedProperty speedProp = serializedObject.FindProperty("speed");
            SerializedProperty minSpeedProp = serializedObject.FindProperty("minSpeed");
            SerializedProperty maxSpeedProp = serializedObject.FindProperty("maxSpeed");

            RenderMinMaxProperty(new GUIContent("Start Speed", "Initial particle speed"), useRandomSpeedProp, speedProp, minSpeedProp, maxSpeedProp, 0.01f, float.MaxValue);
        }

        public void RenderSizeControls(SerializedProperty sizeSettings)
        {
            SerializedProperty use2DSizeProp = sizeSettings.FindPropertyRelative("use2DSize");
            EditorGUILayout.PropertyField(use2DSizeProp, new GUIContent("2D Start Size", "Use separate width and height for particles"));

            SerializedProperty useSizeRangeProp = sizeSettings.FindPropertyRelative("useSizeRange");
            SerializedProperty use2DSizeRangeProp = sizeSettings.FindPropertyRelative("use2DSizeRange");
            
            if (use2DSizeProp.boolValue)
            {
                use2DSizeRangeProp.boolValue = useSizeRangeProp.boolValue;

                SerializedProperty size2DProp = sizeSettings.FindPropertyRelative("size2D");
                SerializedProperty minSize2DProp = sizeSettings.FindPropertyRelative("minSize2D");
                SerializedProperty maxSize2DProp = sizeSettings.FindPropertyRelative("maxSize2D");
                
                RenderMinMaxProperty(new GUIContent("Start Size", "Initial particle size (Width, Height)"), useSizeRangeProp, size2DProp, minSize2DProp, maxSize2DProp);
            }
            else
            {
                use2DSizeRangeProp.boolValue = false;
                
                SerializedProperty sizeProp = sizeSettings.FindPropertyRelative("size");
                SerializedProperty minSizeProp = sizeSettings.FindPropertyRelative("minSize");
                SerializedProperty maxSizeProp = sizeSettings.FindPropertyRelative("maxSize");

                RenderMinMaxProperty(new GUIContent("Start Size", "Initial particle size"), useSizeRangeProp, sizeProp, minSizeProp, maxSizeProp);
            }
        }

        public void RenderRotationControls(SerializedProperty rotationSettings)
        {
            SerializedProperty useRandomRotationProp = rotationSettings.FindPropertyRelative("useRandomRotation");
            SerializedProperty initialRotationProp = rotationSettings.FindPropertyRelative("initialRotation");
            SerializedProperty minRotationProp = rotationSettings.FindPropertyRelative("minRotation");
            SerializedProperty maxRotationProp = rotationSettings.FindPropertyRelative("maxRotation");

            RenderMinMaxProperty(new GUIContent("Start Rotation", "Initial particle rotation"), useRandomRotationProp, initialRotationProp, minRotationProp, maxRotationProp);
        }

        public void RenderRotationOverLifetime(SerializedProperty rotationSettings)
        {
            SerializedProperty useRotationOverLifetimeProp = rotationSettings.FindPropertyRelative("useRotationOverLifetime");
            bool isSizeOverLifetimeEnabled = useRotationOverLifetimeProp.boolValue;

            EditorGUI.BeginDisabledGroup(!isSizeOverLifetimeEnabled);


            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(rotationSettings.FindPropertyRelative("useRandomRotationSpeed"), new GUIContent("Random Rotation Speed"));

            if (rotationSettings.FindPropertyRelative("useRandomRotationSpeed").boolValue)
            {
                EditorGUILayout.PropertyField(rotationSettings.FindPropertyRelative("minRotationSpeed"), new GUIContent("Min Speed"));
                EditorGUILayout.PropertyField(rotationSettings.FindPropertyRelative("maxRotationSpeed"), new GUIContent("Max Speed"));
            }
            else
            {
                EditorGUILayout.PropertyField(rotationSettings.FindPropertyRelative("rotationSpeed"), new GUIContent("Rotation Speed"));
            }
            EditorGUI.indentLevel--;

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(4);
            EditorGUILayout.PropertyField(rotationSettings.FindPropertyRelative("alignToDirection"),
                new GUIContent("Align To Direction", "Face each particle along its velocity. Start Rotation is applied as an extra spin offset. Stretched Billboard already faces velocity."));

            SerializedProperty useRotationBySpeedProp = rotationSettings.FindPropertyRelative("useRotationBySpeed");
            EditorGUILayout.PropertyField(useRotationBySpeedProp,
                new GUIContent("Rotation By Speed", "Add extra spin based on how fast the particle is moving"));

            EditorGUI.BeginDisabledGroup(!useRotationBySpeedProp.boolValue);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(rotationSettings.FindPropertyRelative("rotationBySpeed"),
                new GUIContent("Angular Velocity", "Degrees per second at Max Speed"));
            EditorGUILayout.PropertyField(rotationSettings.FindPropertyRelative("rotationBySpeedMin"),
                new GUIContent("Speed Range Min"));
            EditorGUILayout.PropertyField(rotationSettings.FindPropertyRelative("rotationBySpeedMax"),
                new GUIContent("Speed Range Max"));
            EditorGUI.indentLevel--;
            EditorGUI.EndDisabledGroup();
        }

        public void RenderColorControls(SerializedProperty colorSettings)
        {
            SerializedProperty useRandomColorsProp = colorSettings.FindPropertyRelative("useRandomColors");
            SerializedProperty particleColorProp = colorSettings.FindPropertyRelative("particleColor");
            SerializedProperty particleColorEndProp = colorSettings.FindPropertyRelative("particleColorEnd");

            RenderMinMaxProperty(new GUIContent("Start Color", "Initial particle color"), useRandomColorsProp, particleColorProp, particleColorProp, particleColorEndProp, float.MinValue, float.MaxValue, true);
        }

        public void RenderColorOverLifetime(SerializedProperty colorSettings)
        {
            SerializedProperty useGradientColorProp = colorSettings.FindPropertyRelative("useGradientColor");
            bool useGradientColor = useGradientColorProp.boolValue;

            EditorGUI.BeginDisabledGroup(!useGradientColor);

            SerializedProperty useRandomColorsProp = colorSettings.FindPropertyRelative("useRandomColors");
            bool useRandomColors = useRandomColorsProp.boolValue;

            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            Rect popupRect = new Rect(rect.xMax - 18f, rect.y, 18f, rect.height);
            Rect propRect = new Rect(rect.x, rect.y, rect.width - 18f - 4f, rect.height);

            // Dropdown button on the right
            if (EditorGUI.DropdownButton(popupRect, GUIContent.none, FocusType.Passive, EditorStyles.popup))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Gradient"), !useRandomColors, () =>
                {
                    useRandomColorsProp.boolValue = false;
                    useRandomColorsProp.serializedObject.ApplyModifiedProperties();
                });
                menu.AddItem(new GUIContent("Random Between Two Gradients"), useRandomColors, () =>
                {
                    useRandomColorsProp.boolValue = true;
                    useRandomColorsProp.serializedObject.ApplyModifiedProperties();
                });
                menu.DropDown(popupRect);
            }

            if (!useRandomColors)
            {
                EditorGUI.PropertyField(propRect, colorSettings.FindPropertyRelative("colorGradient"), new GUIContent("Color"));
            }
            else
            {
                EditorGUI.PropertyField(propRect, colorSettings.FindPropertyRelative("colorGradient"), new GUIContent("Gradient1"));
                EditorGUILayout.PropertyField(colorSettings.FindPropertyRelative("colorGradient2"), new GUIContent("Gradient2"));
            }

            EditorGUI.EndDisabledGroup();
        }


        public void RenderSizeOverLifetimeControls(SerializedProperty sizeSettings, bool use2DSize)
        {
            SerializedProperty useSizeOverLifetimeProp = sizeSettings.FindPropertyRelative("useSizeOverLifetime");
            bool isSizeOverLifetimeEnabled = useSizeOverLifetimeProp.boolValue;

            EditorGUI.BeginDisabledGroup(!isSizeOverLifetimeEnabled);

            SerializedProperty use2DSizeOverLifetimeProp = sizeSettings.FindPropertyRelative("use2DSizeOverLifetime");
            EditorGUILayout.PropertyField(use2DSizeOverLifetimeProp, new GUIContent("Separate Axes", "Use separate curves for width (X) and height (Y)"));

            if (use2DSizeOverLifetimeProp.boolValue)
            {
                Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
                Rect fieldBase = EditorGUI.PrefixLabel(rect, new GUIContent("Size X / Y", "Separate axes curves"));

                float halfWidth = (fieldBase.width - 8f) / 2f;

                int oldIndent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                Rect xRect = new Rect(fieldBase.x, fieldBase.y, halfWidth, fieldBase.height);
                float labelXWidth = 14f;
                EditorGUI.LabelField(new Rect(xRect.x, xRect.y, labelXWidth, xRect.height), "X", EditorStyles.miniLabel);

                Rect xCurveRect = new Rect(xRect.x + labelXWidth, xRect.y, xRect.width - labelXWidth, xRect.height);
                SerializedProperty sizeXOverLifetimeCurveProp = sizeSettings.FindPropertyRelative("sizeXOverLifetimeCurve");

                EditorGUI.BeginChangeCheck();
                var newWidthCurve = EditorGUI.CurveField(xCurveRect, sizeXOverLifetimeCurveProp.animationCurveValue ?? AnimationCurve.Linear(0f, 1f, 1f, 0f));
                if (EditorGUI.EndChangeCheck() && isSizeOverLifetimeEnabled)
                {
                    sizeXOverLifetimeCurveProp.animationCurveValue = newWidthCurve;
                }

                Rect yRect = new Rect(fieldBase.x + halfWidth + 8f, fieldBase.y, halfWidth, fieldBase.height);
                float labelYWidth = 14f;
                EditorGUI.LabelField(new Rect(yRect.x, yRect.y, labelYWidth, yRect.height), "Y", EditorStyles.miniLabel);

                Rect yCurveRect = new Rect(yRect.x + labelYWidth, yRect.y, yRect.width - labelYWidth, yRect.height);
                SerializedProperty sizeYOverLifetimeCurveProp = sizeSettings.FindPropertyRelative("sizeYOverLifetimeCurve");

                EditorGUI.BeginChangeCheck();
                var newHeightCurve = EditorGUI.CurveField(yCurveRect, sizeYOverLifetimeCurveProp.animationCurveValue ?? AnimationCurve.Linear(0f, 1f, 1f, 0f));
                if (EditorGUI.EndChangeCheck() && isSizeOverLifetimeEnabled)
                {
                    sizeYOverLifetimeCurveProp.animationCurveValue = newHeightCurve;
                }

                EditorGUI.indentLevel = oldIndent;

                Rect rect2 = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
                Rect multiplierBase = EditorGUI.PrefixLabel(rect2, new GUIContent("Multiplier", "Overall 2D size multiplier"));

                EditorGUI.indentLevel = 0;

                SerializedProperty size2DMultiplierProp = sizeSettings.FindPropertyRelative("size2DMultiplier");
                SerializedProperty multXProp = size2DMultiplierProp.FindPropertyRelative("x");
                SerializedProperty multYProp = size2DMultiplierProp.FindPropertyRelative("y");

                Rect xMultRect = new Rect(multiplierBase.x + labelXWidth, rect2.y, halfWidth - labelXWidth, rect2.height);
                EditorGUI.LabelField(new Rect(multiplierBase.x, rect2.y, labelXWidth, rect2.height), "X", EditorStyles.miniLabel);
                
                EditorGUI.BeginChangeCheck();
                float newMultX = EditorGUI.FloatField(xMultRect, multXProp.floatValue);
                if (EditorGUI.EndChangeCheck() && isSizeOverLifetimeEnabled)
                {
                    multXProp.floatValue = Mathf.Max(0.01f, newMultX);
                }

                Rect yMultRect = new Rect(multiplierBase.x + halfWidth + 8f + labelYWidth, rect2.y, halfWidth - labelYWidth, rect2.height);
                EditorGUI.LabelField(new Rect(multiplierBase.x + halfWidth + 8f, rect2.y, labelYWidth, rect2.height), "Y", EditorStyles.miniLabel);

                EditorGUI.BeginChangeCheck();
                float newMultY = EditorGUI.FloatField(yMultRect, multYProp.floatValue);
                if (EditorGUI.EndChangeCheck() && isSizeOverLifetimeEnabled)
                {
                    multYProp.floatValue = Mathf.Max(0.01f, newMultY);
                }

                EditorGUI.indentLevel = oldIndent;
            }
            else
            {
                SerializedProperty curveProp = sizeSettings.FindPropertyRelative("sizeOverLifetimeCurve");
                SerializedProperty multiplierProp = sizeSettings.FindPropertyRelative("sizeMultiplier");

                EditorGUI.BeginChangeCheck();
                var newCurve = EditorGUILayout.CurveField(new GUIContent("Size", "Size multiplier curve over lifetime (0-1)"),
                    curveProp.animationCurveValue ?? AnimationCurve.Linear(0f, 1f, 1f, 1f));
                if (EditorGUI.EndChangeCheck() && isSizeOverLifetimeEnabled)
                {
                    curveProp.animationCurveValue = newCurve;
                }

                EditorGUILayout.PropertyField(multiplierProp, new GUIContent("Multiplier", "Overall size multiplier for the curve"));
                if (isSizeOverLifetimeEnabled)
                {
                    multiplierProp.floatValue = Mathf.Clamp(multiplierProp.floatValue, 0.1f, 10f);
                }
            }

            EditorGUI.EndDisabledGroup();
        }

        public void RenderColorBySpeedControls(SerializedProperty colorBySpeedSettings)
        {
            SerializedProperty useColorBySpeedProp = colorBySpeedSettings.FindPropertyRelative("useColorBySpeed");
            bool isEnabled = useColorBySpeedProp.boolValue;

            EditorGUI.BeginDisabledGroup(!isEnabled);

            SerializedProperty useRandomColorsProp = colorBySpeedSettings.FindPropertyRelative("useRandomColors");
            bool useRandomColors = useRandomColorsProp.boolValue;

            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            Rect popupRect = new Rect(rect.xMax - 18f, rect.y, 18f, rect.height);
            Rect propRect = new Rect(rect.x, rect.y, rect.width - 18f - 4f, rect.height);

            if (EditorGUI.DropdownButton(popupRect, GUIContent.none, FocusType.Passive, EditorStyles.popup))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Gradient"), !useRandomColors, () =>
                {
                    useRandomColorsProp.boolValue = false;
                    useRandomColorsProp.serializedObject.ApplyModifiedProperties();
                });
                menu.AddItem(new GUIContent("Random Between Two Gradients"), useRandomColors, () =>
                {
                    useRandomColorsProp.boolValue = true;
                    useRandomColorsProp.serializedObject.ApplyModifiedProperties();
                });
                menu.DropDown(popupRect);
            }

            if (!useRandomColors)
            {
                EditorGUI.PropertyField(propRect, colorBySpeedSettings.FindPropertyRelative("colorGradient"), new GUIContent("Color"));
            }
            else
            {
                EditorGUI.PropertyField(propRect, colorBySpeedSettings.FindPropertyRelative("colorGradient"), new GUIContent("Gradient1"));
                EditorGUILayout.PropertyField(colorBySpeedSettings.FindPropertyRelative("colorGradient2"), new GUIContent("Gradient2"));
            }

            Rect rectSpeed = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            Rect speedBase = EditorGUI.PrefixLabel(rectSpeed, new GUIContent("Speed Range", "The particle's speed range for mapping colors"));
            
            float halfWidth = (speedBase.width - 8f) / 2f;
            int oldIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            SerializedProperty minSpeedProp = colorBySpeedSettings.FindPropertyRelative("minSpeed");
            SerializedProperty maxSpeedProp = colorBySpeedSettings.FindPropertyRelative("maxSpeed");

            Rect minRect = new Rect(speedBase.x, speedBase.y, halfWidth, speedBase.height);
            EditorGUI.BeginChangeCheck();
            float newMin = EditorGUI.FloatField(minRect, minSpeedProp.floatValue);
            if (EditorGUI.EndChangeCheck() && isEnabled)
            {
                minSpeedProp.floatValue = Mathf.Max(0f, newMin);
            }

            Rect maxRect = new Rect(speedBase.x + halfWidth + 8f, speedBase.y, halfWidth, speedBase.height);
            EditorGUI.BeginChangeCheck();
            float newMax = EditorGUI.FloatField(maxRect, maxSpeedProp.floatValue);
            if (EditorGUI.EndChangeCheck() && isEnabled)
            {
                maxSpeedProp.floatValue = Mathf.Max(minSpeedProp.floatValue + 0.1f, newMax);
            }

            EditorGUI.indentLevel = oldIndent;

            EditorGUI.EndDisabledGroup();
        }

        public void RenderSizeBySpeedControls(SerializedProperty sizeBySpeedSettings)
        {
            SerializedProperty useSizeBySpeedProp = sizeBySpeedSettings.FindPropertyRelative("useSizeBySpeed");
            bool isEnabled = useSizeBySpeedProp.boolValue;

            EditorGUI.BeginDisabledGroup(!isEnabled);

            SerializedProperty separateAxesProp = sizeBySpeedSettings.FindPropertyRelative("separateAxes");
            EditorGUILayout.PropertyField(separateAxesProp, new GUIContent("Separate Axes", "Use separate curves for width (X) and height (Y) based on speed"));

            int oldIndent = EditorGUI.indentLevel;

            if (separateAxesProp.boolValue)
            {
                Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
                Rect fieldBase = EditorGUI.PrefixLabel(rect, new GUIContent("Size X / Y", "Separate axes curves based on speed"));

                float halfWidth = (fieldBase.width - 8f) / 2f;
                EditorGUI.indentLevel = 0;

                Rect xRect = new Rect(fieldBase.x, fieldBase.y, halfWidth, fieldBase.height);
                float labelXWidth = 14f;
                EditorGUI.LabelField(new Rect(xRect.x, xRect.y, labelXWidth, xRect.height), "X", EditorStyles.miniLabel);

                Rect xCurveRect = new Rect(xRect.x + labelXWidth, xRect.y, xRect.width - labelXWidth, xRect.height);
                SerializedProperty sizeXCurveProp = sizeBySpeedSettings.FindPropertyRelative("sizeXCurve");

                EditorGUI.BeginChangeCheck();
                var newWidthCurve = EditorGUI.CurveField(xCurveRect, sizeXCurveProp.animationCurveValue ?? AnimationCurve.Linear(0f, 1f, 1f, 1f));
                if (EditorGUI.EndChangeCheck() && isEnabled)
                {
                    sizeXCurveProp.animationCurveValue = newWidthCurve;
                }

                Rect yRect = new Rect(fieldBase.x + halfWidth + 8f, fieldBase.y, halfWidth, fieldBase.height);
                float labelYWidth = 14f;
                EditorGUI.LabelField(new Rect(yRect.x, yRect.y, labelYWidth, yRect.height), "Y", EditorStyles.miniLabel);

                Rect yCurveRect = new Rect(yRect.x + labelYWidth, yRect.y, yRect.width - labelYWidth, yRect.height);
                SerializedProperty sizeYCurveProp = sizeBySpeedSettings.FindPropertyRelative("sizeYCurve");

                EditorGUI.BeginChangeCheck();
                var newHeightCurve = EditorGUI.CurveField(yCurveRect, sizeYCurveProp.animationCurveValue ?? AnimationCurve.Linear(0f, 1f, 1f, 1f));
                if (EditorGUI.EndChangeCheck() && isEnabled)
                {
                    sizeYCurveProp.animationCurveValue = newHeightCurve;
                }

                Rect rect2 = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
                Rect multiplierBase = EditorGUI.PrefixLabel(rect2, new GUIContent("Multiplier", "Overall 2D size multiplier"));

                SerializedProperty size2DMultiplierProp = sizeBySpeedSettings.FindPropertyRelative("size2DMultiplier");
                SerializedProperty multXProp = size2DMultiplierProp.FindPropertyRelative("x");
                SerializedProperty multYProp = size2DMultiplierProp.FindPropertyRelative("y");

                Rect xMultRect = new Rect(multiplierBase.x + labelXWidth, rect2.y, halfWidth - labelXWidth, rect2.height);
                EditorGUI.LabelField(new Rect(multiplierBase.x, rect2.y, labelXWidth, rect2.height), "X", EditorStyles.miniLabel);
                
                EditorGUI.BeginChangeCheck();
                float newMultX = EditorGUI.FloatField(xMultRect, multXProp.floatValue);
                if (EditorGUI.EndChangeCheck() && isEnabled)
                {
                    multXProp.floatValue = Mathf.Max(0.01f, newMultX);
                }

                Rect yMultRect = new Rect(multiplierBase.x + halfWidth + 8f + labelYWidth, rect2.y, halfWidth - labelYWidth, rect2.height);
                EditorGUI.LabelField(new Rect(multiplierBase.x + halfWidth + 8f, rect2.y, labelYWidth, rect2.height), "Y", EditorStyles.miniLabel);

                EditorGUI.BeginChangeCheck();
                float newMultY = EditorGUI.FloatField(yMultRect, multYProp.floatValue);
                if (EditorGUI.EndChangeCheck() && isEnabled)
                {
                    multYProp.floatValue = Mathf.Max(0.01f, newMultY);
                }
            }
            else
            {
                SerializedProperty curveProp = sizeBySpeedSettings.FindPropertyRelative("sizeCurve");
                SerializedProperty multiplierProp = sizeBySpeedSettings.FindPropertyRelative("sizeMultiplier");

                EditorGUI.BeginChangeCheck();
                var newCurve = EditorGUILayout.CurveField(new GUIContent("Size", "Size multiplier curve based on speed"),
                    curveProp.animationCurveValue ?? AnimationCurve.Linear(0f, 1f, 1f, 1f));
                if (EditorGUI.EndChangeCheck() && isEnabled)
                {
                    curveProp.animationCurveValue = newCurve;
                }

                EditorGUILayout.PropertyField(multiplierProp, new GUIContent("Multiplier", "Overall size multiplier for the curve"));
                if (isEnabled)
                {
                    multiplierProp.floatValue = Mathf.Clamp(multiplierProp.floatValue, 0.1f, 10f);
                }
            }

            Rect rectSpeed = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            Rect speedBase = EditorGUI.PrefixLabel(rectSpeed, new GUIContent("Speed Range", "The particle's speed range for mapping size"));
            
            float halfWidthRange = (speedBase.width - 8f) / 2f;
            EditorGUI.indentLevel = 0;

            SerializedProperty minSpeedProp = sizeBySpeedSettings.FindPropertyRelative("minSpeed");
            SerializedProperty maxSpeedProp = sizeBySpeedSettings.FindPropertyRelative("maxSpeed");

            Rect minRect = new Rect(speedBase.x, speedBase.y, halfWidthRange, speedBase.height);
            EditorGUI.BeginChangeCheck();
            float newMin = EditorGUI.FloatField(minRect, minSpeedProp.floatValue);
            if (EditorGUI.EndChangeCheck() && isEnabled)
            {
                minSpeedProp.floatValue = Mathf.Max(0f, newMin);
            }

            Rect maxRect = new Rect(speedBase.x + halfWidthRange + 8f, speedBase.y, halfWidthRange, speedBase.height);
            EditorGUI.BeginChangeCheck();
            float newMax = EditorGUI.FloatField(maxRect, maxSpeedProp.floatValue);
            if (EditorGUI.EndChangeCheck() && isEnabled)
            {
                maxSpeedProp.floatValue = Mathf.Max(minSpeedProp.floatValue + 0.1f, newMax);
            }

            EditorGUI.indentLevel = oldIndent;

            EditorGUI.EndDisabledGroup();
        }

        public void RenderEmissionControls(SerializedProperty emissionSettings)
        {
            SerializedProperty emissionRateProp = emissionSettings.FindPropertyRelative("emissionRate");

            if (emissionRateProp.hasMultipleDifferentValues)
            {
                EditorGUI.showMixedValue = true;
                EditorGUI.BeginChangeCheck();
                float newEmissionRate = EditorGUILayout.FloatField(new GUIContent("Rate over Time"), 0f);
                if (EditorGUI.EndChangeCheck())
                {
                    emissionRateProp.floatValue = Mathf.Clamp(newEmissionRate, 0f, 100f);
                }
                EditorGUI.showMixedValue = false;
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(emissionRateProp, new GUIContent("Rate over Time"));
                if (EditorGUI.EndChangeCheck())
                {
                    emissionRateProp.floatValue = Mathf.Clamp(emissionRateProp.floatValue, 0f, 100f);
                }
            }

            RenderDistanceEmissionControls(emissionSettings);
            RenderBurstControls(emissionSettings);
        }

        private void RenderDistanceEmissionControls(SerializedProperty emissionSettings)
        {
            SerializedProperty emissionRateOverDistanceProp =
                emissionSettings.FindPropertyRelative("emissionRateOverDistance");

            if (emissionRateOverDistanceProp.hasMultipleDifferentValues)
            {
                EditorGUI.showMixedValue = true;
                EditorGUI.BeginChangeCheck();
                float newVal = EditorGUILayout.FloatField(new GUIContent("Rate over Distance"), 0f);
                if (EditorGUI.EndChangeCheck())
                    emissionRateOverDistanceProp.floatValue = Mathf.Max(0f, newVal);
                EditorGUI.showMixedValue = false;
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(emissionRateOverDistanceProp, new GUIContent("Rate over Distance"));
                if (EditorGUI.EndChangeCheck())
                {
                    emissionRateOverDistanceProp.floatValue = Mathf.Max(0f, emissionRateOverDistanceProp.floatValue);
                }
            }
        }

        private void RenderBurstControls(SerializedProperty emissionSettings)
        {
            SerializedProperty burstsProperty = emissionSettings.FindPropertyRelative("bursts");

            EditorGUILayout.LabelField("Bursts", EditorStyles.boldLabel);

            if (burstsProperty.hasMultipleDifferentValues)
            {
                EditorGUILayout.HelpBox("Multi-selection burst editing is not supported.", MessageType.Info);
                return;
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            Rect headerRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
            EditorGUI.DrawRect(headerRect, new Color(0.2f, 0.2f, 0.2f, 0.4f));

            float colWidth = headerRect.width / 3f;
            GUIStyle centeredMiniLabel = new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleCenter };

            EditorGUI.LabelField(new Rect(headerRect.x, headerRect.y, colWidth, headerRect.height), "Time", centeredMiniLabel);
            EditorGUI.LabelField(new Rect(headerRect.x + colWidth, headerRect.y, colWidth, headerRect.height), "Count", centeredMiniLabel);
            EditorGUI.LabelField(new Rect(headerRect.x + colWidth * 2, headerRect.y, colWidth, headerRect.height), "Probability", centeredMiniLabel);

            if (burstsProperty.arraySize == 0)
            {
                EditorGUILayout.LabelField("No bursts defined.", EditorStyles.centeredGreyMiniLabel);
            }
            else
            {
                for (int i = 0; i < burstsProperty.arraySize; i++)
                {
                    SerializedProperty burstElement = burstsProperty.GetArrayElementAtIndex(i);
                    Rect rowRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight + 2);

                    if (i % 2 == 0)
                        EditorGUI.DrawRect(rowRect, new Color(1f, 1f, 1f, 0.05f));

                    float fieldPadding = 4f;
                    float fieldWidth = colWidth - (fieldPadding * 2f);

                    SerializedProperty timeProp = burstElement.FindPropertyRelative("time");
                    SerializedProperty countProp = burstElement.FindPropertyRelative("count");
                    SerializedProperty probProp = burstElement.FindPropertyRelative("probability");

                    Rect timeFieldRect = new Rect(rowRect.x + fieldPadding, rowRect.y + 1, fieldWidth, EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(timeFieldRect, timeProp, GUIContent.none);

                    Rect countFieldRect = new Rect(rowRect.x + colWidth + fieldPadding, rowRect.y + 1, fieldWidth, EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(countFieldRect, countProp, GUIContent.none);

                    Rect probFieldRect = new Rect(rowRect.x + colWidth * 2 + fieldPadding, rowRect.y + 1, fieldWidth, EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(probFieldRect, probProp, GUIContent.none);
                }
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            EditorGUI.BeginDisabledGroup(burstsProperty.arraySize >= 5);
            if (GUILayout.Button(new GUIContent("+", "Add new burst"), EditorStyles.miniButtonLeft, GUILayout.Width(30)))
            {
                burstsProperty.arraySize++;
                SerializedProperty newBurst = burstsProperty.GetArrayElementAtIndex(burstsProperty.arraySize - 1);
                newBurst.FindPropertyRelative("time").floatValue = 0f;
                newBurst.FindPropertyRelative("count").intValue = 10;
                newBurst.FindPropertyRelative("probability").floatValue = 1f;
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(burstsProperty.arraySize == 0);
            if (GUILayout.Button(new GUIContent("-", "Remove last burst"), EditorStyles.miniButtonRight, GUILayout.Width(30)))
            {
                burstsProperty.arraySize--;
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(2);
        }

        public void RenderShapeControls(SerializedProperty emissionSettings)
        {
            EditorGUILayout.PropertyField(emissionSettings.FindPropertyRelative("emissionShape"));

            EmissionShapeType shapeType = (EmissionShapeType)emissionSettings.FindPropertyRelative("emissionShape").enumValueIndex;

            switch (shapeType)
            {
                case EmissionShapeType.Rectangle:
                    EditorGUILayout.PropertyField(emissionSettings.FindPropertyRelative("emissionAreaSize"));
                    break;
                case EmissionShapeType.Circle:
                    EditorGUILayout.PropertyField(emissionSettings.FindPropertyRelative("circleRadius"));
                    break;
                case EmissionShapeType.Line:
                    EditorGUILayout.PropertyField(emissionSettings.FindPropertyRelative("lineLength"));
                    break;
                case EmissionShapeType.Cone:
                    EditorGUILayout.PropertyField(emissionSettings.FindPropertyRelative("coneAngle"));
                    EditorGUILayout.PropertyField(emissionSettings.FindPropertyRelative("coneRange"));
                    EditorGUILayout.PropertyField(emissionSettings.FindPropertyRelative("emissionDirection"));
                    break;
                case EmissionShapeType.Edge:
                    EditorGUILayout.PropertyField(emissionSettings.FindPropertyRelative("emissionAreaSize"));
                    break;
                case EmissionShapeType.Sprite:
                    SerializedProperty shapeSpriteProp = emissionSettings.FindPropertyRelative("shapeSprite");
                    EditorGUILayout.PropertyField(shapeSpriteProp, new GUIContent("Sprite", "Sprite whose visible pixels define the emission silhouette"));

                    if (shapeSpriteProp.objectReferenceValue == null)
                        EditorGUILayout.HelpBox("Assign a Sprite to emit particles from its visible pixels.", MessageType.Warning);

                    EditorGUILayout.PropertyField(emissionSettings.FindPropertyRelative("shapeSpriteSize"), new GUIContent("Size", "UI local size the sprite silhouette is mapped into"));

                    SerializedProperty thresholdProp = emissionSettings.FindPropertyRelative("shapeSpriteThreshold");
                    EditorGUILayout.Slider(thresholdProp, 0f, 1f, new GUIContent("Threshold", "Minimum alpha required for a pixel to emit particles"));
                    break;
                case EmissionShapeType.Image:
                    SerializedProperty shapeImageProp = emissionSettings.FindPropertyRelative("shapeImage");
                    EditorGUILayout.PropertyField(shapeImageProp, new GUIContent("Image", "Scene or prefab Image whose sprite silhouette and RectTransform define emission"));

                    Image shapeImage = shapeImageProp.objectReferenceValue as Image;
                    if (shapeImage == null)
                    {
                        EditorGUILayout.HelpBox("Assign an Image component. Particles emit from its visible pixels in the Image's UI space.", MessageType.Warning);
                    }
                    else if (shapeImage.sprite == null)
                    {
                        EditorGUILayout.HelpBox("The assigned Image has no Sprite.", MessageType.Warning);
                    }
                    else if (shapeImage.type != Image.Type.Simple)
                    {
                        EditorGUILayout.HelpBox("Image Type is not Simple. Emission uses the stretched sprite silhouette as an approximation.", MessageType.Info);
                    }

                    SerializedProperty imageThresholdProp = emissionSettings.FindPropertyRelative("shapeSpriteThreshold");
                    EditorGUILayout.Slider(imageThresholdProp, 0f, 1f, new GUIContent("Threshold", "Minimum alpha required for a pixel to emit particles"));
                    break;
                case EmissionShapeType.Text:
                    SerializedProperty shapeTextProp = emissionSettings.FindPropertyRelative("shapeText");
                    EditorGUILayout.PropertyField(shapeTextProp, new GUIContent("Text", "UI Text or TextMeshProUGUI whose glyph silhouettes define emission"));

                    Graphic shapeText = shapeTextProp.objectReferenceValue as Graphic;
                    if (shapeText == null)
                    {
                        EditorGUILayout.HelpBox("Assign a UI Text or TextMeshProUGUI. Particles emit from the visible letter silhouettes.", MessageType.Warning);
                    }
                    else if (!TextShapeSampler.IsSupported(shapeText))
                    {
                        EditorGUILayout.HelpBox("Assign UI Text or TextMeshPro. Images should use Shape: Image.", MessageType.Warning);
                    }
                    else if (string.IsNullOrEmpty(GetGraphicText(shapeText)))
                    {
                        EditorGUILayout.HelpBox("The assigned text is empty.", MessageType.Warning);
                    }

                    SerializedProperty textThresholdProp = emissionSettings.FindPropertyRelative("shapeSpriteThreshold");
                    EditorGUILayout.Slider(textThresholdProp, 0f, 1f, new GUIContent("Threshold", "Minimum glyph alpha. For SDF / TextMeshPro fonts start around 0.5."));
                    EditorGUILayout.HelpBox("Emit From Edge traces letter outlines. Leave it off to fill the glyphs.", MessageType.Info);
                    break;
            }

            EditorGUILayout.PropertyField(emissionSettings.FindPropertyRelative("emitFromEdge"));
            EditorGUILayout.PropertyField(emissionSettings.FindPropertyRelative("randomizeDirection"));

            if (!emissionSettings.FindPropertyRelative("randomizeDirection").boolValue)
                EditorGUILayout.PropertyField(emissionSettings.FindPropertyRelative("emissionDirection"));

            EditorGUILayout.PropertyField(emissionSettings.FindPropertyRelative("directionVariance"));

        }

        private static string GetGraphicText(Graphic graphic)
        {
            if (graphic is Text uiText)
                return uiText.text;
            if (graphic is TMP_Text tmp)
                return tmp.text;
            return null;
        }

        public void RenderTextureSheetControls(SerializedProperty textureSheetSettings)
        {
            SerializedProperty useTextureSheetAnimationProp = textureSheetSettings.FindPropertyRelative("useTextureSheetAnimation");
            bool isTextureSheetEnabled = useTextureSheetAnimationProp.boolValue;
            EditorGUI.BeginDisabledGroup(!isTextureSheetEnabled);

            SerializedProperty textureSheetProp = textureSheetSettings.FindPropertyRelative("textureSheet");
            EditorGUILayout.PropertyField(textureSheetProp, new GUIContent("Texture Sheet", "Sprite sheet texture containing animation frames"));

            SerializedProperty tilesXProp = textureSheetSettings.FindPropertyRelative("tilesX");
            SerializedProperty tilesYProp = textureSheetSettings.FindPropertyRelative("tilesY");

            EditorGUILayout.BeginHorizontal();

            if (tilesXProp.hasMultipleDifferentValues)
            {
                EditorGUI.showMixedValue = true;
                EditorGUI.BeginChangeCheck();
                int newTilesX = EditorGUILayout.IntField(new GUIContent("Tiles X", "Number of horizontal tiles"), 0);
                if (EditorGUI.EndChangeCheck())
                {
                    tilesXProp.intValue = Mathf.Clamp(newTilesX, 1, 20);
                }
                EditorGUI.showMixedValue = false;
            }
            else
            {
                EditorGUILayout.PropertyField(tilesXProp, new GUIContent("Tiles X", "Number of horizontal tiles"));
                tilesXProp.intValue = Mathf.Clamp(tilesXProp.intValue, 1, 20);
            }

            if (tilesYProp.hasMultipleDifferentValues)
            {
                EditorGUI.showMixedValue = true;
                EditorGUI.BeginChangeCheck();
                int newTilesY = EditorGUILayout.IntField(new GUIContent("Tiles Y", "Number of vertical tiles"), 0);
                if (EditorGUI.EndChangeCheck())
                {
                    tilesYProp.intValue = Mathf.Clamp(newTilesY, 1, 20);
                }
                EditorGUI.showMixedValue = false;
            }
            else
            {
                EditorGUILayout.PropertyField(tilesYProp, new GUIContent("Tiles Y", "Number of vertical tiles"));
                tilesYProp.intValue = Mathf.Clamp(tilesYProp.intValue, 1, 20);
            }

            EditorGUILayout.EndHorizontal();

            int totalTiles = 1;
            if (!tilesXProp.hasMultipleDifferentValues && !tilesYProp.hasMultipleDifferentValues)
            {
                totalTiles = tilesXProp.intValue * tilesYProp.intValue;
            }

            SerializedProperty animationFramesProp = textureSheetSettings.FindPropertyRelative("animationFrames");
            if (animationFramesProp.hasMultipleDifferentValues)
            {
                EditorGUI.showMixedValue = true;
                EditorGUI.BeginChangeCheck();
                int newFrames = EditorGUILayout.IntField(new GUIContent("Animation Frames", "Number of frames to use for animation. Set equal to total tiles for full sheet."), 0);
                if (EditorGUI.EndChangeCheck())
                {
                    int maxFrames = (!tilesXProp.hasMultipleDifferentValues && !tilesYProp.hasMultipleDifferentValues) ? totalTiles : 400;
                    animationFramesProp.intValue = Mathf.Clamp(newFrames, 1, maxFrames);
                }
                EditorGUI.showMixedValue = false;
            }
            else
            {
                EditorGUILayout.PropertyField(animationFramesProp, new GUIContent("Animation Frames", "Number of frames to use for animation. Set equal to total tiles for full sheet."));
                if (!tilesXProp.hasMultipleDifferentValues && !tilesYProp.hasMultipleDifferentValues)
                {
                    animationFramesProp.intValue = Mathf.Clamp(animationFramesProp.intValue, 1, totalTiles);
                }
            }

            SerializedProperty timeModeProp = textureSheetSettings.FindPropertyRelative("timeMode");
            EditorGUILayout.PropertyField(timeModeProp, new GUIContent("Time Mode",
                "FramesPerSecond: Animation plays at fixed FPS rate.\n" +
                "LifetimeNormalized: Animation spreads across the particle's entire lifetime."));

            SerializedProperty animationSpeedProp = textureSheetSettings.FindPropertyRelative("animationSpeed");
            AnimationTimeMode currentTimeMode = (AnimationTimeMode)timeModeProp.enumValueIndex;

            string speedLabel = currentTimeMode == AnimationTimeMode.FramesPerSecond
                ? "Animation Speed (FPS)"
                : "Animation Speed (Cycles)";
            string speedTooltip = currentTimeMode == AnimationTimeMode.FramesPerSecond
                ? "Frames per second. Higher values = faster animation."
                : "Number of animation cycles over the particle's lifetime. 1 = play once, 2 = play twice, etc.";
            float speedMax = currentTimeMode == AnimationTimeMode.FramesPerSecond ? 120f : 10f;

            if (animationSpeedProp.hasMultipleDifferentValues)
            {
                EditorGUI.showMixedValue = true;
                EditorGUI.BeginChangeCheck();
                float newSpeed = EditorGUILayout.FloatField(new GUIContent(speedLabel, speedTooltip), 0f);
                if (EditorGUI.EndChangeCheck())
                {
                    animationSpeedProp.floatValue = Mathf.Clamp(newSpeed, 0.1f, speedMax);
                }
                EditorGUI.showMixedValue = false;
            }
            else
            {
                EditorGUILayout.PropertyField(animationSpeedProp, new GUIContent(speedLabel, speedTooltip));
                animationSpeedProp.floatValue = Mathf.Clamp(animationSpeedProp.floatValue, 0.1f, speedMax);
            }

            SerializedProperty useRandomSingleFrameProp = textureSheetSettings.FindPropertyRelative("useRandomSingleFrame");
            EditorGUILayout.PropertyField(useRandomSingleFrameProp, new GUIContent("Random Single Frame",
                "Each particle picks a random tile and stays on it (no animation). Perfect for confetti effects where each particle should show a different tile."));

            bool isRandomSingleFrame = useRandomSingleFrameProp.boolValue;
            EditorGUI.BeginDisabledGroup(isRandomSingleFrame);

            EditorGUILayout.PropertyField(textureSheetSettings.FindPropertyRelative("randomStartFrame"), new GUIContent("Random Start Frame", "Start animation from a random frame"));
            EditorGUILayout.PropertyField(textureSheetSettings.FindPropertyRelative("loopAnimation"), new GUIContent("Loop Animation", "Loop animation continuously"));

            EditorGUI.EndDisabledGroup();

            SerializedProperty animationCycleProp = textureSheetSettings.FindPropertyRelative("animationCycle");
            EditorGUILayout.PropertyField(animationCycleProp, new GUIContent("Cycle Type", "How to cycle through frames"));

            AnimationCycle cycleType = (AnimationCycle)animationCycleProp.enumValueIndex;

            switch (cycleType)
            {
                case AnimationCycle.SingleRow:
                    SerializedProperty singleRowIndexProp = textureSheetSettings.FindPropertyRelative("singleRowIndex");

                    if (singleRowIndexProp.hasMultipleDifferentValues)
                    {
                        EditorGUI.showMixedValue = true;
                        EditorGUI.BeginChangeCheck();
                        int newRowIndex = EditorGUILayout.IntField(new GUIContent("Row Index", "Which row to use (0-based)"), 0);
                        if (EditorGUI.EndChangeCheck())
                        {
                            int maxRow = (!tilesYProp.hasMultipleDifferentValues) ? (tilesYProp.intValue - 1) : 19;
                            singleRowIndexProp.intValue = Mathf.Clamp(newRowIndex, 0, maxRow);
                        }
                        EditorGUI.showMixedValue = false;
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(singleRowIndexProp, new GUIContent("Row Index", "Which row to use (0-based)"));
                        if (!tilesYProp.hasMultipleDifferentValues)
                        {
                            singleRowIndexProp.intValue = Mathf.Clamp(singleRowIndexProp.intValue, 0, tilesYProp.intValue - 1);
                        }
                    }
                    break;

                case AnimationCycle.SingleColumn:
                    SerializedProperty singleColumnIndexProp = textureSheetSettings.FindPropertyRelative("singleColumnIndex");

                    if (singleColumnIndexProp.hasMultipleDifferentValues)
                    {
                        EditorGUI.showMixedValue = true;
                        EditorGUI.BeginChangeCheck();
                        int newColumnIndex = EditorGUILayout.IntField(new GUIContent("Column Index", "Which column to use (0-based)"), 0);
                        if (EditorGUI.EndChangeCheck())
                        {
                            int maxColumn = (!tilesXProp.hasMultipleDifferentValues) ? (tilesXProp.intValue - 1) : 19;
                            singleColumnIndexProp.intValue = Mathf.Clamp(newColumnIndex, 0, maxColumn);
                        }
                        EditorGUI.showMixedValue = false;
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(singleColumnIndexProp, new GUIContent("Column Index", "Which column to use (0-based)"));
                        if (!tilesXProp.hasMultipleDifferentValues)
                        {
                            singleColumnIndexProp.intValue = Mathf.Clamp(singleColumnIndexProp.intValue, 0, tilesXProp.intValue - 1);
                        }
                    }
                    break;

                case AnimationCycle.Custom:
                    SerializedProperty customStartFrameProp = textureSheetSettings.FindPropertyRelative("customStartFrame");
                    SerializedProperty customEndFrameProp = textureSheetSettings.FindPropertyRelative("customEndFrame");

                    EditorGUILayout.BeginHorizontal();

                    if (customStartFrameProp.hasMultipleDifferentValues)
                    {
                        EditorGUI.showMixedValue = true;
                        EditorGUI.BeginChangeCheck();
                        int newStartFrame = EditorGUILayout.IntField(new GUIContent("Start Frame", "First frame index"), 0);
                        if (EditorGUI.EndChangeCheck())
                        {
                            int maxFrame = (!tilesXProp.hasMultipleDifferentValues && !tilesYProp.hasMultipleDifferentValues) ? (totalTiles - 1) : 99;
                            customStartFrameProp.intValue = Mathf.Clamp(newStartFrame, 0, maxFrame);
                        }
                        EditorGUI.showMixedValue = false;
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(customStartFrameProp, new GUIContent("Start Frame", "First frame index"));
                        if (!tilesXProp.hasMultipleDifferentValues && !tilesYProp.hasMultipleDifferentValues)
                        {
                            customStartFrameProp.intValue = Mathf.Clamp(customStartFrameProp.intValue, 0, totalTiles - 1);
                        }
                    }

                    if (customEndFrameProp.hasMultipleDifferentValues)
                    {
                        EditorGUI.showMixedValue = true;
                        EditorGUI.BeginChangeCheck();
                        int newEndFrame = EditorGUILayout.IntField(new GUIContent("End Frame", "Last frame index"), 0);
                        if (EditorGUI.EndChangeCheck())
                        {
                            int minFrame = (!customStartFrameProp.hasMultipleDifferentValues) ? customStartFrameProp.intValue : 0;
                            int maxFrame = (!tilesXProp.hasMultipleDifferentValues && !tilesYProp.hasMultipleDifferentValues) ? (totalTiles - 1) : 99;
                            customEndFrameProp.intValue = Mathf.Clamp(newEndFrame, minFrame, maxFrame);
                        }
                        EditorGUI.showMixedValue = false;
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(customEndFrameProp, new GUIContent("End Frame", "Last frame index"));
                        if (!tilesXProp.hasMultipleDifferentValues && !tilesYProp.hasMultipleDifferentValues)
                        {
                            customEndFrameProp.intValue = Mathf.Clamp(customEndFrameProp.intValue, customStartFrameProp.intValue, totalTiles - 1);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    break;
            }

            if (textureSheetProp.objectReferenceValue != null &&
                !tilesXProp.hasMultipleDifferentValues &&
                !tilesYProp.hasMultipleDifferentValues)
            {
                Texture2D texture = textureSheetProp.objectReferenceValue as Texture2D;
                if (texture != null)
                {
                    float frameWidth = texture.width / (float)tilesXProp.intValue;
                    float frameHeight = texture.height / (float)tilesYProp.intValue;
                    EditorGUILayout.LabelField($"Frame Size: {frameWidth:F0} x {frameHeight:F0} | Total Tiles: {totalTiles}", EditorStyles.miniLabel);
                }
            }
            else if (tilesXProp.hasMultipleDifferentValues || tilesYProp.hasMultipleDifferentValues)
            {
                EditorGUILayout.LabelField("Frame Size: Mixed Values", EditorStyles.miniLabel);
            }

            EditorGUI.EndDisabledGroup();
        }

        public void RenderNoiseControls(SerializedProperty noiseSettings)
        {
            SerializedProperty useNoiseProp = noiseSettings.FindPropertyRelative("useNoise");
            bool isNoiseEnabled = useNoiseProp.boolValue;
            EditorGUI.BeginDisabledGroup(!isNoiseEnabled);

            SerializedProperty noiseTypeProp = noiseSettings.FindPropertyRelative("noiseType");
            EditorGUILayout.PropertyField(noiseTypeProp, new GUIContent("Noise Type", "Type of noise algorithm to use"));

            SerializedProperty separateAxisNoiseProp = noiseSettings.FindPropertyRelative("separateAxisNoise");
            EditorGUILayout.PropertyField(separateAxisNoiseProp, new GUIContent("Separate Axis Noise", "Use different noise settings for X and Y axes"));

            if (separateAxisNoiseProp.boolValue)
            {
                SerializedProperty noiseStrengthXProp = noiseSettings.FindPropertyRelative("noiseStrengthX");
                if (noiseStrengthXProp.hasMultipleDifferentValues)
                {
                    EditorGUI.showMixedValue = true;
                    EditorGUI.BeginChangeCheck();
                    float newXStrength = EditorGUILayout.FloatField(new GUIContent("X Strength", "Noise strength for X-axis movement"), 0f);
                    if (EditorGUI.EndChangeCheck())
                    {
                        noiseStrengthXProp.floatValue = Mathf.Clamp(newXStrength, 0f, 1000f);
                    }
                    EditorGUI.showMixedValue = false;
                }
                else
                {
                    EditorGUILayout.PropertyField(noiseStrengthXProp, new GUIContent("X Strength", "Noise strength for X-axis movement"));
                    noiseStrengthXProp.floatValue = Mathf.Clamp(noiseStrengthXProp.floatValue, 0f, 1000f);
                }

                SerializedProperty noiseFrequencyXProp = noiseSettings.FindPropertyRelative("noiseFrequencyX");
                if (noiseFrequencyXProp.hasMultipleDifferentValues)
                {
                    EditorGUI.showMixedValue = true;
                    EditorGUI.BeginChangeCheck();
                    float newXFrequency = EditorGUILayout.FloatField(new GUIContent("X Frequency", "Noise frequency for X-axis (higher = more chaotic)"), 0f);
                    if (EditorGUI.EndChangeCheck())
                    {
                        noiseFrequencyXProp.floatValue = Mathf.Clamp(newXFrequency, 0.01f, 10f);
                    }
                    EditorGUI.showMixedValue = false;
                }
                else
                {
                    EditorGUILayout.PropertyField(noiseFrequencyXProp, new GUIContent("X Frequency", "Noise frequency for X-axis (higher = more chaotic)"));
                    noiseFrequencyXProp.floatValue = Mathf.Clamp(noiseFrequencyXProp.floatValue, 0.01f, 10f);
                }

                SerializedProperty noiseStrengthYProp = noiseSettings.FindPropertyRelative("noiseStrengthY");
                if (noiseStrengthYProp.hasMultipleDifferentValues)
                {
                    EditorGUI.showMixedValue = true;
                    EditorGUI.BeginChangeCheck();
                    float newYStrength = EditorGUILayout.FloatField(new GUIContent("Y Strength", "Noise strength for Y-axis movement"), 0f);
                    if (EditorGUI.EndChangeCheck())
                    {
                        noiseStrengthYProp.floatValue = Mathf.Clamp(newYStrength, 0f, 1000f);
                    }
                    EditorGUI.showMixedValue = false;
                }
                else
                {
                    EditorGUILayout.PropertyField(noiseStrengthYProp, new GUIContent("Y Strength", "Noise strength for Y-axis movement"));
                    noiseStrengthYProp.floatValue = Mathf.Clamp(noiseStrengthYProp.floatValue, 0f, 1000f);
                }

                SerializedProperty noiseFrequencyYProp = noiseSettings.FindPropertyRelative("noiseFrequencyY");
                if (noiseFrequencyYProp.hasMultipleDifferentValues)
                {
                    EditorGUI.showMixedValue = true;
                    EditorGUI.BeginChangeCheck();
                    float newYFrequency = EditorGUILayout.FloatField(new GUIContent("Y Frequency", "Noise frequency for Y-axis (higher = more chaotic)"), 0f);
                    if (EditorGUI.EndChangeCheck())
                    {
                        noiseFrequencyYProp.floatValue = Mathf.Clamp(newYFrequency, 0.01f, 10f);
                    }
                    EditorGUI.showMixedValue = false;
                }
                else
                {
                    EditorGUILayout.PropertyField(noiseFrequencyYProp, new GUIContent("Y Frequency", "Noise frequency for Y-axis (higher = more chaotic)"));
                    noiseFrequencyYProp.floatValue = Mathf.Clamp(noiseFrequencyYProp.floatValue, 0.01f, 10f);
                }
            }
            else
            {
                SerializedProperty noiseStrengthProp = noiseSettings.FindPropertyRelative("noiseStrength");
                if (noiseStrengthProp.hasMultipleDifferentValues)
                {
                    EditorGUI.showMixedValue = true;
                    EditorGUI.BeginChangeCheck();
                    float newStrength = EditorGUILayout.FloatField(new GUIContent("Noise Strength", "Overall strength of the noise effect"), 0f);
                    if (EditorGUI.EndChangeCheck())
                    {
                        noiseStrengthProp.floatValue = Mathf.Clamp(newStrength, 0f, 1000f);
                    }
                    EditorGUI.showMixedValue = false;
                }
                else
                {
                    EditorGUILayout.PropertyField(noiseStrengthProp, new GUIContent("Noise Strength", "Overall strength of the noise effect"));
                    noiseStrengthProp.floatValue = Mathf.Clamp(noiseStrengthProp.floatValue, 0f, 1000f);
                }

                SerializedProperty noiseFrequencyProp = noiseSettings.FindPropertyRelative("noiseFrequency");
                if (noiseFrequencyProp.hasMultipleDifferentValues)
                {
                    EditorGUI.showMixedValue = true;
                    EditorGUI.BeginChangeCheck();
                    float newFrequency = EditorGUILayout.FloatField(new GUIContent("Noise Frequency", "Frequency of noise pattern (higher = more chaotic)"), 0f);
                    if (EditorGUI.EndChangeCheck())
                    {
                        noiseFrequencyProp.floatValue = Mathf.Clamp(newFrequency, 0.01f, 10f);
                    }
                    EditorGUI.showMixedValue = false;
                }
                else
                {
                    EditorGUILayout.PropertyField(noiseFrequencyProp, new GUIContent("Noise Frequency", "Frequency of noise pattern (higher = more chaotic)"));
                    noiseFrequencyProp.floatValue = Mathf.Clamp(noiseFrequencyProp.floatValue, 0.01f, 10f);
                }

            }

            SerializedProperty animateNoiseProp = noiseSettings.FindPropertyRelative("animateNoise");
            EditorGUILayout.PropertyField(animateNoiseProp, new GUIContent("Animate Noise", "Make noise pattern change over time"));

            if (animateNoiseProp.boolValue)
            {
                SerializedProperty noiseSpeedProp = noiseSettings.FindPropertyRelative("noiseSpeed");
                if (noiseSpeedProp.hasMultipleDifferentValues)
                {
                    EditorGUI.showMixedValue = true;
                    EditorGUI.BeginChangeCheck();
                    float newSpeed = EditorGUILayout.FloatField(new GUIContent("Animation Speed", "Speed of noise pattern animation"), 0f);
                    if (EditorGUI.EndChangeCheck())
                    {
                        noiseSpeedProp.floatValue = Mathf.Clamp(newSpeed, 0f, 10f);
                    }
                    EditorGUI.showMixedValue = false;
                }
                else
                {
                    EditorGUILayout.PropertyField(noiseSpeedProp, new GUIContent("Animation Speed", "Speed of noise pattern animation"));
                    noiseSpeedProp.floatValue = Mathf.Clamp(noiseSpeedProp.floatValue, 0f, 10f);
                }
            }

            SerializedProperty noiseOffsetProp = noiseSettings.FindPropertyRelative("noiseOffset");
            if (noiseOffsetProp.hasMultipleDifferentValues)
            {
                EditorGUI.showMixedValue = true;
                EditorGUI.BeginChangeCheck();
                Vector2 newOffset = EditorGUILayout.Vector2Field(new GUIContent("Noise Offset", "Offset the noise pattern in 2D space"), Vector2.zero);
                if (EditorGUI.EndChangeCheck())
                {
                    noiseOffsetProp.vector2Value = newOffset;
                }
                EditorGUI.showMixedValue = false;
            }
            else
            {
                EditorGUILayout.PropertyField(noiseOffsetProp, new GUIContent("Noise Offset", "Offset the noise pattern in 2D space"));
            }

            EditorGUI.EndDisabledGroup();
        }

        public void RenderTrailControls(SerializedProperty trailSettings)
        {
            SerializedProperty useTrailsProp = trailSettings.FindPropertyRelative("useTrails");
            EditorGUI.BeginDisabledGroup(!useTrailsProp.boolValue);

            SerializedProperty trailLifetimeProp = trailSettings.FindPropertyRelative("trailLifetime");
            EditorGUI.showMixedValue = trailLifetimeProp.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            float newLifetime = EditorGUILayout.FloatField(new GUIContent("Trail Lifetime", "How long trail points last before disappearing"), trailLifetimeProp.floatValue);
            if (EditorGUI.EndChangeCheck())
                trailLifetimeProp.floatValue = Mathf.Clamp(newLifetime, 0.1f, 10f);
            EditorGUI.showMixedValue = false;

            SerializedProperty trailUpdateIntervalProp = trailSettings.FindPropertyRelative("trailUpdateInterval");
            EditorGUI.showMixedValue = trailUpdateIntervalProp.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            float newInterval = EditorGUILayout.FloatField(new GUIContent("Update Interval", "How often to add new trail points (seconds)"), trailUpdateIntervalProp.floatValue);
            if (EditorGUI.EndChangeCheck())
                trailUpdateIntervalProp.floatValue = Mathf.Clamp(newInterval, 0.01f, 1f);
            EditorGUI.showMixedValue = false;

            SerializedProperty trailWidthProp = trailSettings.FindPropertyRelative("trailWidth");
            EditorGUI.showMixedValue = trailWidthProp.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            float newWidth = EditorGUILayout.FloatField(new GUIContent("Trail Width", "Base width of the trail"), trailWidthProp.floatValue);
            if (EditorGUI.EndChangeCheck())
                trailWidthProp.floatValue = Mathf.Clamp(newWidth, 0.1f, 50f);
            EditorGUI.showMixedValue = false;

            SerializedProperty trailWidthCurveProp = trailSettings.FindPropertyRelative("trailWidthCurve");
            EditorGUI.showMixedValue = trailWidthCurveProp.hasMultipleDifferentValues;
            EditorGUILayout.PropertyField(trailWidthCurveProp, new GUIContent("Width Curve", "How width changes along the trail (0 = start, 1 = end)"));
            EditorGUI.showMixedValue = false;

            SerializedProperty trailColorModeProp = trailSettings.FindPropertyRelative("trailColorMode");
            EditorGUI.showMixedValue = trailColorModeProp.hasMultipleDifferentValues;
            EditorGUILayout.PropertyField(trailColorModeProp, new GUIContent("Color Mode"));
            EditorGUI.showMixedValue = false;

            int colorModeIndex = trailColorModeProp.hasMultipleDifferentValues ? -1 : trailColorModeProp.enumValueIndex;

            if (colorModeIndex == 0)
            {
                EditorGUILayout.PropertyField(trailSettings.FindPropertyRelative("trailColor"), new GUIContent("Color"));
            }
            else if (colorModeIndex == 1)
            {
                EditorGUILayout.PropertyField(trailSettings.FindPropertyRelative("trailColor"), new GUIContent("Color A"));
                EditorGUILayout.PropertyField(trailSettings.FindPropertyRelative("trailColorSecondary"), new GUIContent("Color B"));
            }
            else if (colorModeIndex == 2)
            {
                EditorGUILayout.PropertyField(trailSettings.FindPropertyRelative("trailGradient"), new GUIContent("Gradient"));
            }
            else if (colorModeIndex == 3)
            {
                EditorGUILayout.PropertyField(trailSettings.FindPropertyRelative("trailGradient"), new GUIContent("Gradient A"));
                EditorGUILayout.PropertyField(trailSettings.FindPropertyRelative("trailGradientSecondary"), new GUIContent("Gradient B"));
            }
            else if (colorModeIndex == 5)
            {
                EditorGUILayout.HelpBox("Sprite texture must have Read/Write enabled in import settings.", MessageType.Info);
            }
        }

        public void RenderAttractorControls(SerializedProperty attractorSettings)
        {
            SerializedProperty useAttractorProp = attractorSettings.FindPropertyRelative("useAttractor");
            bool useAttractorEnabled = useAttractorProp.boolValue;

            EditorGUI.BeginDisabledGroup(!useAttractorEnabled);

            SerializedProperty attractorTargetProp = attractorSettings.FindPropertyRelative("attractorTarget");
            EditorGUILayout.PropertyField(attractorTargetProp, new GUIContent("Target Transform", "Target object's transform to attract particles to"));

            SerializedProperty attractorTypeProp = attractorSettings.FindPropertyRelative("attractorType");
            EditorGUILayout.PropertyField(attractorTypeProp, new GUIContent("Attractor Type", "Type of attractor force to apply"));

            AttractorType currentAttractorType = (AttractorType)attractorTypeProp.enumValueIndex;

            SerializedProperty attractorStrengthProp = attractorSettings.FindPropertyRelative("attractorStrength");
            if (attractorStrengthProp.hasMultipleDifferentValues)
            {
                EditorGUI.showMixedValue = true;
                EditorGUI.BeginChangeCheck();
                float newStrength = EditorGUILayout.FloatField(new GUIContent("Strength", "Force strength of the attractor"), 0f);
                if (EditorGUI.EndChangeCheck())
                {
                    attractorStrengthProp.floatValue = Mathf.Clamp(newStrength, 0f, 10000f);
                }
                EditorGUI.showMixedValue = false;
            }
            else
            {
                EditorGUILayout.PropertyField(attractorStrengthProp, new GUIContent("Strength", "Force strength of the attractor"));
                attractorStrengthProp.floatValue = Mathf.Clamp(attractorStrengthProp.floatValue, 0f, 10000f);
            }

            if (currentAttractorType != AttractorType.Direct)
            {
                SerializedProperty attractorRangeProp = attractorSettings.FindPropertyRelative("attractorRange");
                if (attractorRangeProp.hasMultipleDifferentValues)
                {
                    EditorGUI.showMixedValue = true;
                    EditorGUI.BeginChangeCheck();
                    float newRange = EditorGUILayout.FloatField(new GUIContent("Range", "Maximum distance for attractor influence"), 0f);
                    if (EditorGUI.EndChangeCheck())
                    {
                        attractorRangeProp.floatValue = Mathf.Clamp(newRange, 1f, 2000f);
                    }
                    EditorGUI.showMixedValue = false;
                }
                else
                {
                    EditorGUILayout.PropertyField(attractorRangeProp, new GUIContent("Range", "Maximum distance for attractor influence"));
                    attractorRangeProp.floatValue = Mathf.Clamp(attractorRangeProp.floatValue, 1f, 2000f);
                }
            }

            if (currentAttractorType == AttractorType.Orbital || currentAttractorType == AttractorType.Vortex)
            {
                SerializedProperty orbitalSpeedProp = attractorSettings.FindPropertyRelative("orbitalSpeed");
                if (orbitalSpeedProp.hasMultipleDifferentValues)
                {
                    EditorGUI.showMixedValue = true;
                    EditorGUI.BeginChangeCheck();
                    float newOrbitalSpeed = EditorGUILayout.FloatField(new GUIContent("Orbital Speed", "Speed of orbital/rotational motion"), 0f);
                    if (EditorGUI.EndChangeCheck())
                    {
                        orbitalSpeedProp.floatValue = Mathf.Clamp(newOrbitalSpeed, -1000f, 1000f);
                    }
                    EditorGUI.showMixedValue = false;
                }
                else
                {
                    EditorGUILayout.PropertyField(orbitalSpeedProp, new GUIContent("Orbital Speed", "Speed of orbital/rotational motion"));
                    orbitalSpeedProp.floatValue = Mathf.Clamp(orbitalSpeedProp.floatValue, -1000f, 1000f);
                }
            }

            SerializedProperty attractorFollowsMouseProp = attractorSettings.FindPropertyRelative("attractorFollowsMouse");
            SerializedProperty attractorRelativeToEmitterProp = attractorSettings.FindPropertyRelative("attractorRelativeToEmitter");
            SerializedProperty attractorPositionProp = attractorSettings.FindPropertyRelative("attractorPosition");

            if (attractorTargetProp.objectReferenceValue != null)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(attractorFollowsMouseProp, new GUIContent("Follow Mouse", "Disabled when Transform target is set"));
                EditorGUILayout.PropertyField(attractorRelativeToEmitterProp, new GUIContent("Relative to Emitter", "Disabled when Transform target is set"));
                EditorGUILayout.PropertyField(attractorPositionProp, new GUIContent("Manual Position", "Disabled when Transform target is set"));
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                EditorGUILayout.PropertyField(attractorFollowsMouseProp, new GUIContent("Follow Mouse", "Attractor follows mouse cursor position"));

                if (!attractorFollowsMouseProp.boolValue)
                {
                    EditorGUILayout.PropertyField(attractorRelativeToEmitterProp, new GUIContent("Relative to Emitter", "Position relative to emitter vs world position"));

                    string positionLabel = attractorRelativeToEmitterProp.boolValue ? "Relative Position" : "World Position";

                    if (attractorPositionProp.hasMultipleDifferentValues)
                    {
                        EditorGUI.showMixedValue = true;
                        EditorGUI.BeginChangeCheck();
                        Vector2 newPosition = EditorGUILayout.Vector2Field(new GUIContent(positionLabel, "Attractor position in 2D space"), Vector2.zero);
                        if (EditorGUI.EndChangeCheck())
                        {
                            attractorPositionProp.vector2Value = newPosition;
                        }
                        EditorGUI.showMixedValue = false;
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(attractorPositionProp, new GUIContent(positionLabel, "Attractor position in 2D space"));
                    }
                }
            }

            if (currentAttractorType == AttractorType.Direct)
            {
                EditorGUILayout.HelpBox(
                    "Direct needs a Target Transform, or uses Relative/World Position / Follow Mouse when Target is empty. Strength is clamped so it cannot overshoot into NaN.",
                    MessageType.Info);
                EditorGUILayout.PropertyField(attractorSettings.FindPropertyRelative("attractorLerp"),
                    new GUIContent("Lifetime Pull", "Direct only: pull over particle lifetime (0=birth, 1=death). Direct ignores Range and Falloff — it always reaches the target."));
            }
            else
            {
                EditorGUILayout.PropertyField(attractorSettings.FindPropertyRelative("attractorFalloff"),
                    new GUIContent("Falloff Curve", "How force decreases with distance. X=0 at the attractor, X=1 at Range."));
            }

            SerializedProperty attractorKillOnReachProp = attractorSettings.FindPropertyRelative("attractorKillOnReach");
            EditorGUILayout.PropertyField(attractorKillOnReachProp, new GUIContent("Kill on Reach", "Destroy particles when they reach the attractor"));

            if (attractorKillOnReachProp.boolValue)
            {
                EditorGUI.indentLevel++;

                SerializedProperty attractorKillRadiusProp = attractorSettings.FindPropertyRelative("attractorKillRadius");
                if (attractorKillRadiusProp.hasMultipleDifferentValues)
                {
                    EditorGUI.showMixedValue = true;
                    EditorGUI.BeginChangeCheck();
                    float newKillRadius = EditorGUILayout.FloatField(new GUIContent("Kill Radius", "Distance from attractor where particles are destroyed"), 0f);
                    if (EditorGUI.EndChangeCheck())
                    {
                        attractorKillRadiusProp.floatValue = Mathf.Clamp(newKillRadius, 1f, 100f);
                    }
                    EditorGUI.showMixedValue = false;
                }
                else
                {
                    EditorGUILayout.PropertyField(attractorKillRadiusProp, new GUIContent("Kill Radius", "Distance from attractor where particles are destroyed"));
                    attractorKillRadiusProp.floatValue = Mathf.Clamp(attractorKillRadiusProp.floatValue, 1f, 100f);
                }

                SerializedProperty attractorFadeOnApproachProp = attractorSettings.FindPropertyRelative("attractorFadeOnApproach");
                EditorGUILayout.PropertyField(attractorFadeOnApproachProp, new GUIContent("Fade on Approach", "Gradually fade particles as they approach the attractor"));

                if (attractorFadeOnApproachProp.boolValue)
                {
                    SerializedProperty attractorFadeStartDistanceProp = attractorSettings.FindPropertyRelative("attractorFadeStartDistance");
                    if (attractorFadeStartDistanceProp.hasMultipleDifferentValues)
                    {
                        EditorGUI.showMixedValue = true;
                        EditorGUI.BeginChangeCheck();
                        float newFadeStartDistance = EditorGUILayout.FloatField(new GUIContent("Fade Start Distance", "Distance where fading effect begins"), 0f);
                        if (EditorGUI.EndChangeCheck())
                        {
                            float minDistance = (!attractorKillRadiusProp.hasMultipleDifferentValues) ? attractorKillRadiusProp.floatValue : 1f;
                            attractorFadeStartDistanceProp.floatValue = Mathf.Clamp(newFadeStartDistance, minDistance, 200f);
                        }
                        EditorGUI.showMixedValue = false;
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(attractorFadeStartDistanceProp, new GUIContent("Fade Start Distance", "Distance where fading effect begins"));

                        if (!attractorKillRadiusProp.hasMultipleDifferentValues)
                        {
                            if (attractorFadeStartDistanceProp.floatValue < attractorKillRadiusProp.floatValue)
                                attractorFadeStartDistanceProp.floatValue = attractorKillRadiusProp.floatValue;
                        }

                        attractorFadeStartDistanceProp.floatValue = Mathf.Clamp(attractorFadeStartDistanceProp.floatValue,
                            (!attractorKillRadiusProp.hasMultipleDifferentValues) ? attractorKillRadiusProp.floatValue : 1f, 200f);
                    }
                }
                EditorGUI.indentLevel--;
            }

            SerializedProperty useAttractorLifetimeLimitProp = attractorSettings.FindPropertyRelative("useAttractorLifetimeLimit");
            EditorGUILayout.PropertyField(useAttractorLifetimeLimitProp, new GUIContent("Use Lifetime Limit", "Only affect particles within specific lifetime range"));

            if (useAttractorLifetimeLimitProp.boolValue)
            {
                SerializedProperty attractorMinLifetimeProp = attractorSettings.FindPropertyRelative("attractorMinLifetime");
                SerializedProperty attractorMaxLifetimeProp = attractorSettings.FindPropertyRelative("attractorMaxLifetime");

                EditorGUILayout.BeginHorizontal();

                if (attractorMinLifetimeProp.hasMultipleDifferentValues)
                {
                    EditorGUI.showMixedValue = true;
                    EditorGUI.BeginChangeCheck();
                    float newMinLifetime = EditorGUILayout.FloatField(new GUIContent("Min Lifetime", "Minimum normalized lifetime to be affected"), 0f);
                    if (EditorGUI.EndChangeCheck())
                    {
                        attractorMinLifetimeProp.floatValue = Mathf.Clamp(newMinLifetime, 0f, 1f);
                    }
                    EditorGUI.showMixedValue = false;
                }
                else
                {
                    EditorGUILayout.PropertyField(attractorMinLifetimeProp, new GUIContent("Min Lifetime", "Minimum normalized lifetime to be affected"));
                    attractorMinLifetimeProp.floatValue = Mathf.Clamp(attractorMinLifetimeProp.floatValue, 0f, 1f);
                }

                if (attractorMaxLifetimeProp.hasMultipleDifferentValues)
                {
                    EditorGUI.showMixedValue = true;
                    EditorGUI.BeginChangeCheck();
                    float newMaxLifetime = EditorGUILayout.FloatField(new GUIContent("Max Lifetime", "Maximum normalized lifetime to be affected"), 0f);
                    if (EditorGUI.EndChangeCheck())
                    {
                        float minValue = (!attractorMinLifetimeProp.hasMultipleDifferentValues) ? attractorMinLifetimeProp.floatValue : 0f;
                        attractorMaxLifetimeProp.floatValue = Mathf.Clamp(newMaxLifetime, minValue, 1f);
                    }
                    EditorGUI.showMixedValue = false;
                }
                else
                {
                    EditorGUILayout.PropertyField(attractorMaxLifetimeProp, new GUIContent("Max Lifetime", "Maximum normalized lifetime to be affected"));
                    attractorMaxLifetimeProp.floatValue = Mathf.Clamp(attractorMaxLifetimeProp.floatValue, attractorMinLifetimeProp.floatValue, 1f);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.EndDisabledGroup();
        }

        public void RenderVelocityOverLifetimeControls(SerializedProperty velocityOverLifetimeSettings)
        {
            SerializedProperty useVelocityOverLifetimeProp = velocityOverLifetimeSettings.FindPropertyRelative("useVelocityOverLifetime");
            bool useVelocityOverLifetime = useVelocityOverLifetimeProp.boolValue;

            EditorGUI.BeginDisabledGroup(!useVelocityOverLifetime);

            SerializedProperty useLinearVelocityProp = velocityOverLifetimeSettings.FindPropertyRelative("useLinearVelocity");
            useLinearVelocityProp.boolValue = true;
            
            SerializedProperty useOrbitalVelocityProp = velocityOverLifetimeSettings.FindPropertyRelative("useOrbitalVelocity");
            useOrbitalVelocityProp.boolValue = true;

            RenderLinearVelocityDropdown(velocityOverLifetimeSettings);

            SerializedProperty useLocalSpaceProp = velocityOverLifetimeSettings.FindPropertyRelative("useLocalSpace");
            Rect spaceRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            Rect spaceFieldBase = EditorGUI.PrefixLabel(spaceRect, new GUIContent("Space"));
            int oldIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            EditorGUI.BeginChangeCheck();
            int spaceIndex = useLocalSpaceProp.boolValue ? 0 : 1;
            string[] spaceOptions = new string[] { "Local", "World" };
            spaceIndex = EditorGUI.Popup(spaceFieldBase, spaceIndex, spaceOptions);
            if (EditorGUI.EndChangeCheck())
            {
                useLocalSpaceProp.boolValue = (spaceIndex == 0);
            }
            EditorGUI.indentLevel = oldIndent;

            RenderOrbitalVelocityDropdown(velocityOverLifetimeSettings);

            SerializedProperty orbitalCenterProp = velocityOverLifetimeSettings.FindPropertyRelative("orbitalCenter");
            Rect offsetRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            Rect offsetFieldBase = EditorGUI.PrefixLabel(offsetRect, new GUIContent("Offset"));
            
            float offsetHalfWidth = (offsetFieldBase.width - 4f) / 2f;
            Rect offsetXRect = new Rect(offsetFieldBase.x, offsetFieldBase.y, offsetHalfWidth, offsetFieldBase.height);
            Rect offsetYRect = new Rect(offsetFieldBase.x + offsetHalfWidth + 4f, offsetFieldBase.y, offsetHalfWidth, offsetFieldBase.height);

            EditorGUI.indentLevel = 0;
            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 15f;
            
            SerializedProperty centerXProp = orbitalCenterProp.FindPropertyRelative("x");
            SerializedProperty centerYProp = orbitalCenterProp.FindPropertyRelative("y");
            
            EditorGUI.PropertyField(offsetXRect, centerXProp, new GUIContent("X"));
            EditorGUI.PropertyField(offsetYRect, centerYProp, new GUIContent("Y"));
            
            EditorGUIUtility.labelWidth = oldLabelWidth;
            EditorGUI.indentLevel = oldIndent;

            EditorGUI.EndDisabledGroup();
        }

        private void RenderLinearVelocityDropdown(SerializedProperty settingsProp)
        {
            SerializedProperty useCurvesProp = settingsProp.FindPropertyRelative("useLinearVelocityCurves");
            SerializedProperty useRandomProp = settingsProp.FindPropertyRelative("useRandomLinearVelocity");

            int mode = 0; 
            if (useCurvesProp.boolValue && useRandomProp.boolValue) mode = 3;
            else if (useRandomProp.boolValue) mode = 2;
            else if (useCurvesProp.boolValue) mode = 1;
            else mode = 0;

            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            Rect popupRect = new Rect(rect.xMax - 18f, rect.y, 18f, rect.height);
            Rect propRect = new Rect(rect.x, rect.y, rect.width - 18f - 4f, rect.height);

            if (EditorGUI.DropdownButton(popupRect, GUIContent.none, FocusType.Passive, EditorStyles.popup))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Constant"), mode == 0, () => { useCurvesProp.boolValue = false; useRandomProp.boolValue = false; useCurvesProp.serializedObject.ApplyModifiedProperties(); });
                menu.AddItem(new GUIContent("Curve"), mode == 1, () => { useCurvesProp.boolValue = true; useRandomProp.boolValue = false; useCurvesProp.serializedObject.ApplyModifiedProperties(); });
                menu.AddItem(new GUIContent("Random Between Two Constants"), mode == 2, () => { useCurvesProp.boolValue = false; useRandomProp.boolValue = true; useCurvesProp.serializedObject.ApplyModifiedProperties(); });
                menu.AddItem(new GUIContent("Random Between Two Curves"), mode == 3, () => { useCurvesProp.boolValue = true; useRandomProp.boolValue = true; useCurvesProp.serializedObject.ApplyModifiedProperties(); });
                menu.DropDown(popupRect);
            }

            int oldIndent = EditorGUI.indentLevel;

            if (mode == 0)
            {
                Rect fieldBase = EditorGUI.PrefixLabel(propRect, new GUIContent("Linear"));
                float halfWidth = (fieldBase.width - 4f) / 2f;
                Rect xRect = new Rect(fieldBase.x, fieldBase.y, halfWidth, fieldBase.height);
                Rect yRect = new Rect(fieldBase.x + halfWidth + 4f, fieldBase.y, halfWidth, fieldBase.height);

                EditorGUI.indentLevel = 0;
                float oldLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 15f;
                
                SerializedProperty linearVelocityProp = settingsProp.FindPropertyRelative("linearVelocity");
                EditorGUI.PropertyField(xRect, linearVelocityProp.FindPropertyRelative("x"), new GUIContent("X"));
                EditorGUI.PropertyField(yRect, linearVelocityProp.FindPropertyRelative("y"), new GUIContent("Y"));
                
                EditorGUIUtility.labelWidth = oldLabelWidth;
                EditorGUI.indentLevel = oldIndent;
            }
            else if (mode == 1)
            {
                Rect fieldBase = EditorGUI.PrefixLabel(propRect, new GUIContent("Linear"));
                float halfWidth = (fieldBase.width - 4f) / 2f;
                Rect xRect = new Rect(fieldBase.x, fieldBase.y, halfWidth, fieldBase.height);
                Rect yRect = new Rect(fieldBase.x + halfWidth + 4f, fieldBase.y, halfWidth, fieldBase.height);

                EditorGUI.indentLevel = 0;
                float oldLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 15f;
                
                EditorGUI.PropertyField(xRect, settingsProp.FindPropertyRelative("linearVelocityX"), new GUIContent("X"));
                EditorGUI.PropertyField(yRect, settingsProp.FindPropertyRelative("linearVelocityY"), new GUIContent("Y"));
                
                EditorGUIUtility.labelWidth = oldLabelWidth;
                EditorGUI.indentLevel = oldIndent;
            }
            else if (mode == 2)
            {
                Rect fieldBaseMin = EditorGUI.PrefixLabel(propRect, new GUIContent("Linear Min"));
                float halfWidth = (fieldBaseMin.width - 4f) / 2f;
                Rect xRectMin = new Rect(fieldBaseMin.x, fieldBaseMin.y, halfWidth, fieldBaseMin.height);
                Rect yRectMin = new Rect(fieldBaseMin.x + halfWidth + 4f, fieldBaseMin.y, halfWidth, fieldBaseMin.height);

                EditorGUI.indentLevel = 0;
                float oldLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 15f;
                
                SerializedProperty linearMinProp = settingsProp.FindPropertyRelative("linearVelocityMin");
                EditorGUI.PropertyField(xRectMin, linearMinProp.FindPropertyRelative("x"), new GUIContent("X"));
                EditorGUI.PropertyField(yRectMin, linearMinProp.FindPropertyRelative("y"), new GUIContent("Y"));
                
                EditorGUIUtility.labelWidth = oldLabelWidth;
                EditorGUI.indentLevel = oldIndent;

                Rect maxRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
                Rect fieldBaseMax = EditorGUI.PrefixLabel(maxRect, new GUIContent("Linear Max"));
                
                Rect xRectMax = new Rect(fieldBaseMax.x, fieldBaseMax.y, halfWidth, fieldBaseMax.height);
                Rect yRectMax = new Rect(fieldBaseMax.x + halfWidth + 4f, fieldBaseMax.y, halfWidth, fieldBaseMax.height);

                EditorGUI.indentLevel = 0;
                EditorGUIUtility.labelWidth = 15f;
                
                SerializedProperty linearMaxProp = settingsProp.FindPropertyRelative("linearVelocityMax");
                EditorGUI.PropertyField(xRectMax, linearMaxProp.FindPropertyRelative("x"), new GUIContent("X"));
                EditorGUI.PropertyField(yRectMax, linearMaxProp.FindPropertyRelative("y"), new GUIContent("Y"));
                
                EditorGUIUtility.labelWidth = oldLabelWidth;
                EditorGUI.indentLevel = oldIndent;
            }
            else if (mode == 3)
            {
                Rect minFieldBase = EditorGUI.PrefixLabel(propRect, new GUIContent("Linear Min"));
                
                float halfWidth = (minFieldBase.width - 4f) / 2f;
                Rect xRectMin = new Rect(minFieldBase.x, minFieldBase.y, halfWidth, minFieldBase.height);
                Rect yRectMin = new Rect(minFieldBase.x + halfWidth + 4f, minFieldBase.y, halfWidth, minFieldBase.height);

                EditorGUI.indentLevel = 0;
                float oldLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 15f;
                
                EditorGUI.PropertyField(xRectMin, settingsProp.FindPropertyRelative("linearVelocityXMinCurve"), new GUIContent("X"));
                EditorGUI.PropertyField(yRectMin, settingsProp.FindPropertyRelative("linearVelocityYMinCurve"), new GUIContent("Y"));
                
                EditorGUIUtility.labelWidth = oldLabelWidth;
                EditorGUI.indentLevel = oldIndent;

                Rect maxRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
                Rect maxFieldBase = EditorGUI.PrefixLabel(maxRect, new GUIContent("Linear Max"));
                
                Rect xRectMax = new Rect(maxFieldBase.x, maxFieldBase.y, halfWidth, maxFieldBase.height);
                Rect yRectMax = new Rect(maxFieldBase.x + halfWidth + 4f, maxFieldBase.y, halfWidth, maxFieldBase.height);

                EditorGUI.indentLevel = 0;
                EditorGUIUtility.labelWidth = 15f;
                
                EditorGUI.PropertyField(xRectMax, settingsProp.FindPropertyRelative("linearVelocityX"), new GUIContent("X"));
                EditorGUI.PropertyField(yRectMax, settingsProp.FindPropertyRelative("linearVelocityY"), new GUIContent("Y"));
                
                EditorGUIUtility.labelWidth = oldLabelWidth;
                EditorGUI.indentLevel = oldIndent;
            }
        }

        private void RenderOrbitalVelocityDropdown(SerializedProperty settingsProp)
        {
            SerializedProperty useCurvesProp = settingsProp.FindPropertyRelative("useOrbitalVelocityCurve");
            SerializedProperty useRandomProp = settingsProp.FindPropertyRelative("useRandomOrbitalVelocity");

            int mode = 0; 
            if (useCurvesProp.boolValue && useRandomProp.boolValue) mode = 3;
            else if (useRandomProp.boolValue) mode = 2;
            else if (useCurvesProp.boolValue) mode = 1;
            else mode = 0;

            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            Rect popupRect = new Rect(rect.xMax - 18f, rect.y, 18f, rect.height);
            Rect propRect = new Rect(rect.x, rect.y, rect.width - 18f - 4f, rect.height);

            if (EditorGUI.DropdownButton(popupRect, GUIContent.none, FocusType.Passive, EditorStyles.popup))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Constant"), mode == 0, () => { useCurvesProp.boolValue = false; useRandomProp.boolValue = false; useCurvesProp.serializedObject.ApplyModifiedProperties(); });
                menu.AddItem(new GUIContent("Curve"), mode == 1, () => { useCurvesProp.boolValue = true; useRandomProp.boolValue = false; useCurvesProp.serializedObject.ApplyModifiedProperties(); });
                menu.AddItem(new GUIContent("Random Between Two Constants"), mode == 2, () => { useCurvesProp.boolValue = false; useRandomProp.boolValue = true; useCurvesProp.serializedObject.ApplyModifiedProperties(); });
                menu.AddItem(new GUIContent("Random Between Two Curves"), mode == 3, () => { useCurvesProp.boolValue = true; useRandomProp.boolValue = true; useCurvesProp.serializedObject.ApplyModifiedProperties(); });
                menu.DropDown(popupRect);
            }

            int oldIndent = EditorGUI.indentLevel;

            if (mode == 0)
            {
                Rect fieldBase = EditorGUI.PrefixLabel(propRect, new GUIContent("Orbital"));
                EditorGUI.indentLevel = 0;
                float oldLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 15f;
                
                SerializedProperty prop = settingsProp.FindPropertyRelative("orbitalVelocity");
                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(fieldBase, prop, new GUIContent("Z"));
                if (EditorGUI.EndChangeCheck()) prop.floatValue = Mathf.Clamp(prop.floatValue, -1000f, 1000f);
                
                EditorGUIUtility.labelWidth = oldLabelWidth;
                EditorGUI.indentLevel = oldIndent;
            }
            else if (mode == 1)
            {
                Rect fieldBase = EditorGUI.PrefixLabel(propRect, new GUIContent("Orbital Curve"));
                EditorGUI.indentLevel = 0;
                float oldLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 15f;
                
                EditorGUI.PropertyField(fieldBase, settingsProp.FindPropertyRelative("orbitalVelocityCurve"), new GUIContent("Z"));
                
                EditorGUIUtility.labelWidth = oldLabelWidth;
                EditorGUI.indentLevel = oldIndent;
            }
            else if (mode == 2)
            {
                Rect fieldBase = EditorGUI.PrefixLabel(propRect, new GUIContent("Orbital Random"));
                float halfWidth = (fieldBase.width - 4f) / 2f;
                Rect minRectField = new Rect(fieldBase.x, fieldBase.y, halfWidth, fieldBase.height);
                Rect maxRectField = new Rect(fieldBase.x + halfWidth + 4f, fieldBase.y, halfWidth, fieldBase.height);

                EditorGUI.indentLevel = 0;
                float oldLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 28f;
                
                SerializedProperty minProp = settingsProp.FindPropertyRelative("orbitalVelocityMin");
                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(minRectField, minProp, new GUIContent("Min"));
                if (EditorGUI.EndChangeCheck()) minProp.floatValue = Mathf.Clamp(minProp.floatValue, -1000f, 1000f);
                
                SerializedProperty maxProp = settingsProp.FindPropertyRelative("orbitalVelocityMax");
                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(maxRectField, maxProp, new GUIContent("Max"));
                if (EditorGUI.EndChangeCheck()) maxProp.floatValue = Mathf.Clamp(maxProp.floatValue, -1000f, 1000f);
                
                EditorGUIUtility.labelWidth = oldLabelWidth;
                EditorGUI.indentLevel = oldIndent;
            }
            else if (mode == 3)
            {
                Rect fieldBase = EditorGUI.PrefixLabel(propRect, new GUIContent("Orbital Curves"));
                float halfWidth = (fieldBase.width - 4f) / 2f;
                Rect minRectField = new Rect(fieldBase.x, fieldBase.y, halfWidth, fieldBase.height);
                Rect maxRectField = new Rect(fieldBase.x + halfWidth + 4f, fieldBase.y, halfWidth, fieldBase.height);

                EditorGUI.indentLevel = 0;
                float oldLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 28f;
                
                EditorGUI.PropertyField(minRectField, settingsProp.FindPropertyRelative("orbitalVelocityMinCurve"), new GUIContent("Min"));
                EditorGUI.PropertyField(maxRectField, settingsProp.FindPropertyRelative("orbitalVelocityCurve"), new GUIContent("Max"));
                
                EditorGUIUtility.labelWidth = oldLabelWidth;
                EditorGUI.indentLevel = oldIndent;
            }
        }
        
        public void RenderLimitVelocityOverLifetimeControls(SerializedProperty limitVelocitySettings)
        {
            SerializedProperty useLimitVelocityOverLifetimeProp = limitVelocitySettings.FindPropertyRelative("useLimitVelocityOverLifetime");
            bool isEnabled = useLimitVelocityOverLifetimeProp.boolValue;

            EditorGUI.BeginDisabledGroup(!isEnabled);

            SerializedProperty separateAxesProp = limitVelocitySettings.FindPropertyRelative("separateAxes");
            EditorGUILayout.PropertyField(separateAxesProp, new GUIContent("Separate Axes"));

            if (!separateAxesProp.boolValue)
            {
                RenderLimitMinMaxProperty(limitVelocitySettings, "limitMode", "limit", "limitMin", "limitCurve", "limitMinCurve", "Speed");
            }
            else
            {
                RenderLimitMinMaxProperty(limitVelocitySettings, "limitXMode", "limitX", "limitXMin", "limitXCurve", "limitXMinCurve", "Speed X");
                RenderLimitMinMaxProperty(limitVelocitySettings, "limitYMode", "limitY", "limitYMin", "limitYCurve", "limitYMinCurve", "Speed Y");
                RenderLimitMinMaxProperty(limitVelocitySettings, "limitZMode", "limitZ", "limitZMin", "limitZCurve", "limitZMinCurve", "Speed Z");
            }

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(limitVelocitySettings.FindPropertyRelative("dampen"), new GUIContent("Dampen"));
            EditorGUI.indentLevel--;

            RenderLimitMinMaxProperty(limitVelocitySettings, "dragMode", "drag", "dragMin", "dragCurve", "dragMinCurve", "Drag");

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(limitVelocitySettings.FindPropertyRelative("multiplyDragBySize"), new GUIContent("Multiply by Size"));
            EditorGUILayout.PropertyField(limitVelocitySettings.FindPropertyRelative("multiplyDragByVelocity"), new GUIContent("Multiply by Velocity"));
            EditorGUI.indentLevel--;

            EditorGUI.EndDisabledGroup();
        }


        private void RenderLimitMinMaxProperty(SerializedProperty settingsProp, string modeName, string constName, string minName, string curveName, string minCurveName, string label)
        {
            SerializedProperty modeProp = settingsProp.FindPropertyRelative(modeName);
            int mode = modeProp.intValue;

            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            Rect popupRect = new Rect(rect.xMax - 18f, rect.y, 18f, rect.height);
            Rect propRect = new Rect(rect.x, rect.y, rect.width - 18f - 4f, rect.height);

            if (EditorGUI.DropdownButton(popupRect, GUIContent.none, FocusType.Passive, EditorStyles.popup))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Constant"), mode == 0, () => { modeProp.intValue = 0; modeProp.serializedObject.ApplyModifiedProperties(); });
                menu.AddItem(new GUIContent("Curve"), mode == 1, () => { modeProp.intValue = 1; modeProp.serializedObject.ApplyModifiedProperties(); });
                menu.AddItem(new GUIContent("Random Between Two Constants"), mode == 2, () => { modeProp.intValue = 2; modeProp.serializedObject.ApplyModifiedProperties(); });
                menu.AddItem(new GUIContent("Random Between Two Curves"), mode == 3, () => { modeProp.intValue = 3; modeProp.serializedObject.ApplyModifiedProperties(); });
                menu.DropDown(popupRect);
            }

            int oldIndent = EditorGUI.indentLevel;

            if (mode == 0)
            {
                Rect fieldBase = EditorGUI.PrefixLabel(propRect, new GUIContent(label));
                EditorGUI.indentLevel = 0;
                EditorGUI.PropertyField(fieldBase, settingsProp.FindPropertyRelative(constName), GUIContent.none);
                EditorGUI.indentLevel = oldIndent;
            }
            else if (mode == 1)
            {
                Rect fieldBase = EditorGUI.PrefixLabel(propRect, new GUIContent(label));
                EditorGUI.indentLevel = 0;
                EditorGUI.PropertyField(fieldBase, settingsProp.FindPropertyRelative(curveName), GUIContent.none);
                EditorGUI.indentLevel = oldIndent;
            }
            else if (mode == 2)
            {
                Rect fieldBase = EditorGUI.PrefixLabel(propRect, new GUIContent(label));
                float halfWidth = (fieldBase.width - 4f) / 2f;
                Rect minRectField = new Rect(fieldBase.x, fieldBase.y, halfWidth, fieldBase.height);
                Rect maxRectField = new Rect(fieldBase.x + halfWidth + 4f, fieldBase.y, halfWidth, fieldBase.height);

                EditorGUI.indentLevel = 0;
                float oldLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 28f;
                EditorGUI.PropertyField(minRectField, settingsProp.FindPropertyRelative(minName), new GUIContent("Min"));
                EditorGUI.PropertyField(maxRectField, settingsProp.FindPropertyRelative(constName), new GUIContent("Max"));
                EditorGUIUtility.labelWidth = oldLabelWidth;
                EditorGUI.indentLevel = oldIndent;
            }
            else if (mode == 3)
            {
                Rect fieldBase = EditorGUI.PrefixLabel(propRect, new GUIContent(label));
                float halfWidth = (fieldBase.width - 4f) / 2f;
                Rect minRectField = new Rect(fieldBase.x, fieldBase.y, halfWidth, fieldBase.height);
                Rect maxRectField = new Rect(fieldBase.x + halfWidth + 4f, fieldBase.y, halfWidth, fieldBase.height);

                EditorGUI.indentLevel = 0;
                float oldLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 28f;
                EditorGUI.PropertyField(minRectField, settingsProp.FindPropertyRelative(minCurveName), new GUIContent("Min"));
                EditorGUI.PropertyField(maxRectField, settingsProp.FindPropertyRelative(curveName), new GUIContent("Max"));
                EditorGUIUtility.labelWidth = oldLabelWidth;
                EditorGUI.indentLevel = oldIndent;
            }
        }

        #endregion

        #region Force Over Lifetime Controls


        public void RenderForceOverLifetimeControls(SerializedProperty forceSettings)
        {
            SerializedProperty useForceProp = forceSettings.FindPropertyRelative("useForceOverLifetime");
            bool isEnabled = useForceProp.boolValue;

            EditorGUI.BeginDisabledGroup(!isEnabled);

            RenderForceXYDropdown(forceSettings);

            SerializedProperty useLocalSpaceProp = forceSettings.FindPropertyRelative("useLocalSpace");
            int spaceIndex = useLocalSpaceProp.boolValue ? 0 : 1;
            int newSpaceIndex = EditorGUILayout.Popup(new GUIContent("Space", "World or Local space for force direction"), spaceIndex, new string[] { "Local", "World" });
            if (newSpaceIndex != spaceIndex)
                useLocalSpaceProp.boolValue = (newSpaceIndex == 0);

            EditorGUI.EndDisabledGroup();
        }


        private void RenderForceXYDropdown(SerializedProperty settingsProp)
        {
            SerializedProperty modeProp = settingsProp.FindPropertyRelative("mode");
            int mode = modeProp.intValue;

            if (mode == 0)
            {
                Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
                Rect popupRect = new Rect(rect.xMax - 18f, rect.y, 18f, rect.height);
                Rect propRect = new Rect(rect.x, rect.y, rect.width - 18f - 4f, rect.height);

                DrawForceModeDropdown(popupRect, modeProp, mode);

                Rect fieldBase = EditorGUI.PrefixLabel(propRect, new GUIContent("Force"));
                float halfWidth = (fieldBase.width - 4f) / 2f;
                Rect xRect = new Rect(fieldBase.x, fieldBase.y, halfWidth, fieldBase.height);
                Rect yRect = new Rect(fieldBase.x + halfWidth + 4f, fieldBase.y, halfWidth, fieldBase.height);

                int oldIndent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                float oldLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 15f;

                EditorGUI.PropertyField(xRect, settingsProp.FindPropertyRelative("x"), new GUIContent("X"));
                EditorGUI.PropertyField(yRect, settingsProp.FindPropertyRelative("y"), new GUIContent("Y"));

                EditorGUIUtility.labelWidth = oldLabelWidth;
                EditorGUI.indentLevel = oldIndent;
            }

            else if (mode == 1)
            {
                Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
                Rect popupRect = new Rect(rect.xMax - 18f, rect.y, 18f, rect.height);
                Rect propRect = new Rect(rect.x, rect.y, rect.width - 18f - 4f, rect.height);

                DrawForceModeDropdown(popupRect, modeProp, mode);

                Rect fieldBase = EditorGUI.PrefixLabel(propRect, new GUIContent("Force"));
                float halfWidth = (fieldBase.width - 4f) / 2f;
                Rect xRect = new Rect(fieldBase.x, fieldBase.y, halfWidth, fieldBase.height);
                Rect yRect = new Rect(fieldBase.x + halfWidth + 4f, fieldBase.y, halfWidth, fieldBase.height);

                int oldIndent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                float oldLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 15f;

                EditorGUI.PropertyField(xRect, settingsProp.FindPropertyRelative("xCurve"), new GUIContent("X"));
                EditorGUI.PropertyField(yRect, settingsProp.FindPropertyRelative("yCurve"), new GUIContent("Y"));

                EditorGUIUtility.labelWidth = oldLabelWidth;
                EditorGUI.indentLevel = oldIndent;
            }

            else if (mode == 2)
            {
                Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
                Rect popupRect = new Rect(rect.xMax - 18f, rect.y, 18f, rect.height);
                Rect propRect = new Rect(rect.x, rect.y, rect.width - 18f - 4f, rect.height);

                DrawForceModeDropdown(popupRect, modeProp, mode);

                Rect fieldBaseMin = EditorGUI.PrefixLabel(propRect, new GUIContent("Force Min"));
                float halfWidth = (fieldBaseMin.width - 4f) / 2f;
                Rect xRectMin = new Rect(fieldBaseMin.x, fieldBaseMin.y, halfWidth, fieldBaseMin.height);
                Rect yRectMin = new Rect(fieldBaseMin.x + halfWidth + 4f, fieldBaseMin.y, halfWidth, fieldBaseMin.height);

                int oldIndent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                float oldLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 15f;

                EditorGUI.PropertyField(xRectMin, settingsProp.FindPropertyRelative("xMin"), new GUIContent("X"));
                EditorGUI.PropertyField(yRectMin, settingsProp.FindPropertyRelative("yMin"), new GUIContent("Y"));

                EditorGUIUtility.labelWidth = oldLabelWidth;
                EditorGUI.indentLevel = oldIndent;

                Rect maxRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
                Rect fieldBaseMax = EditorGUI.PrefixLabel(maxRect, new GUIContent("Force Max"));

                Rect xRectMax = new Rect(fieldBaseMax.x, fieldBaseMax.y, halfWidth, fieldBaseMax.height);
                Rect yRectMax = new Rect(fieldBaseMax.x + halfWidth + 4f, fieldBaseMax.y, halfWidth, fieldBaseMax.height);

                EditorGUI.indentLevel = 0;
                EditorGUIUtility.labelWidth = 15f;

                EditorGUI.PropertyField(xRectMax, settingsProp.FindPropertyRelative("x"), new GUIContent("X"));
                EditorGUI.PropertyField(yRectMax, settingsProp.FindPropertyRelative("y"), new GUIContent("Y"));

                EditorGUIUtility.labelWidth = oldLabelWidth;
                EditorGUI.indentLevel = oldIndent;
            }

            else if (mode == 3)
            {
                Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
                Rect popupRect = new Rect(rect.xMax - 18f, rect.y, 18f, rect.height);
                Rect propRect = new Rect(rect.x, rect.y, rect.width - 18f - 4f, rect.height);

                DrawForceModeDropdown(popupRect, modeProp, mode);

                Rect minFieldBase = EditorGUI.PrefixLabel(propRect, new GUIContent("Force Min"));
                float halfWidth = (minFieldBase.width - 4f) / 2f;
                Rect xRectMin = new Rect(minFieldBase.x, minFieldBase.y, halfWidth, minFieldBase.height);
                Rect yRectMin = new Rect(minFieldBase.x + halfWidth + 4f, minFieldBase.y, halfWidth, minFieldBase.height);

                int oldIndent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                float oldLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 15f;

                EditorGUI.PropertyField(xRectMin, settingsProp.FindPropertyRelative("xMinCurve"), new GUIContent("X"));
                EditorGUI.PropertyField(yRectMin, settingsProp.FindPropertyRelative("yMinCurve"), new GUIContent("Y"));

                EditorGUIUtility.labelWidth = oldLabelWidth;
                EditorGUI.indentLevel = oldIndent;

                Rect maxRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
                Rect maxFieldBase = EditorGUI.PrefixLabel(maxRect, new GUIContent("Force Max"));

                Rect xRectMax = new Rect(maxFieldBase.x, maxFieldBase.y, halfWidth, maxFieldBase.height);
                Rect yRectMax = new Rect(maxFieldBase.x + halfWidth + 4f, maxFieldBase.y, halfWidth, maxFieldBase.height);

                EditorGUI.indentLevel = 0;
                EditorGUIUtility.labelWidth = 15f;

                EditorGUI.PropertyField(xRectMax, settingsProp.FindPropertyRelative("xCurve"), new GUIContent("X"));
                EditorGUI.PropertyField(yRectMax, settingsProp.FindPropertyRelative("yCurve"), new GUIContent("Y"));

                EditorGUIUtility.labelWidth = oldLabelWidth;
                EditorGUI.indentLevel = oldIndent;
            }
        }

        private void DrawForceModeDropdown(Rect popupRect, SerializedProperty modeProp, int currentMode)
        {
            if (EditorGUI.DropdownButton(popupRect, GUIContent.none, FocusType.Passive, EditorStyles.popup))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Constant"), currentMode == 0, () =>
                {
                    modeProp.intValue = 0;
                    modeProp.serializedObject.ApplyModifiedProperties();
                });
                menu.AddItem(new GUIContent("Curve"), currentMode == 1, () =>
                {
                    modeProp.intValue = 1;
                    modeProp.serializedObject.ApplyModifiedProperties();
                });
                menu.AddItem(new GUIContent("Random Between Two Constants"), currentMode == 2, () =>
                {
                    modeProp.intValue = 2;
                    modeProp.serializedObject.ApplyModifiedProperties();
                });
                menu.AddItem(new GUIContent("Random Between Two Curves"), currentMode == 3, () =>
                {
                    modeProp.intValue = 3;
                    modeProp.serializedObject.ApplyModifiedProperties();
                });
                menu.DropDown(popupRect);
            }
        }

        public void RenderCollisionControls(SerializedProperty collisionSettings)
        {
            EditorGUILayout.PropertyField(collisionSettings.FindPropertyRelative("useBounds"), new GUIContent("Bounds", "Keep particles inside a RectTransform (own rect if empty)"));
            if (collisionSettings.FindPropertyRelative("useBounds").boolValue)
                EditorGUILayout.PropertyField(collisionSettings.FindPropertyRelative("boundsTarget"), new GUIContent("Bounds Target", "Leave empty to use this particle system's RectTransform"));

            EditorGUILayout.PropertyField(collisionSettings.FindPropertyRelative("obstacles"), new GUIContent("Obstacles", "UI RectTransforms that particles bounce off from the outside"));

            EditorGUILayout.Slider(collisionSettings.FindPropertyRelative("bounce"), 0f, 1f, new GUIContent("Bounce", "How strongly particles rebound (1 = fully elastic)"));
            EditorGUILayout.Slider(collisionSettings.FindPropertyRelative("dampen"), 0f, 1f, new GUIContent("Dampen", "Velocity lost on each collision"));
            EditorGUILayout.Slider(collisionSettings.FindPropertyRelative("friction"), 0f, 1f, new GUIContent("Friction", "Slows the tangent velocity on impact"));
            EditorGUILayout.Slider(collisionSettings.FindPropertyRelative("lifetimeLoss"), 0f, 1f, new GUIContent("Lifetime Loss", "Fraction of remaining lifetime removed on collision"));
            EditorGUILayout.Slider(collisionSettings.FindPropertyRelative("radiusScale"), 0f, 2f, new GUIContent("Radius Scale", "Collision radius = particle size × 0.5 × this. 1 = bounce at the visual edge. Raise it if particles sink into obstacles."));
            EditorGUILayout.Slider(collisionSettings.FindPropertyRelative("bounceSpread"), 0f, 45f, new GUIContent("Bounce Spread", "Random extra degrees added to the bounce angle"));

            EditorGUILayout.PropertyField(collisionSettings.FindPropertyRelative("killOnCollision"), new GUIContent("Kill On Collision", "Destroy the particle on the first hit"));
            EditorGUILayout.PropertyField(collisionSettings.FindPropertyRelative("maxCollisionCount"), new GUIContent("Max Collision Count", "Kill after this many hits. 0 = unlimited"));
            EditorGUILayout.PropertyField(collisionSettings.FindPropertyRelative("minKillSpeed"), new GUIContent("Min Kill Speed", "Kill if speed after bounce is below this. 0 = off"));
        }

        public void RenderInheritVelocityControls(SerializedProperty inheritSettings)
        {
            EditorGUILayout.PropertyField(inheritSettings.FindPropertyRelative("mode"),
                new GUIContent("Mode", "Initial: add emitter velocity when the particle is born. Current: keep World-space particles moving with the emitter each frame."));
            EditorGUILayout.PropertyField(inheritSettings.FindPropertyRelative("multiplier"),
                new GUIContent("Multiplier", "How much of the emitter velocity/motion is applied (1 = 100%)"));

            EditorGUILayout.HelpBox(
                "Initial is best for swipe/spray (coins, sparks). Current is for World simulation space so particles travel with a moving UI element. In Local space particles already follow the emitter, so Current has no extra effect.",
                MessageType.Info);
        }

        #endregion
    }
}
#endif
