using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TierStudios.UIParticleSystem.Enums;

namespace TierStudios.UIParticleSystem.Enums
{
    public enum StartDelayMode
    {
        Constant = 0,
        RandomBetweenConstants = 1
    }
}

namespace TierStudios.UIParticleSystem.Settings
{
    [System.Serializable]
    public class Burst
    {
        public float time = 0f;
        public int count = 10;
        public float probability = 1f;
        [HideInInspector] public bool triggered = false;
    }

    [System.Serializable]
    public class EmissionSettings
    {
        public float emissionRate = 10f;
        public int maxParticles = 1000;
        public bool emitOnStart = true;

        public float emissionRateOverDistance = 0f;
        public List<Burst> bursts = new List<Burst>();

        public EmissionShapeType emissionShape = EmissionShapeType.Cone;
        public Vector2 emissionAreaSize = new Vector2(100, 100);
        public float circleRadius = 50f;
        public float lineLength = 100f;
        public float coneAngle = 90f;
        public float coneRange = 100f;
        public bool emitFromEdge = false;

        public Sprite shapeSprite;
        public Vector2 shapeSpriteSize = new Vector2(100f, 100f);
        [Range(0f, 1f)]
        public float shapeSpriteThreshold = 0.1f;
        public Image shapeImage;
        public Graphic shapeText;

        public Vector2 emissionDirection = Vector2.up;
        public bool randomizeDirection = true;
        public float directionVariance = 30f;

        public StartDelayMode startDelayMode = StartDelayMode.Constant;
        public float startDelay = 0f;
        public float startDelayMin = 0f;
        public float startDelayMax = 0f;

        public float duration = 5f;
        public bool looping = true;
        public bool prewarm = false;
        public SimulationSpace simulationSpace = SimulationSpace.Local;

        public bool onlyInEditor = false;
        public bool previewInEditor = true;
        public bool pauseWhenDeselected = true;
    }
}