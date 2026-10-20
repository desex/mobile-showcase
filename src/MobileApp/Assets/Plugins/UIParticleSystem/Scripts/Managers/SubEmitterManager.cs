using System.Collections.Generic;
using TierStudios.UIParticleSystem.Core;
using TierStudios.UIParticleSystem.Enums;
using TierStudios.UIParticleSystem.Settings;
using UnityEngine;

namespace TierStudios.UIParticleSystem.Managers
{
    public class SubEmitterManager
    {
        private class PooledInstance
        {
            public Core.UIParticleSystem system;
            public float lastUsedTime;
            public int templateId;
            public int templateRevision;
        }

        private readonly Core.UIParticleSystem particleSystem;
        private readonly Dictionary<SubEmitterEntry, List<PooledInstance>> instancePools =
            new Dictionary<SubEmitterEntry, List<PooledInstance>>();
        private Transform poolRoot;

        private float lastSubEmitTime = 0f;
        private int activeSubEmissionsThisFrame = 0;
        private int currentFrameCount = -1;

        public SubEmitterManager(Core.UIParticleSystem system)
        {
            particleSystem = system;
        }

        public void Cleanup()
        {
            foreach (var pool in instancePools.Values)
            {
                if (pool == null) continue;
                for (int i = 0; i < pool.Count; i++)
                    DestroyPooledSystem(pool[i]?.system);
                pool.Clear();
            }

            instancePools.Clear();

            if (poolRoot != null)
            {
                DestroyPooledObject(poolRoot.gameObject);
                poolRoot = null;
            }
        }

        public void OnParticleBirth(Particle particle)
        {
            if (!CanProcess(SubEmitterTriggerType.Birth)) return;

            var entries = particleSystem.SubEmitterSettings.SubEmitters;
            int count = entries.Count;

            for (int i = 0; i < count; i++)
            {
                var entry = entries[i];
                if (entry.TriggerType != SubEmitterTriggerType.Birth) continue;
                if (!PassesProbabilityCheck(entry)) continue;

                entry.lifetimeTriggered = false;
                ExecuteAction(entry, particle);
            }
        }

        public void OnParticleDeath(Particle particle)
        {
            if (!particleSystem.SubEmitterSettings.EnableSubEmitters) return;
            if (!particleSystem.CanModifyChildHierarchy) return;

            var entries = particleSystem.SubEmitterSettings.SubEmitters;
            int count = entries.Count;

            for (int i = 0; i < count; i++)
            {
                var entry = entries[i];
                if (entry.TriggerType != SubEmitterTriggerType.Death) continue;
                if (!PassesProbabilityCheck(entry)) continue;

                ExecuteAction(entry, particle);
            }
        }

        public void OnParticleCollision(Particle particle)
        {
            if (!CanProcess(SubEmitterTriggerType.Collision)) return;

            var entries = particleSystem.SubEmitterSettings.SubEmitters;
            int count = entries.Count;

            for (int i = 0; i < count; i++)
            {
                var entry = entries[i];
                if (entry.TriggerType != SubEmitterTriggerType.Collision) continue;
                if (!PassesProbabilityCheck(entry)) continue;

                ExecuteAction(entry, particle);
            }
        }

        public void OnParticleUpdate(Particle particle)
        {
            if (!CanProcess(SubEmitterTriggerType.Lifetime)) return;

            float normalizedAge = particle.GetNormalizedAge();

            var entries = particleSystem.SubEmitterSettings.SubEmitters;
            int count = entries.Count;

            for (int i = 0; i < count; i++)
            {
                var entry = entries[i];
                if (entry.TriggerType != SubEmitterTriggerType.Lifetime) continue;
                if (entry.lifetimeTriggered) continue;
                if (normalizedAge < entry.LifetimeThreshold) continue;
                if (!PassesProbabilityCheck(entry)) continue;

                entry.lifetimeTriggered = true;
                ExecuteAction(entry, particle);
            }
        }

        public void ResetLifetimeTriggers()
        {
            var entries = particleSystem.SubEmitterSettings.SubEmitters;
            int count = entries.Count;

            for (int i = 0; i < count; i++)
            {
                entries[i].lifetimeTriggered = false;
            }
        }

        private bool CanProcess(SubEmitterTriggerType triggerType)
        {
            if (!particleSystem.SubEmitterSettings.EnableSubEmitters) return false;
            if (!particleSystem.CanModifyChildHierarchy) return false;

            if (triggerType == SubEmitterTriggerType.Death) return true;

            int frame = Application.isPlaying ? Time.frameCount : (int)(Time.realtimeSinceStartup * 60f);
            if (frame != currentFrameCount)
            {
                currentFrameCount = frame;
                activeSubEmissionsThisFrame = 0;
            }

            if (activeSubEmissionsThisFrame >= particleSystem.SubEmitterSettings.MaxConcurrentSubEmissions)
                return false;

            float currentTime = Application.isPlaying ? Time.time : Time.realtimeSinceStartup;
            float cooldown = particleSystem.SubEmitterSettings.SubEmitterCooldown;

            if (Application.isPlaying && cooldown < 0.01f)
                cooldown = 0f;

            if (currentTime - lastSubEmitTime < cooldown)
                return false;

            return true;
        }

        private bool PassesProbabilityCheck(SubEmitterEntry entry)
        {
            return entry.Probability >= 1f || Random.value <= entry.Probability;
        }

        private void ExecuteAction(SubEmitterEntry entry, Particle sourceParticle)
        {
            lastSubEmitTime = Application.isPlaying ? Time.time : Time.realtimeSinceStartup;
            activeSubEmissionsThisFrame++;

            switch (entry.ActionType)
            {
                case SubEmitterActionType.PlaySystem:
                    ExecutePlaySystem(entry, sourceParticle);
                    break;

                case SubEmitterActionType.StopSystem:
                    ExecuteStopSystem(entry);
                    break;

                case SubEmitterActionType.EmitBurst:
                    ExecuteEmitBurst(entry, sourceParticle);
                    break;

                case SubEmitterActionType.ChangeColor:
                    ExecuteChangeColor(entry, sourceParticle);
                    break;

                case SubEmitterActionType.ChangeSize:
                    ExecuteChangeSize(entry, sourceParticle);
                    break;
            }
        }

        private void ExecutePlaySystem(SubEmitterEntry entry, Particle sourceParticle)
        {
            var template = entry.TargetSystem;
            if (template == null || template == particleSystem) return;

            var instance = GetPooledInstance(entry, template);
            if (instance == null) return;

            instance.EnsureInitializedPublic();

            if (entry.InheritPosition && sourceParticle != null)
                instance.Transform.position = GetParticleWorldPosition(sourceParticle);

            if (entry.InheritColor && sourceParticle != null)
                ApplyColorToSystem(instance, sourceParticle.color);

            if (entry.InheritSize && sourceParticle != null)
                instance.SizeSettings.size = sourceParticle.size;

            instance.RestartSystem();

#if UNITY_EDITOR
            if (!Application.isPlaying)
                UnityEditor.EditorUtility.SetDirty(instance);
#endif
        }

        private void ExecuteStopSystem(SubEmitterEntry entry)
        {
            var target = entry.TargetSystem;
            if (target == null) return;

            target.StopEmitting();

            if (!instancePools.TryGetValue(entry, out var pool) || pool == null)
                return;

            for (int i = 0; i < pool.Count; i++)
            {
                var system = pool[i]?.system;
                if (system != null)
                    system.StopEmitting();
            }
        }

        private void ExecuteEmitBurst(SubEmitterEntry entry, Particle sourceParticle)
        {
            var target = entry.TargetSystem;
            if (target == null || target == particleSystem) return;

            target.EnsureInitializedPublic();

            Vector3? worldPosition = null;
            if (entry.InheritPosition && sourceParticle != null)
                worldPosition = GetParticleWorldPosition(sourceParticle);

            if (entry.InheritColor && sourceParticle != null)
                ApplyColorToSystem(target, sourceParticle.color);

            if (entry.InheritSize && sourceParticle != null)
                target.SizeSettings.size = sourceParticle.size;

            int burstCount = Mathf.Min(entry.BurstCount, target.EffectiveMaxParticles - target.GetActiveParticleCount());
            for (int i = 0; i < burstCount; i++)
                Utilities.ParticleUtilities.EmitParticle(target, worldPosition);
        }

        private Core.UIParticleSystem GetPooledInstance(SubEmitterEntry entry, Core.UIParticleSystem template)
        {
            if (!instancePools.TryGetValue(entry, out var pool) || pool == null)
            {
                pool = new List<PooledInstance>(entry.InstancePoolSize);
                instancePools[entry] = pool;
            }

            int templateId = template.GetInstanceID();
            int templateRevision = template.SettingsRevision;
            float now = Application.isPlaying ? Time.time : Time.realtimeSinceStartup;
            PooledInstance idle = null;
            PooledInstance oldest = null;

            for (int i = pool.Count - 1; i >= 0; i--)
            {
                var pooled = pool[i];
                if (pooled == null || pooled.system == null)
                {
                    pool.RemoveAt(i);
                    continue;
                }

                if (pooled.templateId != templateId || pooled.templateRevision != templateRevision)
                {
                    DestroyPooledSystem(pooled.system);
                    pool.RemoveAt(i);
                    continue;
                }

                if (oldest == null || pooled.lastUsedTime < oldest.lastUsedTime)
                    oldest = pooled;

                if (idle == null && IsInstanceIdle(pooled.system))
                    idle = pooled;
            }

            if (idle != null)
            {
                idle.lastUsedTime = now;
                return idle.system;
            }

            if (pool.Count < entry.InstancePoolSize)
            {
                var created = CreatePooledClone(template);
                if (created == null)
                    return oldest != null ? oldest.system : null;

                pool.Add(new PooledInstance
                {
                    system = created,
                    lastUsedTime = now,
                    templateId = templateId,
                    templateRevision = templateRevision
                });
                return created;
            }

            if (oldest == null)
                return null;

            oldest.lastUsedTime = now;
            return oldest.system;
        }

        private static bool IsInstanceIdle(Core.UIParticleSystem system)
        {
            if (system.GetActiveParticleCount() > 0)
                return false;

            if (!system.IsEmitting)
                return true;

            return !system.EmissionSettings.looping;
        }

        private Core.UIParticleSystem CreatePooledClone(Core.UIParticleSystem template)
        {
            if (template == null)
                return null;

            Transform root = GetPoolRoot(template);
            var runtimeChildren = DetachRuntimeGeneratedChildren(template.transform);
            GameObject cloneObject = null;

            try
            {
                cloneObject = Object.Instantiate(template.gameObject, root, false);
            }
            finally
            {
                ReattachRuntimeGeneratedChildren(template.transform, runtimeChildren);
            }

            if (cloneObject == null)
                return null;

            cloneObject.name = template.gameObject.name + " (SubEmitter)";
            cloneObject.hideFlags = HideFlags.HideAndDontSave;
            StripRuntimeGeneratedChildren(cloneObject.transform);

            var clone = cloneObject.GetComponent<Core.UIParticleSystem>();
            if (clone == null)
            {
                DestroyPooledObject(cloneObject);
                return null;
            }

            clone.EmissionSettings.emitOnStart = false;
            clone.EmissionSettings.pauseWhenDeselected = false;
            if (clone.SubEmitterSettings != null)
                clone.SubEmitterSettings.EnableSubEmitters = false;

            clone.StopEmitting();
            Core.UIParticleSystem.ClearAllParticles(clone, true);
            return clone;
        }

        private Transform GetPoolRoot(Core.UIParticleSystem template)
        {
            if (poolRoot != null)
                return poolRoot;

            Transform parent = template.transform.parent != null
                ? template.transform.parent
                : particleSystem.transform.parent != null
                    ? particleSystem.transform.parent
                    : particleSystem.transform;

            var go = new GameObject("UIParticleSubEmitterPool");
            go.hideFlags = HideFlags.HideAndDontSave;
            go.transform.SetParent(parent, false);

            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.localScale = Vector3.one;

            poolRoot = go.transform;
            return poolRoot;
        }

        private static List<GameObject> DetachRuntimeGeneratedChildren(Transform root)
        {
            var held = new List<GameObject>(4);
            if (root == null)
                return held;

            for (int i = root.childCount - 1; i >= 0; i--)
            {
                Transform child = root.GetChild(i);
                if (child == null || !IsRuntimeGeneratedChild(child.name))
                    continue;

                child.SetParent(null, false);
                held.Add(child.gameObject);
            }

            return held;
        }

        private static void ReattachRuntimeGeneratedChildren(Transform root, List<GameObject> held)
        {
            if (root == null || held == null)
                return;

            for (int i = 0; i < held.Count; i++)
            {
                GameObject go = held[i];
                if (go == null)
                    continue;
                go.transform.SetParent(root, false);
            }
        }

        private static void StripRuntimeGeneratedChildren(Transform root)
        {
            if (root == null)
                return;

            var children = root.GetComponentsInChildren<Transform>(true);
            for (int i = children.Length - 1; i >= 0; i--)
            {
                Transform child = children[i];
                if (child == null || child == root || !IsRuntimeGeneratedChild(child.name))
                    continue;
                DestroyPooledObject(child.gameObject);
            }
        }

        private static bool IsRuntimeGeneratedChild(string name)
        {
            return name == "UIParticleTrail" || name == "UIParticleQuads";
        }

        private static void DestroyPooledSystem(Core.UIParticleSystem system)
        {
            if (system == null)
                return;
            system.UnregisterEditorPreview();
            DestroyPooledObject(system.gameObject);
        }

        private static void DestroyPooledObject(GameObject go)
        {
            if (go == null)
                return;

            if (Application.isPlaying)
                Object.Destroy(go);
            else
                Object.DestroyImmediate(go);
        }

        private void ExecuteChangeColor(SubEmitterEntry entry, Particle sourceParticle)
        {
            if (sourceParticle == null) return;

            if (entry.ColorGradient != null && entry.ColorGradient.colorKeys.Length > 0)
            {
                sourceParticle.color = entry.ColorGradient.Evaluate(sourceParticle.GetNormalizedAge());
            }
            else
            {
                sourceParticle.color = entry.TargetColor;
            }
        }

        private void ExecuteChangeSize(SubEmitterEntry entry, Particle sourceParticle)
        {
            if (sourceParticle == null) return;

            sourceParticle.size = entry.TargetSize;
            sourceParticle.size2D = new Vector2(entry.TargetSize, entry.TargetSize);
        }

        private Vector3 GetParticleWorldPosition(Particle particle)
        {
            if (particle == null)
                return particleSystem.Transform.position;

            if (particleSystem.EmissionSettings.simulationSpace == SimulationSpace.Local)
                return particleSystem.Transform.TransformPoint(particle.position);

            return particle.position;
        }

        private void ApplyColorToSystem(Core.UIParticleSystem target, Color color)
        {
            if (target.ColorSettings.useGradientColor && target.ColorSettings.colorGradient != null)
            {
                var keys = target.ColorSettings.colorGradient.colorKeys;
                if (keys.Length >= 2)
                {
                    keys[0].color = color;
                    var alphaKeys = target.ColorSettings.colorGradient.alphaKeys;
                    target.ColorSettings.colorGradient.SetKeys(keys, alphaKeys);
                }
            }
            else
            {
                target.ColorSettings.particleColor = color;
            }
        }
    }
}
