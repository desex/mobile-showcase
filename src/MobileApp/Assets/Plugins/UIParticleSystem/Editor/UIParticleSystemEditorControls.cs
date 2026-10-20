#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;
using TierStudios.UIParticleSystem.Enums;

namespace TierStudios.UIParticleSystem.Editor
{
    public class UIParticleSystemEditorControls
    {
        private Core.UIParticleSystem particleSystem;
        private UIParticleSystemEditorUtils utils;

        private bool isMultiObjectMode = false;
        private UIParticleSystemEditor parentEditor = null;

        private bool isPlaying = false;
        private float runtime = 0f;
        private float lastRepaintTime = 0f;
        private float lastUpdateTime = 0f;
        private float systemStartTime = 0f;
        private const float REPAINT_INTERVAL = 0.05f;
        private const float UPDATE_INTERVAL = 0.05f;

        private static bool showSceneViewControls = true;
        private static bool previewMinimized = false;
        private static Rect sceneViewRect = new Rect(10, 10, 244, 176);
        private static bool isDraggingPanel = false;
        private static Vector2 sceneViewGuiSize = new Vector2(800f, 600f);
        private const float PreviewEdgeMargin = 10f;

        [InitializeOnEnterPlayMode]
        private static void ResetOnEnterPlayMode(EnterPlayModeOptions options)
        {
            showSceneViewControls = true;
            previewMinimized = false;
            sceneViewRect = new Rect(10, 10, 244, 176);
            isDraggingPanel = false;
        }

        private Texture2D playIcon;
        private Texture2D pauseIcon;
        private Texture2D stopIcon;
        private Texture2D restartIcon;
        private Texture2D previewLogo;
        private bool iconsLoaded = false;

        private GUIStyle previewTitleStyle;
        private GUIStyle previewMutedStyle;
        private GUIStyle previewValueStyle;
        private GUIStyle previewStatusStyle;
        private GUIStyle previewChipStyle;
        private GUIStyle previewHeaderButtonStyle;

        public bool IsPlaying => isPlaying;
        public bool ShowSceneViewControls => showSceneViewControls;
        public string FormattedRuntime => FormatTime(runtime);

        public UIParticleSystemEditorControls(Core.UIParticleSystem system, UIParticleSystemEditorUtils utils)
        {
            this.particleSystem = system;
            this.utils = utils;
        }

        public void Initialize()
        {
            if (particleSystem != null)
            {
                bool systemIsActive = particleSystem.IsEmitting && !particleSystem.IsPaused;

                if (systemIsActive != isPlaying)
                {
                    isPlaying = systemIsActive;

                    if (isPlaying)
                    {
                        runtime = 0f;
                        systemStartTime = (float)EditorApplication.timeSinceStartup;
                    }
                }
            }

            if (!iconsLoaded)
            {
                LoadIcons();
                iconsLoaded = true;
            }
        }

        public bool ShouldRepaint()
        {
            if (EditorWindow.focusedWindow != null)
            {
                string windowType = EditorWindow.focusedWindow.GetType().Name;
                if (windowType == "CurveEditorWindow" || windowType == "AnimationWindow")
                {
                    return false;
                }
            }

            if (particleSystem != null)
            {
                bool systemIsActive = particleSystem.IsEmitting && !particleSystem.IsPaused;
                if (systemIsActive != isPlaying)
                {
                    return true;
                }
            }

            if (!isPlaying || particleSystem == null) return false;

            float currentTime = (float)EditorApplication.timeSinceStartup;
            bool shouldRepaint = currentTime - lastRepaintTime >= REPAINT_INTERVAL;

            if (shouldRepaint)
            {
                lastRepaintTime = currentTime;
            }

            return shouldRepaint;
        }

        public void UpdatePreview()
        {
            float currentTime = (float)EditorApplication.timeSinceStartup;

            if (currentTime - lastUpdateTime < UPDATE_INTERVAL)
                return;

            lastUpdateTime = currentTime;

            bool systemIsActive = particleSystem != null &&
                                 particleSystem.IsEmitting &&
                                 !particleSystem.IsPaused;

            if (systemIsActive != isPlaying)
            {
                isPlaying = systemIsActive;

                if (isPlaying)
                {
                    runtime = 0f;
                    systemStartTime = currentTime;
                }

                EditorUtility.SetDirty(particleSystem);
            }

            if (isPlaying)
            {
                runtime = currentTime - systemStartTime;

                if (particleSystem.EmissionSettings.duration > 0 && particleSystem.EmissionSettings.looping)
                {
                    if (runtime >= particleSystem.EmissionSettings.duration)
                    {
                        systemStartTime = currentTime;
                        runtime = 0f;
                    }
                }

                else if (particleSystem.EmissionSettings.duration > 0 && !particleSystem.EmissionSettings.looping)
                {
                    if (runtime >= particleSystem.EmissionSettings.duration)
                    {
                        runtime = particleSystem.EmissionSettings.duration;
                        isPlaying = false;
                    }
                }

                if (currentTime - lastRepaintTime >= REPAINT_INTERVAL)
                {
                    SceneView.RepaintAll();
                    lastRepaintTime = currentTime;
                }
            }
        }

        public void SetMultiObjectState(bool isMultiMode, UIParticleSystemEditor editor = null)
        {
            isMultiObjectMode = isMultiMode;
            parentEditor = editor;
        }

        private Core.UIParticleSystem[] GetChildParticleSystems()
        {
            if (particleSystem == null || particleSystem.transform == null)
                return new Core.UIParticleSystem[0];

            return particleSystem.transform.GetComponentsInChildren<Core.UIParticleSystem>(true)
                .Where(childSystem => childSystem != particleSystem)
                .ToArray();
        }

        private void ExecuteOnSystemAndChildren(System.Action<Core.UIParticleSystem> action)
        {
            if (particleSystem == null || action == null)
                return;

            action(particleSystem);

            var childSystems = GetChildParticleSystems();
            foreach (var childSystem in childSystems)
            {
                if (childSystem != null)
                {
                    action(childSystem);
                }
            }
        }

        private void ConfigureChildSystemsForEditor(bool enable)
        {
            var childSystems = GetChildParticleSystems();
            foreach (var childSystem in childSystems)
            {
                if (childSystem != null)
                {
                    childSystem.EmissionSettings.previewInEditor = enable;
                    childSystem.EmissionSettings.pauseWhenDeselected = false;

                    EditorUtility.SetDirty(childSystem);
                }
            }
        }

        public void PlayParticleSystem()
        {
            if (isMultiObjectMode && parentEditor != null)
            {
                bool anyPlaying = parentEditor.IsAnySystemPlaying();
                if (anyPlaying)
                    parentEditor.PauseAllSelectedSystems();
                else
                    parentEditor.PlayAllSelectedSystems();
            }
            else
            {
                bool mainSystemPlaying = particleSystem.IsEmitting && !particleSystem.IsPaused;

                if (!mainSystemPlaying)
                {
                    ConfigureChildSystemsForEditor(true);

                    ExecuteOnSystemAndChildren(system =>
                    {
                        if (system != particleSystem)
                        {
                            var emissionManager = GetEmissionManager(system);
                            if (emissionManager != null)
                                emissionManager.HandleEditorSelection(true);
                        }

                        if (system.IsPaused)
                        {
                            system.ResumeSystem();
                        }
                        else
                        {
                            system.StartEmitting();
                        }
                    });

                    runtime = 0f;
                    systemStartTime = (float)EditorApplication.timeSinceStartup;
                    isPlaying = true;
                }
                else
                    PauseParticleSystem();
            }
        }

        public void PauseParticleSystem()
        {
            if (isMultiObjectMode && parentEditor != null)
            {
                parentEditor.PauseAllSelectedSystems();
            }
            else
            {
                ExecuteOnSystemAndChildren(system =>
                {
                    system.PauseSystem();
                });

                isPlaying = false;
            }
        }

        public void StopParticleSystem()
        {
            if (isMultiObjectMode && parentEditor != null)
            {
                parentEditor.StopAllSelectedSystems();
            }
            else
            {
                ExecuteOnSystemAndChildren(system =>
                {
                    system.StopEmitting();
                    Core.UIParticleSystem.ClearAllParticles(system);

                    system.CleanupTrails();
                });

                ConfigureChildSystemsForEditor(false);

                runtime = 0f;
                systemStartTime = (float)EditorApplication.timeSinceStartup;
                isPlaying = false;
                SceneView.RepaintAll();
                EditorUtility.SetDirty(particleSystem);
            }
        }


        public void RestartParticleSystem()
        {
            if (isMultiObjectMode && parentEditor != null)
            {
                parentEditor.RestartAllSelectedSystems();
            }
            else
            {
                ExecuteOnSystemAndChildren(system =>
                {
                    system.CleanupTrails();
                });

                ConfigureChildSystemsForEditor(true);

                ExecuteOnSystemAndChildren(system =>
                {
                    if (system != particleSystem)
                    {
                        var emissionManager = GetEmissionManager(system);
                        if (emissionManager != null)
                        {
                            emissionManager.HandleEditorSelection(true);
                        }
                    }

                    system.RestartSystem();
                });

                runtime = 0f;
                systemStartTime = (float)EditorApplication.timeSinceStartup;
                isPlaying = particleSystem.IsEmitting && !particleSystem.IsPaused;
            }
        }

        private dynamic GetEmissionManager(Core.UIParticleSystem system)
        {
            try
            {
                var field = typeof(Core.UIParticleSystem).GetField("emissionManager",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                return field?.GetValue(system);
            }
            catch
            {
                return null;
            }
        }

        public void ToggleSceneViewControls()
        {
            showSceneViewControls = !showSceneViewControls;
            SceneView.RepaintAll();
        }

        public void HandleSceneViewControls(SceneView sceneView, UIParticleSystemEditorStyles styles)
        {
            if (particleSystem == null || !showSceneViewControls || isMultiObjectMode)
                return;

            EnsurePreviewStyles();

            bool systemIsActive = particleSystem.IsEmitting && !particleSystem.IsPaused;
            if (systemIsActive != isPlaying)
            {
                isPlaying = systemIsActive;
                if (isPlaying && runtime == 0f)
                    systemStartTime = (float)EditorApplication.timeSinceStartup;
            }

            if (isPlaying)
            {
                float currentTime = (float)EditorApplication.timeSinceStartup;
                runtime = currentTime - systemStartTime;

                if (particleSystem.EmissionSettings.duration > 0)
                {
                    if (particleSystem.EmissionSettings.looping)
                    {
                        if (runtime >= particleSystem.EmissionSettings.duration)
                        {
                            systemStartTime = currentTime;
                            runtime = 0f;
                        }
                    }
                    else if (runtime >= particleSystem.EmissionSettings.duration)
                    {
                        runtime = particleSystem.EmissionSettings.duration;
                        isPlaying = false;
                    }
                }
            }

            var childSystems = GetChildParticleSystems();
            bool hasChildren = childSystems.Length > 0;
            string[] moduleChips = previewMinimized ? new string[0] : GetActiveModuleChips();
            float panelWidth = 244f;
            float panelHeight = previewMinimized
                ? 102f
                : 160f + (moduleChips.Length > 0 ? 24f : 0f) + (hasChildren ? 34f : 0f);

            Handles.BeginGUI();

            sceneViewGuiSize = GetSceneViewDrawSize(sceneView);
            Rect panelRect = ClampPreviewRect(
                isDraggingPanel
                    ? new Rect(sceneViewRect.x, sceneViewRect.y, panelWidth, panelHeight)
                    : new Rect(
                        sceneViewGuiSize.x - panelWidth - PreviewEdgeMargin,
                        sceneViewGuiSize.y - panelHeight - PreviewEdgeMargin,
                        panelWidth,
                        panelHeight));

            sceneViewRect = panelRect;

            DrawPreviewPanel(childSystems, moduleChips);
            BlockSceneViewClicks(sceneViewRect);

            Handles.EndGUI();

            if (isPlaying)
                SceneView.RepaintAll();
        }

        private static Vector2 GetSceneViewDrawSize(SceneView sceneView)
        {
            float toolbarHeight = EditorStyles.toolbar != null ? EditorStyles.toolbar.fixedHeight : 21f;
            Vector2 windowSize = sceneView != null ? sceneView.position.size : new Vector2(Screen.width, Screen.height);

            Camera cam = sceneView != null ? sceneView.camera : Camera.current;
            if (cam != null)
            {
                float pixelsPerPoint = Mathf.Max(0.01f, EditorGUIUtility.pixelsPerPoint);
                Rect pixelRect = cam.pixelRect;
                float width = pixelRect.width / pixelsPerPoint;
                float height = pixelRect.height / pixelsPerPoint;

                if (width > 1f && height > 1f)
                {
                    if (windowSize.x > 1f && width > windowSize.x * 1.6f)
                    {
                        width = windowSize.x;
                        height = Mathf.Max(1f, windowSize.y - toolbarHeight);
                    }

                    return new Vector2(width, height);
                }
            }

            return new Vector2(windowSize.x, Mathf.Max(1f, windowSize.y - toolbarHeight));
        }

        private static Rect ClampPreviewRect(Rect rect)
        {
            float maxX = Mathf.Max(PreviewEdgeMargin, sceneViewGuiSize.x - rect.width - PreviewEdgeMargin);
            float maxY = Mathf.Max(PreviewEdgeMargin, sceneViewGuiSize.y - rect.height - PreviewEdgeMargin);
            rect.x = Mathf.Clamp(rect.x, PreviewEdgeMargin, maxX);
            rect.y = Mathf.Clamp(rect.y, PreviewEdgeMargin, maxY);
            return rect;
        }

        private void DrawPreviewPanel(Core.UIParticleSystem[] childSystems, string[] moduleChips)
        {
            Color panelBg = new Color(0.07f, 0.07f, 0.075f, 0.97f);
            Color headerBg = new Color(0.10f, 0.10f, 0.105f, 1f);
            Color border = new Color(1f, 1f, 1f, 0.08f);
            Color accent = new Color(0.78f, 0.78f, 0.80f, 1f);

            EditorGUI.DrawRect(sceneViewRect, panelBg);
            DrawRectBorder(sceneViewRect, border);

            Rect headerRect = new Rect(sceneViewRect.x, sceneViewRect.y, sceneViewRect.width, 30f);
            EditorGUI.DrawRect(headerRect, headerBg);
            EditorGUI.DrawRect(new Rect(headerRect.x, headerRect.yMax - 1f, headerRect.width, 1f), border);

            HandlePanelDragging(new Rect(headerRect.x, headerRect.y, headerRect.width - 28f, headerRect.height));

            Color stateColor = GetStateColor();
            Rect logoRect = new Rect(headerRect.x + 8f, headerRect.y + 6f, 18f, 18f);
            if (previewLogo != null)
            {
                Color previous = GUI.color;
                GUI.color = stateColor;
                GUI.DrawTexture(logoRect, previewLogo, ScaleMode.ScaleToFit, true);
                GUI.color = previous;
            }

            GUI.Label(new Rect(headerRect.x + 32f, headerRect.y + 6f, 150f, 18f), "PARTICLE PREVIEW", previewTitleStyle);

            Rect toggleRect = new Rect(headerRect.xMax - 24f, headerRect.y + 6f, 16f, 16f);
            if (DrawHeaderButton(toggleRect, previewMinimized, previewMinimized ? "Expand" : "Minimize"))
                previewMinimized = !previewMinimized;

            float y = headerRect.yMax + 6f;
            float x = sceneViewRect.x + 10f;
            float innerWidth = sceneViewRect.width - 20f;

            DrawTransportButtons(new Rect(x, y, innerWidth, 22f));
            y += 26f;

            float duration = particleSystem.EmissionSettings.duration;
            float progress = duration > 0f ? Mathf.Clamp01(runtime / duration) : (isPlaying ? 1f : 0f);
            string durationText = duration > 0f ? $" / {FormatTime(duration)}" : "";
            GUI.Label(new Rect(x, y, innerWidth, 16f), $"{FormatTime(runtime)}{durationText}", previewMutedStyle);
            y += 16f;
            DrawProgressBar(new Rect(x, y, innerWidth, 4f), progress, accent);

            if (previewMinimized)
                return;

            y += 6f;

            int particles = particleSystem.GetActiveParticleCount();
            int maxParticles = particleSystem.EffectiveMaxParticles;
            DrawStatRow(x, ref y, innerWidth, "Particles", $"{particles} / {maxParticles}");
            DrawStatRow(x, ref y, innerWidth, "Blend", particleSystem.CurrentBlendMode == BlendMode.Additive ? "Additive" : "Alpha");
            DrawStatRow(x, ref y, innerWidth, "Space", particleSystem.EmissionSettings.simulationSpace.ToString());
            DrawStatRow(x, ref y, innerWidth, "Loop", particleSystem.EmissionSettings.looping ? "On" : "Off");

            if (moduleChips.Length > 0)
            {
                y += 4f;
                DrawModuleChips(new Rect(x, y, innerWidth, 20f), moduleChips, accent);
                y += 24f;
            }

            if (childSystems.Length > 0)
            {
                int childParticles = childSystems.Sum(child => child != null ? child.GetActiveParticleCount() : 0);
                int playingChildren = childSystems.Count(child => child != null && child.IsEmitting && !child.IsPaused);
                DrawStatRow(x, ref y, innerWidth, "Children", $"{playingChildren} / {childSystems.Length} playing");
                DrawStatRow(x, ref y, innerWidth, "Child Particles", childParticles.ToString());
            }
        }

        private string[] GetActiveModuleChips()
        {
            var chips = new System.Collections.Generic.List<string>(4);
            if (particleSystem.TrailSettings != null && particleSystem.TrailSettings.useTrails)
                chips.Add("Trails");
            if (particleSystem.NoiseSettings != null && particleSystem.NoiseSettings.useNoise)
                chips.Add("Noise");
            if (particleSystem.AttractorSettings != null && particleSystem.AttractorSettings.useAttractor)
                chips.Add("Attractor");
            if (particleSystem.CollisionSettings != null && particleSystem.CollisionSettings.useCollision)
                chips.Add("Collision");
            if (particleSystem.TextureSheetSettings != null && particleSystem.TextureSheetSettings.useTextureSheetAnimation)
                chips.Add("Sheet");
            if (particleSystem.SubEmitterSettings != null && particleSystem.SubEmitterSettings.EnableSubEmitters)
                chips.Add("Sub");
            return chips.ToArray();
        }

        private void DrawTransportButtons(Rect rect)
        {
            float buttonWidth = 24f;
            float gap = 4f;
            Rect playRect = new Rect(rect.x, rect.y, buttonWidth, rect.height);
            Rect stopRect = new Rect(playRect.xMax + gap, rect.y, buttonWidth, rect.height);
            Rect restartRect = new Rect(stopRect.xMax + gap, rect.y, buttonWidth, rect.height);

            Texture playPauseIcon = isPlaying ? pauseIcon : playIcon;
            string playPauseTooltip = isPlaying ? "Pause" : "Play";

            if (DrawIconButton(playRect, playPauseIcon, playPauseTooltip, isPlaying))
                PlayParticleSystem();

            if (DrawIconButton(stopRect, stopIcon, "Stop", false))
                StopParticleSystem();

            if (DrawIconButton(restartRect, restartIcon, "Restart", false))
                RestartParticleSystem();

            string status = isPlaying ? "PLAYING" : particleSystem.IsPaused ? "PAUSED" : "STOPPED";
            Color statusColor = isPlaying
                ? new Color(0.45f, 0.82f, 0.52f)
                : particleSystem.IsPaused
                    ? new Color(0.95f, 0.80f, 0.25f)
                    : new Color(0.92f, 0.34f, 0.34f);
            Color old = previewStatusStyle.normal.textColor;
            previewStatusStyle.normal.textColor = statusColor;
            GUI.Label(new Rect(restartRect.xMax + 10f, rect.y + 5f, 90f, 18f), status, previewStatusStyle);
            previewStatusStyle.normal.textColor = old;
        }

        private bool DrawIconButton(Rect rect, Texture icon, string tooltip, bool active)
        {
            Event e = Event.current;
            bool hover = rect.Contains(e.mousePosition);
            bool pressed = hover && e.type == EventType.MouseDown && e.button == 0;

            Color bg = active
                ? new Color(0.24f, 0.24f, 0.255f, 1f)
                : pressed
                    ? new Color(0.20f, 0.20f, 0.21f, 1f)
                    : hover
                        ? new Color(0.17f, 0.17f, 0.18f, 1f)
                        : new Color(0.13f, 0.13f, 0.14f, 1f);

            Color border = active
                ? new Color(1f, 1f, 1f, 0.22f)
                : hover
                    ? new Color(1f, 1f, 1f, 0.14f)
                    : new Color(1f, 1f, 1f, 0.07f);

            EditorGUI.DrawRect(rect, bg);
            DrawRectBorder(rect, border);

            if (icon != null)
            {
                const float iconSize = 11f;
                Rect iconRect = new Rect(
                    rect.x + (rect.width - iconSize) * 0.5f,
                    rect.y + (rect.height - iconSize) * 0.5f,
                    iconSize,
                    iconSize);

                Color previous = GUI.color;
                GUI.color = active ? Color.white : new Color(0.82f, 0.82f, 0.84f, 0.95f);
                GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit, true);
                GUI.color = previous;
            }

            return GUI.Button(rect, new GUIContent(string.Empty, tooltip), GUIStyle.none);
        }

        private void DrawStatRow(float x, ref float y, float width, string label, string value)
        {
            GUI.Label(new Rect(x, y, width * 0.46f, 16f), label, previewMutedStyle);
            GUI.Label(new Rect(x + width * 0.46f, y, width * 0.54f, 16f), value, previewValueStyle);
            y += 17f;
        }

        private void DrawProgressBar(Rect rect, float progress, Color fill)
        {
            EditorGUI.DrawRect(rect, new Color(1f, 1f, 1f, 0.06f));
            if (progress > 0f)
                EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width * progress, rect.height), fill);
        }

        private void DrawModuleChips(Rect rect, string[] chips, Color accent)
        {
            float x = rect.x;
            foreach (string chip in chips)
            {
                Vector2 size = previewChipStyle.CalcSize(new GUIContent(chip));
                Rect chipRect = new Rect(x, rect.y, size.x + 12f, 18f);
                EditorGUI.DrawRect(chipRect, new Color(accent.r, accent.g, accent.b, 0.16f));
                GUI.Label(chipRect, chip, previewChipStyle);
                x += chipRect.width + 4f;
                if (x > rect.xMax - 30f)
                    break;
            }
        }

        private static void DrawRectBorder(Rect rect, Color color)
        {
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, 1f), color);
            EditorGUI.DrawRect(new Rect(rect.x, rect.yMax - 1f, rect.width, 1f), color);
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, 1f, rect.height), color);
            EditorGUI.DrawRect(new Rect(rect.xMax - 1f, rect.y, 1f, rect.height), color);
        }

        private void EnsurePreviewStyles()
        {
            if (previewTitleStyle != null)
                return;

            Color primary = new Color(0.88f, 0.88f, 0.89f);
            Color muted = new Color(0.55f, 0.55f, 0.57f);

            previewTitleStyle = new GUIStyle(EditorStyles.miniBoldLabel)
            {
                fontSize = 10,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = primary }
            };

            previewMutedStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                fontSize = 10,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = muted }
            };

            previewValueStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                fontSize = 10,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleRight,
                normal = { textColor = primary }
            };

            previewStatusStyle = new GUIStyle(EditorStyles.miniBoldLabel)
            {
                fontSize = 9,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = new Color(0.72f, 0.72f, 0.74f) }
            };

            previewHeaderButtonStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                normal = { textColor = muted }
            };

            previewChipStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                fontSize = 9,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.80f, 0.80f, 0.82f) }
            };
        }

        private Color GetStateColor()
        {
            if (isPlaying)
                return new Color(0.35f, 0.82f, 0.45f);

            if (particleSystem != null && particleSystem.IsPaused)
                return new Color(0.95f, 0.80f, 0.25f);

            if (particleSystem != null && particleSystem.WasEverStarted)
                return new Color(0.90f, 0.32f, 0.32f);

            return new Color(0x7E / 255f, 0f, 1f);
        }

        private bool DrawHeaderButton(Rect rect, bool minimized, string tooltip)
        {
            Event e = Event.current;
            bool hover = rect.Contains(e.mousePosition);

            EditorGUI.DrawRect(rect, hover ? new Color(0.18f, 0.18f, 0.19f, 1f) : new Color(0.13f, 0.13f, 0.14f, 1f));
            DrawRectBorder(rect, hover ? new Color(1f, 1f, 1f, 0.14f) : new Color(1f, 1f, 1f, 0.07f));

            Color iconColor = hover ? new Color(0.92f, 0.92f, 0.94f) : new Color(0.72f, 0.72f, 0.74f);
            float cx = rect.x + rect.width * 0.5f;
            float cy = rect.y + rect.height * 0.5f;

            if (minimized)
            {
                const float size = 7f;
                DrawRectBorder(new Rect(cx - size * 0.5f, cy - size * 0.5f, size, size), iconColor);
            }
            else
            {
                EditorGUI.DrawRect(new Rect(cx - 4f, cy - 0.75f, 8f, 1.5f), iconColor);
            }

            return GUI.Button(rect, new GUIContent(string.Empty, tooltip), GUIStyle.none);
        }

        private static void BlockSceneViewClicks(Rect panelRect)
        {
            Event e = Event.current;
            if (e == null) return;

            Rect hitRect = new Rect(panelRect.x - 4f, panelRect.y - 4f, panelRect.width + 8f, panelRect.height + 8f);
            bool overPanel = hitRect.Contains(e.mousePosition) || isDraggingPanel;
            if (!overPanel) return;

            int controlId = GUIUtility.GetControlID(FocusType.Passive);
            HandleUtility.AddDefaultControl(controlId);

            switch (e.GetTypeForControl(controlId))
            {
                case EventType.MouseDown:
                    GUIUtility.hotControl = controlId;
                    e.Use();
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlId)
                        GUIUtility.hotControl = 0;
                    e.Use();
                    break;
                case EventType.MouseDrag:
                case EventType.ScrollWheel:
                case EventType.ContextClick:
                    e.Use();
                    break;
            }
        }

        private void HandlePanelDragging(Rect headerRect)
        {
            Event e = Event.current;
            int controlId = GUIUtility.GetControlID(FocusType.Passive);

            switch (e.GetTypeForControl(controlId))
            {
                case EventType.MouseDown:
                    if (headerRect.Contains(e.mousePosition) && e.button == 0)
                    {
                        GUIUtility.hotControl = controlId;
                        isDraggingPanel = true;
                        e.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlId && isDraggingPanel)
                    {
                        sceneViewRect.x += e.delta.x;
                        sceneViewRect.y += e.delta.y;
                        sceneViewRect = ClampPreviewRect(sceneViewRect);
                        e.Use();
                    }
                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlId)
                    {
                        GUIUtility.hotControl = 0;
                        isDraggingPanel = false;
                        e.Use();
                    }
                    break;
            }
        }

        private void LoadIcons()
        {
            playIcon = GetBuiltinIcon("d_PlayButton", "PlayButton");
            pauseIcon = GetBuiltinIcon("d_PauseButton", "PauseButton");
            stopIcon = UIParticleSystemEditorIcons.GetIconTexture("Stop")
                       ?? GetBuiltinIcon("d_PreMatQuad", "PreMatQuad")
                       ?? GetBuiltinIcon("StopButton", "animation.nextkey");
            restartIcon = GetBuiltinIcon("d_Animation.FirstKey", "Animation.FirstKey");
            previewLogo = UIParticleSystemEditorIcons.GetIconTexture("UIParticleSystemIcon@2xWhite")
                          ?? UIParticleSystemEditorIcons.GetIconTexture("UIParticleSystemIcon");
        }

        private static Texture2D GetBuiltinIcon(params string[] names)
        {
            foreach (string name in names)
            {
                Texture2D fromContent = EditorGUIUtility.IconContent(name).image as Texture2D;
                if (fromContent != null)
                    return fromContent;

                Texture2D fromFind = EditorGUIUtility.FindTexture(name);
                if (fromFind != null)
                    return fromFind;
            }

            return null;
        }

        private string FormatTime(float time)
        {
            int minutes = (int)(time / 60f);
            int seconds = (int)(time % 60f);
            int milliseconds = (int)((time * 100f) % 100f);

            if (minutes > 0)
                return $"{minutes}:{seconds:00}.{milliseconds:00}";
            else
                return $"{seconds}.{milliseconds:00}";
        }
    }
}
#endif
