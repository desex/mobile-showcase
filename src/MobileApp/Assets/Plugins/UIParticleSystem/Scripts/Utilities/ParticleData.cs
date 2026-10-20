namespace TierStudios.UIParticleSystem.Core
{
    public struct ParticleData
    {
        public UnityEngine.Vector3 position;
        public UnityEngine.Vector3 velocity;
        public UnityEngine.Vector3 originalVelocity;
        public float lifetime;
        public float age;
        public float maxLifetime;
        public byte isAlive;          

        public float size;
        public float initialSize;
        public UnityEngine.Vector2 size2D;
        public UnityEngine.Vector2 initialSize2D;

        public UnityEngine.Color color;
        public UnityEngine.Color initialColor;

        public float rotation;
        public float angularVelocity;

        public float attractorFadeAlpha;

        public UnityEngine.Vector2 noiseOffset;
        public float noiseTime;
        public float noiseSeed;

        public float stretchedRotation;
        public float stretchedWidth;
        public float stretchedHeight;

        public UnityEngine.Vector2 velocityRandomLinearOffset;
        public float velocityRandomOrbitalOffset;

        public float limitVelocityRandomLerp;
        public UnityEngine.Vector3 limitVelocityRandomAxesLerp;
        public float dragRandomLerp;

        public float forceRandomLerpX;
        public float forceRandomLerpY;

        public int currentFrame;
        public float animationTime;
        public int startFrame;
        public int endFrame;
        public byte hasAnimation;

        public byte useGradient;      
        public byte useSecondGradient;

        public UnityEngine.Vector3 point0; 
        public UnityEngine.Vector3 point1;
        public UnityEngine.Vector3 point2;
        public UnityEngine.Vector3 point3;
    }
}