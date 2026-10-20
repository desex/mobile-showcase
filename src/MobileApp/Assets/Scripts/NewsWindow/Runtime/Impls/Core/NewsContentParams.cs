using System;
using Com.ForbiddenByte.OSA.Core;
using UnityEngine;

namespace Sotbify.MobileApp.NewsWindow.Core
{
    [Serializable]
    public class NewsContentParams : BaseParams
    {
        public RectTransform TextPrefab;
        public RectTransform ImagePrefab;

        public override void InitIfNeeded(IOSA adapter)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}