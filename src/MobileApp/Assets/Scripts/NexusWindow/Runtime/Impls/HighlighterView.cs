using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using Sotbify.MobileApp.Utils;
using Sotbify.MobileApp.Utils.Extensions;
using UnityEngine;

namespace Sotbify.MobileApp.NexusWindow
{
    [RequireComponent(typeof(RectTransform))]
    public class HighlighterView : MonoBehaviour
    {
        [SerializeField] private MotionParameters _motionParameters;
        
        private RectTransform RectTransform => transform.AsRectTransform();

        public async UniTask MoveTo(RectTransform targetRectTransform, CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}