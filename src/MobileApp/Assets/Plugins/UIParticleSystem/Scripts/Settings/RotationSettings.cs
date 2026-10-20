namespace TierStudios.UIParticleSystem.Settings
{
    [System.Serializable]
    public class RotationSettings
    {
        public bool useRandomRotation = false;
        public float initialRotation = 0f;
        public float minRotation = 0f;
        public float maxRotation = 360f;

        public bool useRotationOverLifetime = false;
        public float rotationSpeed = 90f;
        public bool useRandomRotationSpeed = false;
        public float minRotationSpeed = -90f;
        public float maxRotationSpeed = 90f;

        public bool alignToDirection = false;

        public bool useRotationBySpeed = false;
        public float rotationBySpeed = 90f;
        public float rotationBySpeedMin = 0f;
        public float rotationBySpeedMax = 200f;
    }
}