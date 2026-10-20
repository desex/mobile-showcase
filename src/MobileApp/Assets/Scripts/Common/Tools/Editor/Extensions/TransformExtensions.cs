using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sotbifay.Launcher.Common.Tools.Editor.Extensions
{
    public static class TransformExtensions
    {
        public static IEnumerable<Transform> GetAllChildren(this IEnumerable<Transform> transforms, bool includeParent = false)
        {
            if (transforms is null)
                throw new ArgumentNullException(nameof(transforms));

            return transforms.SelectMany(parent => parent.GetAllChildren(includeParent));
        }
        
        public static IEnumerable<Transform> GetAllChildren(this Transform transform, bool includeParent = false)
        {
            if (transform is null)
                throw new ArgumentNullException(nameof(transform));
            
            var transformsQueue = new Queue<Transform>();
        
            if(includeParent == false)
            {
                foreach (Transform childTransform in transform)
                    transformsQueue.Enqueue(childTransform);
            }
            else
            {
                transformsQueue.Enqueue(transform);
            }

            while (transformsQueue.Count > 0)
            {
                var current = transformsQueue.Dequeue();
                yield return current;

                foreach (Transform childTransform in current)
                    transformsQueue.Enqueue(childTransform);
            }
        }
    }
}