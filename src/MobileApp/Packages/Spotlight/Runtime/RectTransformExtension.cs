using System;
using UnityEngine;

namespace Sotbifay.Spotlight
{
    public static class RectTransformExtension
    {
        /// <summary>
        ///   Transforms Rect from local space to world space.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="rect">Rect in local coordiantes.</param>>
        /// <returns>Rect in world coordinates</returns>
        public static Rect GetWorldSpaceRect(this Transform transform, Rect rect)
        {
            if(transform == null)
                throw new ArgumentNullException(nameof(transform));
            
            var lossyScale = transform.lossyScale;
            var position = transform.position;

            return new Rect
            (
                rect.x * lossyScale.x + position.x,
                rect.y * lossyScale.y + position.y,
                rect.width * lossyScale.x,
                rect.height * lossyScale.y
            );
        }
    }
}