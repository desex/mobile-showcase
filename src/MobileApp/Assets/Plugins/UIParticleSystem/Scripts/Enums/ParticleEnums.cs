using UnityEngine;

namespace TierStudios.UIParticleSystem.Enums
{
    public enum EmissionShapeType
    {
        Rectangle,
        Circle,
        Line,
        Cone,
        Point,
        Edge,
        Sprite,
        Image,
        Text
    }

    public enum SimulationSpace
    {
        Local,
        World
    }

    public enum AttractorType
    {
        Attract,
        Repel,
        Orbital,
        Vortex,
        Direct
    }

    public enum NoiseType
    {
        Perlin,
        Turbulence,
        Ridge,
        Billow
    }

    public enum AnimationCycle
    {
        WholeSheet,
        SingleRow,
        SingleColumn,
        Custom
    }

    public enum LifetimeMode
    {
        Constant,
        RandomBetweenTwoConstants
    }

    public enum BlendMode
    {
        AlphaBlend,
        Additive
    }

    public enum AnimationTimeMode
    {
        FramesPerSecond,
        LifetimeNormalized
    }

    public enum TrailColorMode
    {
        Color,
        RandomColor,
        Gradient,
        RandomGradient,
        InheritParticleColor,
        InheritTextureColor
    }

    public enum UIRenderMode
    {
        Billboard,
        StretchedBillboard
    }

    public enum InheritVelocityMode
    {
        Initial = 0,
        Current = 1
    }
}