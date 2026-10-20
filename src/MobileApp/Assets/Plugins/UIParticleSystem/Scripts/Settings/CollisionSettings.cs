using System.Collections.Generic;
using UnityEngine;

namespace TierStudios.UIParticleSystem.Settings
{
    [System.Serializable]
    public class CollisionSettings
    {
        public bool useCollision = false;

        public bool useBounds = true;
        public RectTransform boundsTarget;

        public List<RectTransform> obstacles = new List<RectTransform>();

        [Range(0f, 1f)]
        public float bounce = 0.5f;
        [Range(0f, 1f)]
        public float dampen = 0.1f;
        [Range(0f, 1f)]
        public float friction = 0f;
        [Range(0f, 1f)]
        public float lifetimeLoss = 0f;
        [Range(0f, 2f)]
        public float radiusScale = 0.5f;
        [Range(0f, 45f)]
        public float bounceSpread = 0f;

        public bool killOnCollision = false;
        public int maxCollisionCount = 0;
        public float minKillSpeed = 0f;
    }
}
