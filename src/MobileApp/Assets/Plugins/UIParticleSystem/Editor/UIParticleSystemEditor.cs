#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TierStudios.UIParticleSystem.Editor
{
    [CustomEditor(typeof(Core.UIParticleSystem), true)]
    [CanEditMultipleObjects]
    public class UIParticleSystemEditor : UnityEditor.Editor
    {
        private UIParticleSystemEditorGUI guiRenderer;
        private UIParticleSystemEditorControls controls;
        private UIParticleSystemEditorStyles styles;
        private UIParticleSystemEditorUtils utils;

        private Core.UIParticleSystem particleSystem;
        private Core.UIParticleSystem[] multiSelectedSystems;
        private Dictionary<Core.UIParticleSystem, UIParticleSystemEditorControls> systemControls;
        private bool isUpdateListenerActive = false;
        private bool isMultiObjectEditing = false;

        private double lastRepaintTime = 0;
        private const double INSPECTOR_REPAINT_INTERVAL = 0.05;

        private void OnEnable()
        {
            if (target == null)
                return;

            particleSystem = target as Core.UIParticleSystem;

            if (particleSystem == null)
                return;

            CheckMultiObjectSelection();

            styles = new UIParticleSystemEditorStyles();
            utils = new UIParticleSystemEditorUtils();
            controls = new UIParticleSystemEditorControls(particleSystem, utils);

            InitializeSystemControls();

            guiRenderer = new UIParticleSystemEditorGUI(particleSystem, styles, controls, utils, isMultiObjectEditing, multiSelectedSystems, this);

            controls.Initialize();
            controls.SetMultiObjectState(isMultiObjectEditing, this);

            if (particleSystem != null)
            {
                particleSystem.enabled = true;
                utils.ValidateAnimationCurves(particleSystem);

                if (isMultiObjectEditing)
                {
                    particleSystem.EmissionSettings.pauseWhenDeselected = false;
                }
            }

            if (isMultiObjectEditing && multiSelectedSystems != null)
            {
                foreach (var system in multiSelectedSystems)
                {
                    if (system != null)
                    {
                        system.enabled = true;
                        utils.ValidateAnimationCurves(system);
                        system.EmissionSettings.pauseWhenDeselected = false;

                        if (systemControls.ContainsKey(system))
                        {
                            systemControls[system].Initialize();
                        }
                    }
                }
            }
            SceneView.duringSceneGui += OnSceneGUI;
            Selection.selectionChanged += OnSelectionChanged;
            StartUpdateListener();
        }

        private void InitializeSystemControls()
        {
            if (systemControls == null)
                systemControls = new Dictionary<Core.UIParticleSystem, UIParticleSystemEditorControls>();
            else
                systemControls.Clear();

            if (isMultiObjectEditing && multiSelectedSystems != null)
            {
                foreach (var system in multiSelectedSystems)
                {
                    if (system != null && !systemControls.ContainsKey(system))
                    {
                        var systemControl = new UIParticleSystemEditorControls(system, utils);
                        systemControl.SetMultiObjectState(true, this);
                        systemControls[system] = systemControl;
                    }
                }
            }
            else if (particleSystem != null)
            {
                controls.SetMultiObjectState(false, null);
                systemControls[particleSystem] = controls;
            }
        }

        private void CheckMultiObjectSelection()
        {
            if (targets != null && targets.Length > 1)
            {
                multiSelectedSystems = targets
                    .OfType<Core.UIParticleSystem>()
                    .Where(system => system != null)
                    .ToArray();

                isMultiObjectEditing = multiSelectedSystems.Length > 1;
            }
            else
            {
                multiSelectedSystems = null;
                isMultiObjectEditing = false;
            }
        }

        private void OnSelectionChanged()
        {
            if (!isMultiObjectEditing)
            {
                CheckMultiObjectSelection();
                InitializeSystemControls();
            }
        }

        private void OnDisable()
        {
            StopUpdateListener();
            SceneView.duringSceneGui -= OnSceneGUI;
            Selection.selectionChanged -= OnSelectionChanged;

            if (isMultiObjectEditing && multiSelectedSystems != null)
            {
                foreach (var system in multiSelectedSystems)
                {
                    if (system != null)
                    {
                        system.EmissionSettings.pauseWhenDeselected = true;
                    }
                }
            }

            systemControls?.Clear();
        }

        private void StartUpdateListener()
        {
            if (!isUpdateListenerActive)
            {
                EditorApplication.update += UpdatePreviewPanel;
                isUpdateListenerActive = true;
            }
        }

        private void StopUpdateListener()
        {
            if (isUpdateListenerActive)
            {
                EditorApplication.update -= UpdatePreviewPanel;
                isUpdateListenerActive = false;
            }
        }

        protected override void OnHeaderGUI()
        {
            if (!isMultiObjectEditing)
            {
                guiRenderer.RenderHeader();
            }
        }

        public override void OnInspectorGUI()
        {
            if (guiRenderer != null)
            {
                guiRenderer.SetMultiObjectState(isMultiObjectEditing, multiSelectedSystems, this);
            }

            if (controls != null)
            {
                controls.SetMultiObjectState(isMultiObjectEditing, this);
            }

            serializedObject.Update();

            if (isMultiObjectEditing)
            {
                foreach (var system in multiSelectedSystems)
                {
                    if (system != null && !system.enabled)
                    {
                        system.enabled = true;
                    }
                }
            }
            else
            {
                MonoBehaviour mono = target as MonoBehaviour;
                if (mono != null && !mono.enabled)
                {
                    mono.enabled = true;
                }
            }

            guiRenderer.RenderInspector(serializedObject);

            if (GUI.changed)
            {
                if (isMultiObjectEditing)
                {
                    foreach (var system in multiSelectedSystems)
                    {
                        if (system != null)
                        {
                            EditorUtility.SetDirty(system);
                        }
                    }
                }
                else
                {
                    EditorUtility.SetDirty(target);
                }
            }

            serializedObject.ApplyModifiedProperties();

        }
        public bool IsAnySystemPlaying()
        {
            if (!isMultiObjectEditing || multiSelectedSystems == null)
                return false;

            return multiSelectedSystems.Any(system =>
                system != null && system.IsEmitting && !system.IsPaused);
        }

        public void PlayAllSelectedSystems()
        {
            int successCount = 0;

            foreach (var system in multiSelectedSystems)
            {
                if (system != null)
                {
                    system.EmissionSettings.pauseWhenDeselected = false;

                    if (system.IsPaused)
                    {
                        system.ResumeSystem();
                    }
                    else
                    {
                        system.StartEmitting();
                    }

                    successCount++;

                    EditorUtility.SetDirty(system);
                }
            }
            SceneView.RepaintAll();
        }

        public void PauseAllSelectedSystems()
        {
            int successCount = 0;

            foreach (var system in multiSelectedSystems)
            {
                if (system != null && system.IsEmitting)
                {
                    system.PauseSystem();
                    successCount++;
                    EditorUtility.SetDirty(system);
                }
            }
            SceneView.RepaintAll();
        }

        public void StopAllSelectedSystems()
        {
            int successCount = 0;

            foreach (var system in multiSelectedSystems)
            {
                if (system != null)
                {
                    Core.UIParticleSystem.ClearAllParticles(system);
                    system.StopEmitting();
                    successCount++;
                    EditorUtility.SetDirty(system);
                }
            }
            SceneView.RepaintAll();
        }

        public void RestartAllSelectedSystems()
        {
            int successCount = 0;

            foreach (var system in multiSelectedSystems)
            {
                if (system != null)
                {
                    system.RestartSystem();
                    successCount++;
                    EditorUtility.SetDirty(system);
                }
            }
            SceneView.RepaintAll();
        }

        public override bool RequiresConstantRepaint()
        {
            double now = EditorApplication.timeSinceStartup;
            if (now - lastRepaintTime < INSPECTOR_REPAINT_INTERVAL)
                return false;

            bool needsRepaint = false;

            if (isMultiObjectEditing)
            {
                if (multiSelectedSystems != null)
                {
                    for (int i = 0; i < multiSelectedSystems.Length; i++)
                    {
                        var system = multiSelectedSystems[i];
                        if (system != null &&
                            (system.IsEmitting || (system.ActiveParticles?.Count ?? 0) > 0))
                        {
                            needsRepaint = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                needsRepaint = controls?.ShouldRepaint() ?? false;
            }

            if (needsRepaint)
            {
                lastRepaintTime = now;
            }

            return needsRepaint;
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (!isMultiObjectEditing)
                guiRenderer.RenderSceneView(SceneView.currentDrawingSceneView);
        }

        private void UpdatePreviewPanel()
        {
            if (isMultiObjectEditing && systemControls != null)
            {
                bool shouldRepaint = false;

                foreach (var kvp in systemControls)
                {
                    if (kvp.Key != null && kvp.Value != null)
                    {
                        kvp.Value.UpdatePreview();

                        if (kvp.Value.ShouldRepaint())
                        {
                            shouldRepaint = true;
                        }
                    }
                }

                if (shouldRepaint)
                {
                    Repaint();
                }
            }
            else
            {
                if (particleSystem == null)
                {
                    return;
                }

                controls?.UpdatePreview();

                if (controls?.ShouldRepaint() ?? false)
                {
                    Repaint();
                }
            }
        }

        [MenuItem("GameObject/Effects/UI Particle System", false, 10)]
        static void CreateUIParticleSystem(MenuCommand menuCommand)
        {
            UIParticleSystemEditorUtils.CreateUIParticleSystemGameObject(menuCommand);
        }
    }
}
#endif