using System;
using Com.ForbiddenByte.OSA.Core;
using UnityEngine;

namespace Sotbify.MobileApp.FeedWindow.FeedList.Core
{
    [Serializable]
    public class FeedParams : BaseParams
    {
        [SerializeField, Min(1f)] private float _defaultPostSize = 100f;
        [SerializeField, Min(1f)] private float _defaultBonusSize = 100f;

        public RectTransform PostPrefab;
        public RectTransform BonusPrefab;

        public override void InitIfNeeded(IOSA iAdapter)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}