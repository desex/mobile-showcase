using UnityEngine;

namespace TierStudios.UIParticleSystem.Settings
{
    [System.Serializable]
    public class LimitVelocityOverLifetimeSettings
    {
        public bool useLimitVelocityOverLifetime = false;

        public bool separateAxes = false;

        public int limitMode = 0;
        public float limit = 100f;
        public AnimationCurve limitCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);
        public float limitMin = 0f;
        public float limitMax = 100f;
        public AnimationCurve limitMinCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        public int limitXMode = 0;
        public float limitX = 100f;
        public AnimationCurve limitXCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);
        public float limitXMin = 0f;
        public float limitXMax = 100f;
        public AnimationCurve limitXMinCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        public int limitYMode = 0;
        public float limitY = 100f;
        public AnimationCurve limitYCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);
        public float limitYMin = 0f;
        public float limitYMax = 100f;
        public AnimationCurve limitYMinCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        public int limitZMode = 0;
        public float limitZ = 100f;
        public AnimationCurve limitZCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);
        public float limitZMin = 0f;
        public float limitZMax = 100f;
        public AnimationCurve limitZMinCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        public float dampen = 1f;

        public int dragMode = 0;
        public float drag = 0f;
        public AnimationCurve dragCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);
        public float dragMin = 0f;
        public float dragMax = 0f;
        public AnimationCurve dragMinCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);

        public bool multiplyDragBySize = false;
        public bool multiplyDragByVelocity = false;
    }
}
