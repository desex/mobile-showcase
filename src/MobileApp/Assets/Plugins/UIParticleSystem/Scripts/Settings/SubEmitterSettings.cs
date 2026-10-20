using System.Collections.Generic;
using UnityEngine;
using TierStudios.UIParticleSystem.Enums;

namespace TierStudios.UIParticleSystem.Settings
{
    [System.Serializable]
    public class SubEmitterSettings
    {
        [Header("Sub-Emitter Settings")]
        [SerializeField] private bool enableSubEmitters = false;

        [SerializeField] private List<SubEmitterEntry> subEmitters = new List<SubEmitterEntry>();

        [Header("Mobile Optimization")]
        [SerializeField] private int maxConcurrentSubEmissions = 5;
        [SerializeField] private float subEmitterCooldown = 0.05f;

        public bool EnableSubEmitters
        {
            get => enableSubEmitters;
            set => enableSubEmitters = value;
        }

        public List<SubEmitterEntry> SubEmitters => subEmitters;

        public int MaxConcurrentSubEmissions
        {
            get => maxConcurrentSubEmissions;
            set => maxConcurrentSubEmissions = Mathf.Max(1, value);
        }

        public float SubEmitterCooldown
        {
            get => subEmitterCooldown;
            set => subEmitterCooldown = Mathf.Max(0f, value);
        }
    }

    [System.Serializable]
    public class SubEmitterEntry
    {
        [SerializeField] private SubEmitterTriggerType triggerType = SubEmitterTriggerType.Birth;
        [SerializeField] private SubEmitterActionType actionType = SubEmitterActionType.PlaySystem;

        [SerializeField] private TierStudios.UIParticleSystem.Core.UIParticleSystem targetSystem;

        [Header("Probability")]
        [Range(0f, 1f)]
        [SerializeField] private float probability = 1f;

        [Header("Lifetime Trigger")]
        [Range(0f, 1f)]
        [SerializeField] private float lifetimeThreshold = 0.5f;
        [System.NonSerialized] public bool lifetimeTriggered = false;

        [Header("Inherit Properties")]
        [SerializeField] private bool inheritPosition = true;
        [SerializeField] private bool inheritColor = false;
        [SerializeField] private bool inheritSize = false;

        [Header("Action Parameters")]
        [SerializeField] private Color targetColor = Color.red;
        [SerializeField] private float targetSize = 50f;
        [SerializeField] private int burstCount = 5;
        [SerializeField] private int instancePoolSize = 8;
        [SerializeField] private Gradient colorGradient;

        public SubEmitterTriggerType TriggerType
        {
            get => triggerType;
            set => triggerType = value;
        }

        public SubEmitterActionType ActionType
        {
            get => actionType;
            set => actionType = value;
        }

        public TierStudios.UIParticleSystem.Core.UIParticleSystem TargetSystem
        {
            get => targetSystem;
            set => targetSystem = value;
        }

        public float Probability
        {
            get => probability;
            set => probability = Mathf.Clamp01(value);
        }

        public float LifetimeThreshold
        {
            get => lifetimeThreshold;
            set => lifetimeThreshold = Mathf.Clamp01(value);
        }

        public bool InheritPosition
        {
            get => inheritPosition;
            set => inheritPosition = value;
        }

        public bool InheritColor
        {
            get => inheritColor;
            set => inheritColor = value;
        }

        public bool InheritSize
        {
            get => inheritSize;
            set => inheritSize = value;
        }

        public Color TargetColor
        {
            get => targetColor;
            set => targetColor = value;
        }

        public float TargetSize
        {
            get => targetSize;
            set => targetSize = Mathf.Max(0.1f, value);
        }

        public int BurstCount
        {
            get => burstCount;
            set => burstCount = Mathf.Max(1, value);
        }

        public int InstancePoolSize
        {
            get => instancePoolSize < 1 ? 8 : Mathf.Clamp(instancePoolSize, 1, 64);
            set => instancePoolSize = Mathf.Clamp(value, 1, 64);
        }

        public Gradient ColorGradient
        {
            get => colorGradient;
            set => colorGradient = value;
        }
    }
}