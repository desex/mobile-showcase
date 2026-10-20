using UnityEngine;

namespace TierStudios.UIParticleSystem.Core
{
    [System.Serializable]
    public class Particle
    {
        public Vector3 position;
        public Vector3 velocity;
        public Vector3 originalVelocity;
        public float lifetime;
        public float age;
        public bool isAlive;
        public float rotation;
        public float angularVelocity;
        public float rotationSpeed;

        public float size;
        public float initialSize;
        public Vector2 size2D;
        public Vector2 initialSize2D;
        public Vector3 initialScale;

        public Color color;
        public Color initialColor;
        public Color endColor;
        public bool useGradient;
        public bool useSecondGradient;
        public Gradient assignedGradient;

        public float maxLifetime;

        public int currentFrame;
        public float frameTime;
        public float animationTime;
        public int startFrame;
        public int endFrame;
        public bool hasAnimation;

        public ParticleTrail trail;
        public float trailUpdateTimer;
        public Vector2 lastTrailPosition;

        public float attractorStrength;
        public float attractorFadeAlpha;

        public Vector2 noiseOffset;
        public float noiseTime;
        public float noiseSeed;

        public float stretchedWidth;
        public float stretchedHeight;
        public float stretchedRotation;

        public Vector2 velocityRandomLinearOffset;
        public float velocityRandomOrbitalOffset;

        public float limitVelocityRandomLerp;
        public Vector3 limitVelocityRandomAxesLerp;
        public float dragRandomLerp;

        public float forceRandomLerpX;
        public float forceRandomLerpY;

        public int collisionCount;

        public readonly Vector3[] points = new Vector3[4];

        public Particle()
        {
            isAlive = false;
            age = 0f;
            currentFrame = 0;
            frameTime = 0f;
            trailUpdateTimer = 0f;
            lastTrailPosition = Vector2.zero;
            attractorStrength = 1f;
            attractorFadeAlpha = 1f;
            noiseOffset = Vector2.zero;
            noiseTime = 0f;
            size2D = Vector2.one * 50f;
            initialSize2D = Vector2.one * 50f;
            velocityRandomLinearOffset = Vector2.zero;
            velocityRandomOrbitalOffset = 0f;
            limitVelocityRandomLerp = 0f;
            limitVelocityRandomAxesLerp = Vector3.zero;
            dragRandomLerp = 0f;
            forceRandomLerpX = 0f;
            forceRandomLerpY = 0f;
            collisionCount = 0;
            stretchedWidth = 0f;
            stretchedHeight = 0f;
            stretchedRotation = 0f;
        }

        public void Initialize(Vector3 startPosition, Vector3 startVelocity, float particleLifetime, float particleSize, Color particleColor)
        {
            position = startPosition;
            velocity = startVelocity;
            originalVelocity = startVelocity;
            lifetime = particleLifetime;
            maxLifetime = particleLifetime;
            age = 0f;
            size = particleSize;
            initialSize = particleSize;
            color = particleColor;
            initialColor = particleColor;
            rotation = 0f;
            angularVelocity = 0f;
            rotationSpeed = 0f;
            isAlive = true;
            currentFrame = 0;
            frameTime = 0f;
            animationTime = 0f;
            trailUpdateTimer = 0f;
            lastTrailPosition = startPosition;
            attractorStrength = 1f;
            attractorFadeAlpha = 1f;
            noiseOffset = new Vector2(Random.Range(-1000f, 1000f), Random.Range(-1000f, 1000f));
            noiseTime = 0f;
            noiseSeed = Random.Range(-1000f, 1000f);
            velocityRandomLinearOffset = Vector2.zero;
            velocityRandomOrbitalOffset = 0f;
            limitVelocityRandomLerp = Random.value;
            limitVelocityRandomAxesLerp = new Vector3(Random.value, Random.value, Random.value);
            dragRandomLerp = Random.value;
            forceRandomLerpX = Random.value;
            forceRandomLerpY = Random.value;
            collisionCount = 0;
            stretchedWidth = 0f;
            stretchedHeight = 0f;
            stretchedRotation = 0f;
            size2D = Vector2.one * particleSize;
            initialSize2D = size2D;

            trail?.Clear();
        }

        public void Initialize(Vector3 startPosition, Vector3 startVelocity, float particleLifetime, Vector2 particle2DSize, Color particleColor)
        {
            Initialize(startPosition, startVelocity, particleLifetime, particle2DSize.x, particleColor);
            size2D = particle2DSize;
            initialSize2D = particle2DSize;
        }

        public void UpdateAge(float deltaTime)
        {
            age += deltaTime;
            lifetime -= deltaTime;

            if (age >= maxLifetime || lifetime <= 0f)
                isAlive = false;
        }

        public float GetNormalizedAge()
        {
            return maxLifetime > 0f ? Mathf.Clamp01(age / maxLifetime) : 0f;
        }
    }
}