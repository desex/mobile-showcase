#if UNITY_EDITOR
using TierStudios.UIParticleSystem.Enums;
using TierStudios.UIParticleSystem.Settings;
using UnityEditor;
using UnityEngine;

namespace TierStudios.UIParticleSystem.Editor
{
    public class UIParticleSystemEditorGUI
    {
        private Core.UIParticleSystem particleSystem;
        private UIParticleSystemEditorStyles styles;
        private UIParticleSystemEditorControls controls;
        private UIParticleSystemEditorUtils utils;

        private bool isMultiObjectEditing;
        private Core.UIParticleSystem[] multiSelectedSystems;

        private string GetSessionKey(string name)
        {
            return "UIParticleSystem_" + name + "_" + (particleSystem != null ? particleSystem.GetInstanceID().ToString() : "0");
        }

        private bool showColorSettings
        {
            get => SessionState.GetBool(GetSessionKey("showColorSettings"), false);
            set => SessionState.SetBool(GetSessionKey("showColorSettings"), value);
        }
        private bool showColorBySpeedSettings
        {
            get => SessionState.GetBool(GetSessionKey("showColorBySpeedSettings"), false);
            set => SessionState.SetBool(GetSessionKey("showColorBySpeedSettings"), value);
        }
        private bool showSizeSettings
        {
            get => SessionState.GetBool(GetSessionKey("showSizeSettings"), false);
            set => SessionState.SetBool(GetSessionKey("showSizeSettings"), value);
        }
        private bool showSizeBySpeedSettings
        {
            get => SessionState.GetBool(GetSessionKey("showSizeBySpeedSettings"), false);
            set => SessionState.SetBool(GetSessionKey("showSizeBySpeedSettings"), value);
        }
        private bool showEmissionSettings
        {
            get => SessionState.GetBool(GetSessionKey("showEmissionSettings"), false);
            set => SessionState.SetBool(GetSessionKey("showEmissionSettings"), value);
        }
        private bool showShapeSettings
        {
            get => SessionState.GetBool(GetSessionKey("showShapeSettings"), false);
            set => SessionState.SetBool(GetSessionKey("showShapeSettings"), value);
        }
        private bool showRotationSettings
        {
            get => SessionState.GetBool(GetSessionKey("showRotationSettings"), false);
            set => SessionState.SetBool(GetSessionKey("showRotationSettings"), value);
        }
        private bool showTextureSheetSettings
        {
            get => SessionState.GetBool(GetSessionKey("showTextureSheetSettings"), false);
            set => SessionState.SetBool(GetSessionKey("showTextureSheetSettings"), value);
        }
        private bool showNoiseSettings
        {
            get => SessionState.GetBool(GetSessionKey("showNoiseSettings"), false);
            set => SessionState.SetBool(GetSessionKey("showNoiseSettings"), value);
        }
        private bool showAttractorSettings
        {
            get => SessionState.GetBool(GetSessionKey("showAttractorSettings"), false);
            set => SessionState.SetBool(GetSessionKey("showAttractorSettings"), value);
        }
        private bool showTrailSettings
        {
            get => SessionState.GetBool(GetSessionKey("showTrailSettings"), false);
            set => SessionState.SetBool(GetSessionKey("showTrailSettings"), value);
        }
        private bool showEventSettings
        {
            get => SessionState.GetBool(GetSessionKey("showEventSettings"), false);
            set => SessionState.SetBool(GetSessionKey("showEventSettings"), value);
        }
        private bool showVelocityOverLifetimeSettings
        {
            get => SessionState.GetBool(GetSessionKey("showVelocityOverLifetimeSettings"), false);
            set => SessionState.SetBool(GetSessionKey("showVelocityOverLifetimeSettings"), value);
        }
        private bool showLimitVelocityOverLifetimeSettings
        {
            get => SessionState.GetBool(GetSessionKey("showLimitVelocityOverLifetimeSettings"), false);
            set => SessionState.SetBool(GetSessionKey("showLimitVelocityOverLifetimeSettings"), value);
        }
        private bool showForceOverLifetimeSettings
        {
            get => SessionState.GetBool(GetSessionKey("showForceOverLifetimeSettings"), false);
            set => SessionState.SetBool(GetSessionKey("showForceOverLifetimeSettings"), value);
        }
        private bool showInheritVelocitySettings
        {
            get => SessionState.GetBool(GetSessionKey("showInheritVelocitySettings"), false);
            set => SessionState.SetBool(GetSessionKey("showInheritVelocitySettings"), value);
        }
        private bool showCollisionSettings
        {
            get => SessionState.GetBool(GetSessionKey("showCollisionSettings"), false);
            set => SessionState.SetBool(GetSessionKey("showCollisionSettings"), value);
        }
        private bool showMobileOptimizationSettings
        {
            get => SessionState.GetBool(GetSessionKey("showMobileOptimizationSettings"), false);
            set => SessionState.SetBool(GetSessionKey("showMobileOptimizationSettings"), value);
        }
        private bool showSubEmitterSettings
        {
            get => SessionState.GetBool(GetSessionKey("showSubEmitterSettings"), false);
            set => SessionState.SetBool(GetSessionKey("showSubEmitterSettings"), value);
        }
        private bool showSoundEffectSettings
        {
            get => SessionState.GetBool(GetSessionKey("showSoundEffectSettings"), false);
            set => SessionState.SetBool(GetSessionKey("showSoundEffectSettings"), value);
        }

        private UIParticleSystemEditor parentEditor;

        private static readonly GUIContent headerLabel = new GUIContent("UI Particle System (Script)");
        private static readonly GUIContent soundTriggerContent = new GUIContent("Sound Trigger", "Determines when the sound will play");
        private static readonly GUIContent audioClipsContent = new GUIContent("Audio Clips", "List of audio clips to play");
        private static readonly GUIContent clipPlayModeContent = new GUIContent("Clip Play Mode", "How to play when multiple clips exist");
        private static readonly GUIContent useRandomVolumeContent = new GUIContent("Use Random Volume", "Use random volume level");
        private static readonly GUIContent volumeRangeContent = new GUIContent("Volume Range");
        private static readonly GUIContent volumeContent = new GUIContent("Volume", "Sound volume level");
        private static readonly GUIContent pitchModeContent = new GUIContent("Pitch Mode", "Pitch variation mode");
        private static readonly GUIContent pitchContent = new GUIContent("Pitch", "Sound pitch value");
        private static readonly GUIContent pitchRangeContent = new GUIContent("Pitch Range");
        private static readonly GUIContent mixerGroupContent = new GUIContent("Mixer Group", "AudioMixerGroup assignment (optional)");
        private static readonly GUIContent spatialBlendContent = new GUIContent("Spatial Blend", "0 = 2D, 1 = 3D");
        private static readonly GUIContent minDistanceContent = new GUIContent("Min Distance", "Distance at which sound is at full volume");
        private static readonly GUIContent maxDistanceContent = new GUIContent("Max Distance", "Distance at which sound completely fades out");
        private static readonly GUIContent loopContent = new GUIContent("Loop", "Loop the sound continuously");
        private static readonly GUIContent cooldownTimeContent = new GUIContent("Cooldown Time", "Minimum wait time between sounds (seconds)");
        private static readonly GUIContent maxSimultaneousContent = new GUIContent("Max Simultaneous", "Maximum number of sounds that can play at once");
        private static readonly GUIContent playOnlyOnceContent = new GUIContent("Play Only Once", "Play only once (for explosion effects, etc.)");
        private static readonly GUIContent particleSpriteContent = new GUIContent("Particle Sprite", "Sprite to use for particles");
        private static readonly GUIContent blendModeContent = new GUIContent("Blend Mode", "Particle blend mode. AlphaBlend = Standard transparency, Additive = Glowing additive blend");
        private static readonly GUIContent useMaskedContent = new GUIContent("Use Masked", "Enable masking for particles. When enabled, particles will respect UI masks.");
        private static readonly GUIContent enableEventsContent = new GUIContent("Enable Events", "Is the event system active?");
        private static readonly GUIContent logToConsoleContent = new GUIContent("Log to Console", "Print events to Console");
        private static readonly GUIContent onParticleSpawnedContent = new GUIContent("On Particle Spawned", "Triggered when the particle is born");
        private static readonly GUIContent onParticleDiedContent = new GUIContent("On Particle Died", "Triggered when particle dies");
        private static readonly GUIContent onParticleCollisionContent = new GUIContent("On Particle Collision", "Triggered when a particle hits Bounds or an Obstacle");
        private static readonly GUIContent onEmissionStartedContent = new GUIContent("On Emission Started", "Emisyon başladığında tetiklenir");
        private static readonly GUIContent onEmissionStoppedContent = new GUIContent("On Emission Stopped", "Emisyon durduğunda tetiklenir");
        private static readonly GUIContent onSystemPausedContent = new GUIContent("On System Paused", "Sistem pause edildiğinde tetiklenir");
        private static readonly GUIContent onSystemResumedContent = new GUIContent("On System Resumed", "Sistem resume edildiğinde tetiklenir");
        private static readonly GUIContent onParticlesClearedContent = new GUIContent("On Particles Cleared", "Partiküller temizlendiğinde tetiklenir");
        private static readonly GUIContent maxConcurrentContent = new GUIContent("Max Concurrent", "Maximum number of Birth/Collision/Lifetime sub-emissions per frame. Death events ignore this limit.");
        private static readonly GUIContent cooldownContent = new GUIContent("Cooldown (s)", "Minimum wait time between Birth/Collision/Lifetime sub-emissions. Death events ignore cooldown.");
        private static readonly GUIContent triggerContent = new GUIContent("Trigger", "When should this trigger?");
        private static readonly GUIContent actionContent = new GUIContent("Action", "What should happen?");
        private static readonly GUIContent lifetimeThresholdContent = new GUIContent("Lifetime Threshold", "Normalized lifetime value (0=birth, 1=death)");
        private static readonly GUIContent targetSystemContent = new GUIContent("Target System", "Target UI Particle System");
        private static readonly GUIContent probabilityContent = new GUIContent("Probability", "Trigger probability (0-1)");
        private static readonly GUIContent inheritPositionContent = new GUIContent("Inherit Position", "Start at particle's position");
        private static readonly GUIContent inheritColorContent = new GUIContent("Inherit Color", "Inherit particle's color");
        private static readonly GUIContent inheritSizeContent = new GUIContent("Inherit Size", "Inherit particle's size");
        private static readonly GUIContent burstCountContent = new GUIContent("Burst Count", "How many particles should be emitted into the target system?");
        private static readonly GUIContent instancePoolSizeContent = new GUIContent("Instance Pool Size", "How many overlapping copies of the target system can play at once. Increase this if death/birth bursts drop effects.");
        private static readonly GUIContent targetColorContent = new GUIContent("Target Color", "Target color");
        private static readonly GUIContent colorGradientContent = new GUIContent("Color Gradient", "If gradient is used, color is determined accordingly (optional)");
        private static readonly GUIContent targetSizeContent = new GUIContent("Target Size", "New size value");
        private static readonly GUIContent mobileMaxParticlesContent = new GUIContent("Max Particles (Mobile)", "Maximum particle count in mobile mode. Lower values improve performance on weak devices.");
        private static readonly GUIContent mobileUpdateIntervalContent = new GUIContent("Update Interval", "Update rate in seconds. Higher values = better battery life.\n0.033 = ~30 FPS, 0.016 = ~60 FPS");
        private static readonly GUIContent disableTrailsContent = new GUIContent("Disable Trails", "Disable trail rendering in mobile mode to reduce draw calls and CPU usage.");
        private static readonly GUIContent disableNoiseContent = new GUIContent("Disable Noise", "Disable Perlin noise calculations in mobile mode. Significant CPU savings.");

        private GUIStyle _headerLabelStyle;
        private GUIStyle _boldMiniStyle;

        private GUIStyle HeaderLabelStyle
        {
            get
            {
                if (_headerLabelStyle == null)
                    _headerLabelStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 14 };
                return _headerLabelStyle;
            }
        }

        private GUIStyle BoldMiniStyle
        {
            get
            {
                if (_boldMiniStyle == null)
                    _boldMiniStyle = new GUIStyle(EditorStyles.miniLabel) { fontStyle = FontStyle.Bold };
                return _boldMiniStyle;
            }
        }

        public UIParticleSystemEditorGUI(Core.UIParticleSystem system,
          UIParticleSystemEditorStyles styles, UIParticleSystemEditorControls controls,
          UIParticleSystemEditorUtils utils, bool isMultiEdit = false,
          Core.UIParticleSystem[] multiSystems = null, UIParticleSystemEditor editor = null)
        {
            this.particleSystem = system;
            this.styles = styles;
            this.controls = controls;
            this.utils = utils;
            this.isMultiObjectEditing = isMultiEdit;
            this.multiSelectedSystems = multiSystems;
            this.parentEditor = editor;
        }

        public void SetMultiObjectState(bool isMultiEdit, Core.UIParticleSystem[] multiSystems, UIParticleSystemEditor editor = null)
        {
            this.isMultiObjectEditing = isMultiEdit;
            this.multiSelectedSystems = multiSystems;
            this.parentEditor = editor;
        }

        public void RenderHeader()
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            
            Texture iconTexture = null;
            if (particleSystem != null && particleSystem.ParticleSprite != null)
            {
                iconTexture = AssetPreview.GetAssetPreview(particleSystem.ParticleSprite);
                if (iconTexture == null)
                {
                    iconTexture = particleSystem.ParticleSprite.texture;
                }
            }

            if (iconTexture != null)
            {
                Rect rect = GUILayoutUtility.GetRect(20, 20, GUILayout.Width(20), GUILayout.Height(20));
                GUI.DrawTexture(rect, iconTexture, ScaleMode.ScaleToFit);
            }
            else
            {
                GUIContent iconContent = EditorGUIUtility.IconContent("Prefab Icon");
                GUILayout.Label(iconContent, GUILayout.Width(20), GUILayout.Height(20));
            }
            
            GUILayout.Label(headerLabel, HeaderLabelStyle);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);
        }

        public void RenderInspector(SerializedObject serializedObject)
        {
            styles.EnsureStylesInitialized();

            RenderMainSettings(serializedObject);
            RenderEmissionSettings(serializedObject);
            RenderShapeSettings(serializedObject);
            RenderVelocityOverLifetimeSettings(serializedObject);
            RenderLimitVelocityOverLifetimeSettings(serializedObject);
            RenderInheritVelocitySettings(serializedObject);
            RenderForceOverLifetimeSettings(serializedObject);
            RenderCollisionSettings(serializedObject);
            RenderColorSettings(serializedObject);
            RenderColorBySpeedSettings(serializedObject);
            RenderSizeSettings(serializedObject);
            RenderSizeBySpeedSettings(serializedObject);
            RenderRotationSettings(serializedObject);
            RenderNoiseSettings(serializedObject);
            RenderTextureSheetSettings(serializedObject);
            RenderTrailSettings(serializedObject);
            RenderAttractorSettings(serializedObject);
            RenderEventSettings(serializedObject);
            RenderSubEmitterSettings(serializedObject);
            RenderSoundEffectSettings(serializedObject);
            RenderMobileOptimizationSettings(serializedObject);
            RenderControlPanel();
        }
        private void RenderSoundEffectSettings(SerializedObject serializedObject)
        {
            SerializedProperty soundEffectSettings = serializedObject.FindProperty("soundEffectSettings");
            if (soundEffectSettings == null) return;

            SerializedProperty enableSoundEffectsProp = soundEffectSettings.FindPropertyRelative("enableSoundEffects");

            EditorGUILayout.BeginVertical(styles.SettingsStyle);
            EditorGUILayout.BeginVertical(styles.Titlestyle);
            EditorGUILayout.BeginHorizontal();

            GUIContent soundIcon = UIParticleSystemEditorIcons.GetIcon("SoundEffect");
            if (soundIcon == null || soundIcon.image == null)
            {
                soundIcon = EditorGUIUtility.IconContent("AudioSource Icon");
            }
            GUILayout.Label(soundIcon, GUILayout.Width(16), GUILayout.Height(16));

            bool currentValue = enableSoundEffectsProp.boolValue;
            bool newValue = EditorGUILayout.Toggle(currentValue, GUILayout.Width(12));
            if (newValue != currentValue)
            {
                enableSoundEffectsProp.boolValue = newValue;
            }

            showSoundEffectSettings ^= GUILayout.Button("Sound Effects", styles.TransparentToggleStyle);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            if (showSoundEffectSettings)
            {
                EditorGUI.indentLevel++;
                RenderSoundEffectControls(soundEffectSettings);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
        }
        private void RenderSoundEffectControls(SerializedProperty soundEffectSettings)
        {
            SerializedProperty enableProp = soundEffectSettings.FindPropertyRelative("enableSoundEffects");

            if (!enableProp.boolValue)
            {
                EditorGUILayout.HelpBox("Sound Effects are disabled. Check the checkbox to enable.", MessageType.Info);
                return;
            }

            EditorGUILayout.Space(3);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            SerializedProperty soundTriggerProp = soundEffectSettings.FindPropertyRelative("soundTrigger");
            EditorGUILayout.PropertyField(soundTriggerProp, soundTriggerContent);

            SerializedProperty audioClipsProp = soundEffectSettings.FindPropertyRelative("audioClips");
            EditorGUILayout.PropertyField(audioClipsProp, audioClipsContent, true);

            if (audioClipsProp.arraySize > 1)
            {
                SerializedProperty clipPlayModeProp = soundEffectSettings.FindPropertyRelative("clipPlayMode");
                EditorGUILayout.PropertyField(clipPlayModeProp, clipPlayModeContent);
            }

            EditorGUILayout.Space(2);

            SerializedProperty useRandomVolumeProp = soundEffectSettings.FindPropertyRelative("useRandomVolume");
            bool useRandomVolume = useRandomVolumeProp.boolValue;

            Rect volRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            Rect volPopupRect = new Rect(volRect.xMax - 18f, volRect.y, 18f, volRect.height);
            Rect volPropRect = new Rect(volRect.x, volRect.y, volRect.width - 18f - 4f, volRect.height);

            if (EditorGUI.DropdownButton(volPopupRect, GUIContent.none, FocusType.Passive, EditorStyles.popup))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Constant"), !useRandomVolume, () =>
                {
                    useRandomVolumeProp.boolValue = false;
                    useRandomVolumeProp.serializedObject.ApplyModifiedProperties();
                });
                menu.AddItem(new GUIContent("Random Between Two Constants"), useRandomVolume, () =>
                {
                    useRandomVolumeProp.boolValue = true;
                    useRandomVolumeProp.serializedObject.ApplyModifiedProperties();
                });
                menu.DropDown(volPopupRect);
            }

            if (!useRandomVolume)
            {
                SerializedProperty volumeProp = soundEffectSettings.FindPropertyRelative("volume");
                EditorGUI.Slider(volPropRect, volumeProp, 0f, 1f, new GUIContent("Volume", "Sound volume level"));
            }
            else
            {
                Rect fieldBase = EditorGUI.PrefixLabel(volPropRect, new GUIContent("Volume"));
                
                SerializedProperty minVolumeProp = soundEffectSettings.FindPropertyRelative("minVolume");
                SerializedProperty maxVolumeProp = soundEffectSettings.FindPropertyRelative("maxVolume");

                float minVol = minVolumeProp.floatValue;
                float maxVol = maxVolumeProp.floatValue;

                float sliderWidth = fieldBase.width * 0.7f;
                float textWidth = fieldBase.width * 0.3f - 4f;

                Rect sliderRect = new Rect(fieldBase.x, fieldBase.y, sliderWidth, fieldBase.height);
                Rect textRect = new Rect(fieldBase.x + sliderWidth + 4f, fieldBase.y, textWidth, fieldBase.height);

                EditorGUI.MinMaxSlider(sliderRect, ref minVol, ref maxVol, 0f, 1f);
                
                int oldIndent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                EditorGUI.LabelField(textRect, $"{minVol:F2} - {maxVol:F2}", EditorStyles.miniLabel);
                EditorGUI.indentLevel = oldIndent;

                minVolumeProp.floatValue = minVol;
                maxVolumeProp.floatValue = maxVol;
            }

            EditorGUILayout.Space(2);

            SerializedProperty pitchModeProp = soundEffectSettings.FindPropertyRelative("pitchMode");
            int pitchMode = pitchModeProp.intValue;

            Rect pitchRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            Rect pitchPopupRect = new Rect(pitchRect.xMax - 18f, pitchRect.y, 18f, pitchRect.height);
            Rect pitchPropRect = new Rect(pitchRect.x, pitchRect.y, pitchRect.width - 18f - 4f, pitchRect.height);

            if (EditorGUI.DropdownButton(pitchPopupRect, GUIContent.none, FocusType.Passive, EditorStyles.popup))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Constant"), pitchMode == 0, () =>
                {
                    pitchModeProp.intValue = 0;
                    pitchModeProp.serializedObject.ApplyModifiedProperties();
                });
                menu.AddItem(new GUIContent("Random Between Two Constants"), pitchMode == 1, () =>
                {
                    pitchModeProp.intValue = 1;
                    pitchModeProp.serializedObject.ApplyModifiedProperties();
                });
                menu.AddItem(new GUIContent("Sequential"), pitchMode == 2, () =>
                {
                    pitchModeProp.intValue = 2;
                    pitchModeProp.serializedObject.ApplyModifiedProperties();
                });
                menu.DropDown(pitchPopupRect);
            }

            if (pitchMode == 0)
            {
                SerializedProperty pitchProp = soundEffectSettings.FindPropertyRelative("pitch");
                EditorGUI.Slider(pitchPropRect, pitchProp, -3f, 3f, new GUIContent("Pitch", "Sound pitch level"));
            }
            else
            {
                Rect fieldBase = EditorGUI.PrefixLabel(pitchPropRect, new GUIContent("Pitch"));
                
                SerializedProperty minPitchProp = soundEffectSettings.FindPropertyRelative("minPitch");
                SerializedProperty maxPitchProp = soundEffectSettings.FindPropertyRelative("maxPitch");

                float minPitch = minPitchProp.floatValue;
                float maxPitch = maxPitchProp.floatValue;

                float sliderWidth = fieldBase.width * 0.7f;
                float textWidth = fieldBase.width * 0.3f - 4f;

                Rect sliderRect = new Rect(fieldBase.x, fieldBase.y, sliderWidth, fieldBase.height);
                Rect textRect = new Rect(fieldBase.x + sliderWidth + 4f, fieldBase.y, textWidth, fieldBase.height);

                EditorGUI.MinMaxSlider(sliderRect, ref minPitch, ref maxPitch, -3f, 3f);
                
                int oldIndent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                EditorGUI.LabelField(textRect, $"{minPitch:F2} - {maxPitch:F2}", EditorStyles.miniLabel);
                EditorGUI.indentLevel = oldIndent;

                minPitchProp.floatValue = minPitch;
                maxPitchProp.floatValue = maxPitch;
            }

            EditorGUILayout.Space(2);

            SerializedProperty audioMixerGroupProp = soundEffectSettings.FindPropertyRelative("audioMixerGroup");
            EditorGUILayout.PropertyField(audioMixerGroupProp, mixerGroupContent);

            SerializedProperty loopProp = soundEffectSettings.FindPropertyRelative("loop");
            EditorGUILayout.PropertyField(loopProp, loopContent);

            SerializedProperty cooldownTimeProp = soundEffectSettings.FindPropertyRelative("cooldownTime");
            EditorGUILayout.PropertyField(cooldownTimeProp, cooldownTimeContent);

            SerializedProperty maxSimultaneousSoundsProp = soundEffectSettings.FindPropertyRelative("maxSimultaneousSounds");
            EditorGUILayout.IntSlider(maxSimultaneousSoundsProp, 1, 50, maxSimultaneousContent);

            SerializedProperty playOnlyOnceProp = soundEffectSettings.FindPropertyRelative("playOnlyOnce");
            EditorGUILayout.PropertyField(playOnlyOnceProp, playOnlyOnceContent);

            EditorGUILayout.EndVertical();
        }
        private void RenderMainSettings(SerializedObject serializedObject)
        {
            EditorGUILayout.BeginVertical(styles.SettingsStyle);
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginVertical(styles.Titlestyle);
            EditorGUILayout.BeginHorizontal(GUILayout.Height(28));
            GUILayout.Space(4);
            
            Texture iconTexture = null;
            if (particleSystem != null && particleSystem.ParticleSprite != null)
            {
                iconTexture = AssetPreview.GetAssetPreview(particleSystem.ParticleSprite);
                if (iconTexture == null)
                {
                    iconTexture = particleSystem.ParticleSprite.texture;
                }
            }

            if (iconTexture != null)
            {
                Rect rect = GUILayoutUtility.GetRect(24, 24, GUILayout.Width(24), GUILayout.Height(24));
                GUI.DrawTexture(rect, iconTexture, ScaleMode.ScaleToFit);
            }
            else
            {
                GUIContent eventIcon = UIParticleSystemEditorIcons.GetIcon("UIParticleSystemIcon");
                if (eventIcon == null || eventIcon.image == null)
                {
                    eventIcon = EditorGUIUtility.IconContent("ParticleSystem Icon");
                }
                GUILayout.Label(eventIcon, GUILayout.Width(24), GUILayout.Height(24));
            }

            GUIStyle largeHeaderStyle = new GUIStyle(styles.HeaderStyle)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                margin = new RectOffset(6, 0, 4, 0)
            };
            GUILayout.Label("Particle", largeHeaderStyle);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            SerializedProperty particleSpriteProp = serializedObject.FindProperty("particleSprite");

            EditorGUILayout.PropertyField(particleSpriteProp, particleSpriteContent);

            SerializedProperty blendModeProp = serializedObject.FindProperty("blendMode");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(blendModeProp, blendModeContent);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                particleSystem.UpdateParticlesMaterial();
            }

            SerializedProperty rendererSettings = serializedObject.FindProperty("rendererSettings");
            SerializedProperty renderModeProp = rendererSettings.FindPropertyRelative("renderMode");
            EditorGUILayout.PropertyField(renderModeProp, new GUIContent("Render Mode", "Billboard = normal, Stretched Billboard = velocity-based stretching"));

            if (renderModeProp.enumValueIndex == (int)UIRenderMode.StretchedBillboard)
            {
                EditorGUI.indentLevel++;
                SerializedProperty speedStretchProp = rendererSettings.FindPropertyRelative("speedStretch");
                SerializedProperty lengthScaleProp = rendererSettings.FindPropertyRelative("lengthScale");

                EditorGUILayout.PropertyField(speedStretchProp, new GUIContent("Speed Stretch", "How much speed affects particle length. 0 = no stretch."));
                speedStretchProp.floatValue = Mathf.Max(0f, speedStretchProp.floatValue);

                EditorGUILayout.PropertyField(lengthScaleProp, new GUIContent("Length Scale", "Base length multiplier. 1 = original size."));
                lengthScaleProp.floatValue = Mathf.Max(0.01f, lengthScaleProp.floatValue);

                EditorGUI.indentLevel--;
            }

            styles.DrawHorizontalLine(styles.GetLineColor(), 0.5f, 3f);

            SerializedProperty emissionSettings = serializedObject.FindProperty("emissionSettings");
            utils.RenderDurationSettings(emissionSettings);

            utils.RenderLifetimeControls(serializedObject.FindProperty("lifetimeSettings"));

            utils.RenderSpeedControls(serializedObject);

            utils.RenderSizeControls(serializedObject.FindProperty("sizeSettings"));

            utils.RenderRotationControls(serializedObject.FindProperty("rotationSettings"));

            utils.RenderColorControls(serializedObject.FindProperty("colorSettings"));

            styles.DrawHorizontalLine(styles.GetLineColor(), 0.5f, 3f);


            SerializedProperty useMaskedProp = serializedObject.FindProperty("useMasked");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(useMaskedProp, useMaskedContent);
            if (EditorGUI.EndChangeCheck())
            {
                if (Application.isPlaying)
                {
                    particleSystem.UpdateParticlesMaskedState();
                }
            }
            utils.RenderGravitySettings(serializedObject);
            styles.DrawHorizontalLine(styles.GetLineColor(), 0.5f, 3f);
            utils.RenderMaxParticlesSettings(emissionSettings);
            utils.RenderSimulationSpaceSettings(emissionSettings);

            SerializedProperty simulationSpeedProp = serializedObject.FindProperty("simulationSpeed");
            SerializedProperty useUnscaledTimeProp = serializedObject.FindProperty("useUnscaledTime");

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(simulationSpeedProp, new GUIContent("Simulation Speed", "Speed factor of the particle simulation"));
            if (EditorGUI.EndChangeCheck())
            {
                simulationSpeedProp.floatValue = Mathf.Max(0f, simulationSpeedProp.floatValue);
            }

            int currentTimeMode = useUnscaledTimeProp.boolValue ? 1 : 0;
            string[] timeModes = new[] { "Scaled", "Unscaled" };
            EditorGUI.BeginChangeCheck();
            int newTimeMode = EditorGUILayout.Popup(new GUIContent("Delta Time", "Choose between Scaled Time (affected by Time.timeScale) or Unscaled Time"), currentTimeMode, timeModes);
            if (EditorGUI.EndChangeCheck())
            {
                useUnscaledTimeProp.boolValue = newTimeMode == 1;
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        private void RenderMobileOptimizationSettings(SerializedObject serializedObject)
        {
            SerializedProperty mobileOptModeProp = serializedObject.FindProperty("mobileOptimizationMode");

            EditorGUILayout.BeginVertical(styles.SettingsStyle);
            EditorGUILayout.BeginVertical(styles.Titlestyle);
            EditorGUILayout.BeginHorizontal();

            GUIContent mobileIcon = UIParticleSystemEditorIcons.GetIcon("MobileOptimization");
            GUILayout.Label(mobileIcon, GUILayout.Width(16), GUILayout.Height(16));

            bool currentValue = mobileOptModeProp.boolValue;
            bool newValue = EditorGUILayout.Toggle(currentValue, GUILayout.Width(12));
            if (newValue != currentValue)
            {
                mobileOptModeProp.boolValue = newValue;
            }

            showMobileOptimizationSettings ^= GUILayout.Button("Mobile Optimization", styles.TransparentToggleStyle);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            if (showMobileOptimizationSettings)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                string platformInfo = "Platform: ";
#if UNITY_ANDROID
                platformInfo += "Android (Auto-enabled in build)";
#elif UNITY_IOS
                platformInfo += "iOS (Auto-enabled in build)";
#else
                platformInfo += "Standalone (Manual control)";
#endif

                EditorGUILayout.LabelField(platformInfo, EditorStyles.miniLabel);
                EditorGUILayout.EndVertical();

                EditorGUILayout.Space(5);

                bool mobileOptEnabled = mobileOptModeProp.boolValue;

                EditorGUI.BeginDisabledGroup(!mobileOptEnabled);

                SerializedProperty mobileMaxParticlesProp = serializedObject.FindProperty("mobileMaxParticles");
                EditorGUILayout.PropertyField(mobileMaxParticlesProp, mobileMaxParticlesContent);

                if (mobileMaxParticlesProp.intValue < 1)
                    mobileMaxParticlesProp.intValue = 1;

                SerializedProperty mobileUpdateIntervalProp = serializedObject.FindProperty("mobileUpdateInterval");
                EditorGUILayout.PropertyField(mobileUpdateIntervalProp, mobileUpdateIntervalContent);

                if (mobileUpdateIntervalProp.floatValue < 0.016f)
                    mobileUpdateIntervalProp.floatValue = 0.016f;

                float fps = 1f / mobileUpdateIntervalProp.floatValue;
                EditorGUILayout.LabelField($"  ≈ {fps:F0} FPS", EditorStyles.miniLabel);

                EditorGUILayout.Space(5);

                SerializedProperty disableTrailsProp = serializedObject.FindProperty("disableTrailsOnMobile");
                EditorGUILayout.PropertyField(disableTrailsProp, disableTrailsContent);

                SerializedProperty disableNoiseProp = serializedObject.FindProperty("disableNoiseOnMobile");
                EditorGUILayout.PropertyField(disableNoiseProp, disableNoiseContent);

                EditorGUI.EndDisabledGroup();

                if (mobileOptEnabled)
                {
                    EditorGUILayout.Space(5);
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    EditorGUILayout.LabelField("Estimated Performance Impact:", BoldMiniStyle);

                    string cpuImpact = disableNoiseProp.boolValue ? "~30-40% ↓" : "~15-25% ↓";
                    string gcImpact = "~70-80% ↓";
                    string batteryImpact = disableTrailsProp.boolValue ? "~25-35% ↓" : "~15-20% ↓";

                    EditorGUILayout.LabelField($"  CPU Usage: {cpuImpact}", EditorStyles.miniLabel);
                    EditorGUILayout.LabelField($"  GC Allocation: {gcImpact}", EditorStyles.miniLabel);
                    EditorGUILayout.LabelField($"  Battery Drain: {batteryImpact}", EditorStyles.miniLabel);

                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.Space(5);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.LabelField("Sprite Atlas Optimization", BoldMiniStyle);

                GUIStyle atlasInfoLabel = new GUIStyle(EditorStyles.miniLabel);
                atlasInfoLabel.normal.textColor = new Color(0.4f, 0.8f, 0.4f);
                atlasInfoLabel.wordWrap = true;
                EditorGUILayout.LabelField("Packs the current particle sprite into a Sprite Atlas to reduce draw calls. If no atlas exists, one will be created automatically.", atlasInfoLabel);

                EditorGUILayout.Space(3);

                SerializedProperty particleSpriteProp = serializedObject.FindProperty("particleSprite");

                GUIContent packAtlasButtonContent = new GUIContent(
                    "  Pack Atlas",
                    "Adds the current particle sprite to the Sprite Atlas and packs it.\n\n" +
                    "• If no atlas exists, a new one is created.\n" +
                    "• If the sprite is already in the atlas, only re-packing is performed.\n" +
                    "• Reduces draw calls for better mobile performance."
                );

                if (GUILayout.Button(packAtlasButtonContent, GUILayout.Height(24)))
                {
                    Sprite currentSprite = particleSpriteProp.objectReferenceValue as Sprite;
                    if (currentSprite != null)
                    {
                        UIParticleAtlasManager.EnsureSpriteInAtlas(currentSprite);
                    }

                    UIParticleAtlasManager.PackAtlas();
                    Debug.Log("[UIParticleSystem] Atlas packed successfully.");
                }

                EditorGUILayout.EndVertical();

                EditorGUILayout.Space(5);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();

        }

        private void RenderEmissionSettings(SerializedObject serializedObject)
        {
            EditorGUILayout.BeginVertical(styles.SettingsStyle);
            EditorGUILayout.BeginVertical(styles.Titlestyle);
            EditorGUILayout.BeginHorizontal();
            GUIContent eventIcon = UIParticleSystemEditorIcons.GetIcon("Emmision");
            GUILayout.Label(eventIcon, GUILayout.Width(16), GUILayout.Height(16));
            showEmissionSettings ^= GUILayout.Button("Emission", styles.TransparentToggleStyle);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            if (showEmissionSettings)
            {
                EditorGUI.indentLevel++;
                utils.RenderEmissionControls(serializedObject.FindProperty("emissionSettings"));
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }
        private void RenderShapeSettings(SerializedObject serializedObject)
        {
            EditorGUILayout.BeginVertical(styles.SettingsStyle);
            EditorGUILayout.BeginVertical(styles.Titlestyle);
            EditorGUILayout.BeginHorizontal();
            GUIContent eventIcon = UIParticleSystemEditorIcons.GetIcon("Shape");
            GUILayout.Label(eventIcon, GUILayout.Width(16), GUILayout.Height(16));
            showShapeSettings ^= GUILayout.Button("Shape", styles.TransparentToggleStyle);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            if (showShapeSettings)
            {
                EditorGUI.indentLevel++;
                utils.RenderShapeControls(serializedObject.FindProperty("emissionSettings"));
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }
        private void RenderColorSettings(SerializedObject serializedObject)
        {
            SerializedProperty colorSettings = serializedObject.FindProperty("colorSettings");
            SerializedProperty useGradientColorProp = colorSettings.FindPropertyRelative("useGradientColor");

            EditorGUILayout.BeginVertical(styles.SettingsStyle);

            EditorGUILayout.BeginVertical(styles.Titlestyle);
            EditorGUILayout.BeginHorizontal();
            GUIContent eventIcon = UIParticleSystemEditorIcons.GetIcon("ColorOverLifetime");
            GUILayout.Label(eventIcon, GUILayout.Width(16), GUILayout.Height(16));
            EditorGUILayout.PropertyField(useGradientColorProp, GUIContent.none, GUILayout.Width(12));
            showColorSettings ^= GUILayout.Button("Color Over Lifetime", styles.TransparentToggleStyle);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            if (showColorSettings)
            {
                EditorGUI.indentLevel++;
                utils.RenderColorOverLifetime(serializedObject.FindProperty("colorSettings"));
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }
        private void RenderSizeSettings(SerializedObject serializedObject)
        {
            SerializedProperty sizeSettings = serializedObject.FindProperty("sizeSettings");
            SerializedProperty useSizeOverLifetimeProp = sizeSettings.FindPropertyRelative("useSizeOverLifetime");

            EditorGUILayout.BeginVertical(styles.SettingsStyle);

            EditorGUILayout.BeginVertical(styles.Titlestyle);
            EditorGUILayout.BeginHorizontal();
            GUIContent eventIcon = UIParticleSystemEditorIcons.GetIcon("SizeOverLifetime");
            GUILayout.Label(eventIcon, GUILayout.Width(16), GUILayout.Height(16));
            EditorGUILayout.PropertyField(useSizeOverLifetimeProp, GUIContent.none, GUILayout.Width(12));
            showSizeSettings ^= GUILayout.Button("Size Over Lifetime", styles.TransparentToggleStyle);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            if (showSizeSettings)
            {
                EditorGUI.indentLevel++;
                SerializedProperty use2DSizeProperty = sizeSettings.FindPropertyRelative("use2DSize");
                utils.RenderSizeOverLifetimeControls(sizeSettings, use2DSizeProperty.boolValue);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
        }

        private void RenderColorBySpeedSettings(SerializedObject serializedObject)
        {
            SerializedProperty colorBySpeedSettings = serializedObject.FindProperty("colorBySpeedSettings");
            if (colorBySpeedSettings == null) return;
            SerializedProperty useColorBySpeedProp = colorBySpeedSettings.FindPropertyRelative("useColorBySpeed");

            EditorGUILayout.BeginVertical(styles.SettingsStyle);

            EditorGUILayout.BeginVertical(styles.Titlestyle);
            EditorGUILayout.BeginHorizontal();
            GUIContent eventIcon = UIParticleSystemEditorIcons.GetIcon("ColorOverLifetime");
            GUILayout.Label(eventIcon, GUILayout.Width(16), GUILayout.Height(16));
            EditorGUILayout.PropertyField(useColorBySpeedProp, GUIContent.none, GUILayout.Width(12));
            showColorBySpeedSettings ^= GUILayout.Button("Color By Speed", styles.TransparentToggleStyle);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            if (showColorBySpeedSettings)
            {
                EditorGUI.indentLevel++;
                utils.RenderColorBySpeedControls(colorBySpeedSettings);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }

        private void RenderSizeBySpeedSettings(SerializedObject serializedObject)
        {
            SerializedProperty sizeBySpeedSettings = serializedObject.FindProperty("sizeBySpeedSettings");
            if (sizeBySpeedSettings == null) return;
            SerializedProperty useSizeBySpeedProp = sizeBySpeedSettings.FindPropertyRelative("useSizeBySpeed");

            EditorGUILayout.BeginVertical(styles.SettingsStyle);

            EditorGUILayout.BeginVertical(styles.Titlestyle);
            EditorGUILayout.BeginHorizontal();
            GUIContent eventIcon = UIParticleSystemEditorIcons.GetIcon("SizeOverLifetime");
            GUILayout.Label(eventIcon, GUILayout.Width(16), GUILayout.Height(16));
            EditorGUILayout.PropertyField(useSizeBySpeedProp, GUIContent.none, GUILayout.Width(12));
            showSizeBySpeedSettings ^= GUILayout.Button("Size By Speed", styles.TransparentToggleStyle);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            if (showSizeBySpeedSettings)
            {
                EditorGUI.indentLevel++;
                utils.RenderSizeBySpeedControls(sizeBySpeedSettings);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
        }

        private void RenderRotationSettings(SerializedObject serializedObject)
        {
            SerializedProperty rotationsettings = serializedObject.FindProperty("rotationSettings");
            SerializedProperty useSizeOverLifetimeProp = rotationsettings.FindPropertyRelative("useRotationOverLifetime");

            EditorGUILayout.BeginVertical(styles.SettingsStyle);

            EditorGUILayout.BeginVertical(styles.Titlestyle);
            EditorGUILayout.BeginHorizontal();
            GUIContent eventIcon = UIParticleSystemEditorIcons.GetIcon("RotationOverLifetime");
            GUILayout.Label(eventIcon, GUILayout.Width(16), GUILayout.Height(16));
            EditorGUILayout.PropertyField(useSizeOverLifetimeProp, GUIContent.none, GUILayout.Width(12));
            showRotationSettings ^= GUILayout.Button("Rotation Over Lifetime", styles.TransparentToggleStyle);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            if (showRotationSettings)
            {
                EditorGUI.indentLevel++;
                utils.RenderRotationOverLifetime(serializedObject.FindProperty("rotationSettings"));
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }
        private void RenderNoiseSettings(SerializedObject serializedObject)
        {
            SerializedProperty noiseSettings = serializedObject.FindProperty("noiseSettings");
            SerializedProperty useNoiseProp = noiseSettings.FindPropertyRelative("useNoise");

            EditorGUILayout.BeginVertical(styles.SettingsStyle);
            EditorGUILayout.BeginVertical(styles.Titlestyle);
            EditorGUILayout.BeginHorizontal();
            GUIContent eventIcon = UIParticleSystemEditorIcons.GetIcon("Noise");
            GUILayout.Label(eventIcon, GUILayout.Width(16), GUILayout.Height(16));
            EditorGUILayout.PropertyField(useNoiseProp, GUIContent.none, GUILayout.Width(12));
            showNoiseSettings ^= GUILayout.Button("Noise", styles.TransparentToggleStyle);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            if (showNoiseSettings)
            {
                EditorGUI.indentLevel++;
                utils.RenderNoiseControls(serializedObject.FindProperty("noiseSettings"));
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }
        private void RenderTextureSheetSettings(SerializedObject serializedObject)
        {
            SerializedProperty textureSheetSettings = serializedObject.FindProperty("textureSheetSettings");
            SerializedProperty useTextureSheetProp = textureSheetSettings.FindPropertyRelative("useTextureSheetAnimation");

            EditorGUILayout.BeginVertical(styles.SettingsStyle);
            EditorGUILayout.BeginVertical(styles.Titlestyle);
            EditorGUILayout.BeginHorizontal();
            GUIContent eventIcon = UIParticleSystemEditorIcons.GetIcon("TextureSheetAnimation");
            GUILayout.Label(eventIcon, GUILayout.Width(16), GUILayout.Height(16));
            EditorGUILayout.PropertyField(useTextureSheetProp, GUIContent.none, GUILayout.Width(12));
            showTextureSheetSettings ^= GUILayout.Button("Texture Sheet Animation", styles.TransparentToggleStyle);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            if (showTextureSheetSettings)
            {
                EditorGUI.indentLevel++;
                utils.RenderTextureSheetControls(serializedObject.FindProperty("textureSheetSettings"));
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }
        private void RenderTrailSettings(SerializedObject serializedObject)
        {
            SerializedProperty trailSettings = serializedObject.FindProperty("trailSettings");
            SerializedProperty useTrailSettings = trailSettings.FindPropertyRelative("useTrails");

            EditorGUILayout.BeginVertical(styles.SettingsStyle);
            EditorGUILayout.BeginVertical(styles.Titlestyle);
            EditorGUILayout.BeginHorizontal();
            GUIContent eventIcon = UIParticleSystemEditorIcons.GetIcon("Trail");
            GUILayout.Label(eventIcon, GUILayout.Width(16), GUILayout.Height(16));
            EditorGUILayout.PropertyField(useTrailSettings, GUIContent.none, GUILayout.Width(12));
            showTrailSettings ^= GUILayout.Button("Trail", styles.TransparentToggleStyle);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            if (showTrailSettings)
            {
                EditorGUI.indentLevel++;
                utils.RenderTrailControls(serializedObject.FindProperty("trailSettings"));
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }
        private void RenderAttractorSettings(SerializedObject serializedObject)
        {
            SerializedProperty attractorSettings = serializedObject.FindProperty("attractorSettings");
            SerializedProperty useaAttractorSettings = attractorSettings.FindPropertyRelative("useAttractor");

            EditorGUILayout.BeginVertical(styles.SettingsStyle);
            EditorGUILayout.BeginVertical(styles.Titlestyle);
            EditorGUILayout.BeginHorizontal();
            GUIContent eventIcon = UIParticleSystemEditorIcons.GetIcon("Attractor");
            GUILayout.Label(eventIcon, GUILayout.Width(16), GUILayout.Height(16));
            EditorGUILayout.PropertyField(useaAttractorSettings, GUIContent.none, GUILayout.Width(12));
            showAttractorSettings ^= GUILayout.Button("Attractor", styles.TransparentToggleStyle);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            if (showAttractorSettings)
            {
                EditorGUI.indentLevel++;
                utils.RenderAttractorControls(serializedObject.FindProperty("attractorSettings"));
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }

        private void RenderEventSettings(SerializedObject serializedObject)
        {
            SerializedProperty eventSettings = serializedObject.FindProperty("eventSettings");
            if (eventSettings == null) return;

            EditorGUILayout.BeginVertical(styles.SettingsStyle);
            EditorGUILayout.BeginVertical(styles.Titlestyle);
            EditorGUILayout.BeginHorizontal();

            GUIContent eventIcon = UIParticleSystemEditorIcons.GetIcon("Events");
            GUILayout.Label(eventIcon, GUILayout.Width(16), GUILayout.Height(16));

            showEventSettings ^= GUILayout.Button("Events", styles.TransparentToggleStyle);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            if (showEventSettings)
            {
                EditorGUI.indentLevel++;
                RenderEventControls(eventSettings);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }

        private void RenderEventControls(SerializedProperty eventSettings)
        {
            SerializedProperty enableEventsProp = eventSettings.FindPropertyRelative("enableEvents");
            SerializedProperty logEventsProp = eventSettings.FindPropertyRelative("logEventsToConsole");

            EditorGUILayout.PropertyField(enableEventsProp, enableEventsContent);

            if (enableEventsProp.boolValue)
            {
                EditorGUILayout.PropertyField(logEventsProp, logToConsoleContent);

                var particleSpawnEvent = eventSettings.FindPropertyRelative("onParticleSpawned");
                var particleDeathEvent = eventSettings.FindPropertyRelative("onParticleDied");

                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(particleSpawnEvent, onParticleSpawnedContent);
                EditorGUILayout.PropertyField(particleDeathEvent, onParticleDiedContent);
                EditorGUILayout.PropertyField(eventSettings.FindPropertyRelative("onParticleCollision"), onParticleCollisionContent);

                EditorGUI.indentLevel--;

                var emissionStartedEvent = eventSettings.FindPropertyRelative("onEmissionStarted");
                var emissionStoppedEvent = eventSettings.FindPropertyRelative("onEmissionStopped");
                var systemPausedEvent = eventSettings.FindPropertyRelative("onSystemPaused");
                var systemResumedEvent = eventSettings.FindPropertyRelative("onSystemResumed");
                var particlesClearedEvent = eventSettings.FindPropertyRelative("onParticlesCleared");

                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(emissionStartedEvent, onEmissionStartedContent);
                EditorGUILayout.PropertyField(emissionStoppedEvent, onEmissionStoppedContent);
                EditorGUILayout.PropertyField(systemPausedEvent, onSystemPausedContent);
                EditorGUILayout.PropertyField(systemResumedEvent, onSystemResumedContent);
                EditorGUILayout.PropertyField(particlesClearedEvent, onParticlesClearedContent);
                EditorGUI.indentLevel--;
            }
        }

        private void RenderSubEmitterSettings(SerializedObject serializedObject)
        {
            SerializedProperty subEmitterSettings = serializedObject.FindProperty("subEmitterSettings");
            if (subEmitterSettings == null) return;

            SerializedProperty enableSubEmittersProp = subEmitterSettings.FindPropertyRelative("enableSubEmitters");

            EditorGUILayout.BeginVertical(styles.SettingsStyle);
            EditorGUILayout.BeginVertical(styles.Titlestyle);
            EditorGUILayout.BeginHorizontal();

            GUIContent subEmitterIcon = UIParticleSystemEditorIcons.GetIcon("Subemitters");
            if (subEmitterIcon == null || subEmitterIcon.image == null)
            {
                subEmitterIcon = EditorGUIUtility.IconContent("ParticleSystem Icon");
            }
            GUILayout.Label(subEmitterIcon, GUILayout.Width(16), GUILayout.Height(16));

            bool currentValue = enableSubEmittersProp.boolValue;
            bool newValue = EditorGUILayout.Toggle(currentValue, GUILayout.Width(12));
            if (newValue != currentValue)
            {
                enableSubEmittersProp.boolValue = newValue;
            }

            showSubEmitterSettings ^= GUILayout.Button("Sub-Emitters", styles.TransparentToggleStyle);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            if (showSubEmitterSettings)
            {
                EditorGUI.indentLevel++;
                RenderSubEmitterControls(subEmitterSettings);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
        }

        private void RenderSubEmitterControls(SerializedProperty subEmitterSettings)
        {
            SerializedProperty enableProp = subEmitterSettings.FindPropertyRelative("enableSubEmitters");

            if (!enableProp.boolValue)
            {
                EditorGUILayout.HelpBox("Sub-Emitters are disabled. Check the checkbox to enable.", MessageType.Info);
                return;
            }

            EditorGUILayout.Space(3);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Performance Settings", BoldMiniStyle);

            SerializedProperty maxConcurrentProp = subEmitterSettings.FindPropertyRelative("maxConcurrentSubEmissions");
            EditorGUILayout.PropertyField(maxConcurrentProp, maxConcurrentContent);
            if (maxConcurrentProp.intValue < 1) maxConcurrentProp.intValue = 1;

            SerializedProperty cooldownProp = subEmitterSettings.FindPropertyRelative("subEmitterCooldown");
            EditorGUILayout.PropertyField(cooldownProp, cooldownContent);
            if (cooldownProp.floatValue < 0f) cooldownProp.floatValue = 0f;

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(5);

            SerializedProperty subEmittersList = subEmitterSettings.FindPropertyRelative("subEmitters");

            EditorGUILayout.LabelField("Sub-Emitter Entries", EditorStyles.boldLabel);

            if (subEmittersList.arraySize == 0)
            {
                EditorGUILayout.HelpBox("No sub-emitters added yet. Click the button below to add.", MessageType.Info);
            }

            for (int i = 0; i < subEmittersList.arraySize; i++)
            {
                SerializedProperty entry = subEmittersList.GetArrayElementAtIndex(i);

                SerializedProperty triggerTypeProp = entry.FindPropertyRelative("triggerType");
                SerializedProperty actionTypeProp = entry.FindPropertyRelative("actionType");

                string triggerName = triggerTypeProp.enumDisplayNames[triggerTypeProp.enumValueIndex];
                string actionName = actionTypeProp.enumDisplayNames[actionTypeProp.enumValueIndex];

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"#{i + 1}  [{triggerName} → {actionName}]", EditorStyles.boldLabel);

                if (GUILayout.Button("✕", GUILayout.Width(22), GUILayout.Height(18)))
                {
                    subEmittersList.DeleteArrayElementAtIndex(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(triggerTypeProp, triggerContent);
                EditorGUILayout.PropertyField(actionTypeProp, actionContent);

                if (triggerTypeProp.enumValueIndex == 2)
                {
                    SerializedProperty thresholdProp = entry.FindPropertyRelative("lifetimeThreshold");
                    EditorGUILayout.Slider(thresholdProp, 0f, 1f, lifetimeThresholdContent);
                }

                int actionIndex = actionTypeProp.enumValueIndex;
                if (actionIndex == 0 || actionIndex == 1 || actionIndex == 2)
                {
                    SerializedProperty targetProp = entry.FindPropertyRelative("targetSystem");
                    EditorGUILayout.PropertyField(targetProp, targetSystemContent);

                    if (targetProp.objectReferenceValue == particleSystem)
                    {
                        EditorGUILayout.HelpBox("Target system cannot be itself! This may cause an infinite loop.", MessageType.Error);
                    }
                }

                if (actionIndex == 0)
                {
                    SerializedProperty poolSizeProp = entry.FindPropertyRelative("instancePoolSize");
                    EditorGUILayout.PropertyField(poolSizeProp, instancePoolSizeContent);
                    if (poolSizeProp.intValue < 1) poolSizeProp.intValue = 8;
                    if (poolSizeProp.intValue > 64) poolSizeProp.intValue = 64;
                    EditorGUILayout.HelpBox("Play System clones the target so overlapping events can play at the same time. Turn off Emit On Start on the template system.", MessageType.Info);
                }

                if (actionIndex == 2)
                {
                    EditorGUILayout.HelpBox("Emit Burst adds particles into the same target system. Increase that system's Max Particles so overlapping events are not dropped.", MessageType.Info);
                }

                SerializedProperty probabilityProp = entry.FindPropertyRelative("probability");
                EditorGUILayout.Slider(probabilityProp, 0f, 1f, probabilityContent);

                if (actionIndex == 0 || actionIndex == 2)
                {
                    EditorGUILayout.Space(2);
                    EditorGUILayout.LabelField("Inherit Properties", EditorStyles.miniLabel);

                    SerializedProperty inheritPosProp = entry.FindPropertyRelative("inheritPosition");
                    EditorGUILayout.PropertyField(inheritPosProp, inheritPositionContent);

                    SerializedProperty inheritColorProp = entry.FindPropertyRelative("inheritColor");
                    EditorGUILayout.PropertyField(inheritColorProp, inheritColorContent);

                    SerializedProperty inheritSizeProp = entry.FindPropertyRelative("inheritSize");
                    EditorGUILayout.PropertyField(inheritSizeProp, inheritSizeContent);
                }

                if (actionIndex == 2)
                {
                    SerializedProperty burstCountProp = entry.FindPropertyRelative("burstCount");
                    EditorGUILayout.PropertyField(burstCountProp, burstCountContent);
                    if (burstCountProp.intValue < 1) burstCountProp.intValue = 1;
                }

                if (actionIndex == 3)
                {
                    SerializedProperty targetColorProp = entry.FindPropertyRelative("targetColor");
                    EditorGUILayout.PropertyField(targetColorProp, targetColorContent);

                    SerializedProperty colorGradientProp = entry.FindPropertyRelative("colorGradient");
                    EditorGUILayout.PropertyField(colorGradientProp, colorGradientContent);
                }

                if (actionIndex == 4)
                {
                    SerializedProperty targetSizeProp = entry.FindPropertyRelative("targetSize");
                    EditorGUILayout.PropertyField(targetSizeProp, targetSizeContent);
                    if (targetSizeProp.floatValue < 0.1f) targetSizeProp.floatValue = 0.1f;
                }

                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(2);
            }

            EditorGUILayout.Space(3);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("+ Add Sub-Emitter", GUILayout.Width(160), GUILayout.Height(24)))
            {
                subEmittersList.InsertArrayElementAtIndex(subEmittersList.arraySize);
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void RenderVelocityOverLifetimeSettings(SerializedObject serializedObject)
        {
            SerializedProperty velocityOverLifetimeSettings = serializedObject.FindProperty("velocityOverLifetimeSettings");
            SerializedProperty useVelocityOverLifetimeProp = velocityOverLifetimeSettings.FindPropertyRelative("useVelocityOverLifetime");

            EditorGUILayout.BeginVertical(styles.SettingsStyle);
            EditorGUILayout.BeginVertical(styles.Titlestyle);
            EditorGUILayout.BeginHorizontal();

            GUIContent velocityIcon = UIParticleSystemEditorIcons.GetIcon("VelocityOverLifetime");
            GUILayout.Label(velocityIcon, GUILayout.Width(16), GUILayout.Height(16));

            bool currentValue = useVelocityOverLifetimeProp.boolValue;
            bool newValue = EditorGUILayout.Toggle(currentValue, GUILayout.Width(12));
            if (newValue != currentValue)
            {
                useVelocityOverLifetimeProp.boolValue = newValue;
            }

            showVelocityOverLifetimeSettings ^= GUILayout.Button("Velocity Over Lifetime", styles.TransparentToggleStyle);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            if (showVelocityOverLifetimeSettings)
            {
                EditorGUI.indentLevel++;
                utils.RenderVelocityOverLifetimeControls(velocityOverLifetimeSettings);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }

        private void RenderLimitVelocityOverLifetimeSettings(SerializedObject serializedObject)
        {
            SerializedProperty limitVelocitySettings = serializedObject.FindProperty("limitVelocityOverLifetimeSettings");
            if (limitVelocitySettings == null) return;

            SerializedProperty useLimitProp = limitVelocitySettings.FindPropertyRelative("useLimitVelocityOverLifetime");

            EditorGUILayout.BeginVertical(styles.SettingsStyle);
            EditorGUILayout.BeginVertical(styles.Titlestyle);
            EditorGUILayout.BeginHorizontal();

            GUIContent limitVelocityIcon = UIParticleSystemEditorIcons.GetIcon("VelocityOverLifetime");
            GUILayout.Label(limitVelocityIcon, GUILayout.Width(16), GUILayout.Height(16));

            bool currentValue = useLimitProp.boolValue;
            bool newValue = EditorGUILayout.Toggle(currentValue, GUILayout.Width(12));
            if (newValue != currentValue)
                useLimitProp.boolValue = newValue;

            showLimitVelocityOverLifetimeSettings ^= GUILayout.Button("Limit Velocity over Lifetime", styles.TransparentToggleStyle);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            if (showLimitVelocityOverLifetimeSettings)
            {
                EditorGUI.indentLevel++;
                utils.RenderLimitVelocityOverLifetimeControls(limitVelocitySettings);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }

        private void RenderInheritVelocitySettings(SerializedObject serializedObject)
        {
            SerializedProperty inheritSettings = serializedObject.FindProperty("inheritVelocitySettings");
            if (inheritSettings == null) return;

            SerializedProperty useInheritProp = inheritSettings.FindPropertyRelative("useInheritVelocity");

            EditorGUILayout.BeginVertical(styles.SettingsStyle);
            EditorGUILayout.BeginVertical(styles.Titlestyle);
            EditorGUILayout.BeginHorizontal();

            GUIContent inheritIcon = UIParticleSystemEditorIcons.GetIcon("InheritVelocity");
            if (inheritIcon == null || inheritIcon.image == null)
                inheritIcon = EditorGUIUtility.IconContent("Rigidbody Icon");
            GUILayout.Label(inheritIcon, GUILayout.Width(16), GUILayout.Height(16));

            bool currentValue = useInheritProp.boolValue;
            bool newValue = EditorGUILayout.Toggle(currentValue, GUILayout.Width(12));
            if (newValue != currentValue)
                useInheritProp.boolValue = newValue;

            showInheritVelocitySettings ^= GUILayout.Button("Inherit Velocity", styles.TransparentToggleStyle);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            if (showInheritVelocitySettings)
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginDisabledGroup(!useInheritProp.boolValue);
                utils.RenderInheritVelocityControls(inheritSettings);
                EditorGUI.EndDisabledGroup();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }

        private void RenderForceOverLifetimeSettings(SerializedObject serializedObject)
        {
            SerializedProperty forceSettings = serializedObject.FindProperty("forceOverLifetimeSettings");
            if (forceSettings == null) return;

            SerializedProperty useForceProp = forceSettings.FindPropertyRelative("useForceOverLifetime");

            EditorGUILayout.BeginVertical(styles.SettingsStyle);
            EditorGUILayout.BeginVertical(styles.Titlestyle);
            EditorGUILayout.BeginHorizontal();

            GUIContent forceIcon = UIParticleSystemEditorIcons.GetIcon("VelocityOverLifetime");
            GUILayout.Label(forceIcon, GUILayout.Width(16), GUILayout.Height(16));

            bool currentValue = useForceProp.boolValue;
            bool newValue = EditorGUILayout.Toggle(currentValue, GUILayout.Width(12));
            if (newValue != currentValue)
                useForceProp.boolValue = newValue;

            showForceOverLifetimeSettings ^= GUILayout.Button("Force over Lifetime", styles.TransparentToggleStyle);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            if (showForceOverLifetimeSettings)
            {
                EditorGUI.indentLevel++;
                utils.RenderForceOverLifetimeControls(forceSettings);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }

        private void RenderCollisionSettings(SerializedObject serializedObject)
        {
            SerializedProperty collisionSettings = serializedObject.FindProperty("collisionSettings");
            if (collisionSettings == null) return;

            SerializedProperty useCollisionProp = collisionSettings.FindPropertyRelative("useCollision");

            EditorGUILayout.BeginVertical(styles.SettingsStyle);
            EditorGUILayout.BeginVertical(styles.Titlestyle);
            EditorGUILayout.BeginHorizontal();

            GUIContent collisionIcon = UIParticleSystemEditorIcons.GetIcon("Collision");
            if (collisionIcon == null || collisionIcon.image == null)
                collisionIcon = EditorGUIUtility.IconContent("BoxCollider2D Icon");
            GUILayout.Label(collisionIcon, GUILayout.Width(16), GUILayout.Height(16));

            bool currentValue = useCollisionProp.boolValue;
            bool newValue = EditorGUILayout.Toggle(currentValue, GUILayout.Width(12));
            if (newValue != currentValue)
                useCollisionProp.boolValue = newValue;

            showCollisionSettings ^= GUILayout.Button("Collision", styles.TransparentToggleStyle);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            if (showCollisionSettings)
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginDisabledGroup(!useCollisionProp.boolValue);
                utils.RenderCollisionControls(collisionSettings);
                EditorGUI.EndDisabledGroup();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }


        private void RenderControlPanel()
        {
            EditorGUILayout.BeginVertical(styles.SettingsStyle);

            EditorGUILayout.BeginHorizontal();

            if (isMultiObjectEditing && parentEditor != null)
            {
                bool anyPlaying = parentEditor.IsAnySystemPlaying();

                string playButtonText = anyPlaying ? "Pause All" : "Play All";
                if (GUILayout.Button(playButtonText, GUILayout.Height(30)))
                {
                    if (anyPlaying)
                        parentEditor.PauseAllSelectedSystems();
                    else
                        parentEditor.PlayAllSelectedSystems();
                }

                if (GUILayout.Button("Stop All", GUILayout.Height(30)))
                {
                    parentEditor.StopAllSelectedSystems();
                }

                if (GUILayout.Button("Restart All", GUILayout.Height(30)))
                {
                    parentEditor.RestartAllSelectedSystems();
                }
            }
            else
            {
                string playButtonText = controls.IsPlaying ? "Pause" : "Play";
                if (GUILayout.Button(playButtonText, GUILayout.Height(30)))
                {
                    controls.PlayParticleSystem();
                }

                if (GUILayout.Button("Stop", GUILayout.Height(30)))
                {
                    controls.StopParticleSystem();
                }

                if (GUILayout.Button("Restart", GUILayout.Height(30)))
                {
                    controls.RestartParticleSystem();
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (!isMultiObjectEditing)
            {
                EditorGUILayout.BeginHorizontal();
                string sceneViewButtonText = controls.ShowSceneViewControls ? "Hide Scene View Panel" : "Show Scene View Panel";
                if (GUILayout.Button(sceneViewButtonText, GUILayout.Height(25)))
                {
                    controls.ToggleSceneViewControls();
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

        public void RenderSceneView(SceneView sceneView)
        {
            if (particleSystem == null || !controls.ShowSceneViewControls)
                return;

            controls.HandleSceneViewControls(sceneView, styles);
        }
    }
}
#endif