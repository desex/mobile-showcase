using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

namespace Sotbify.MobileApp.NewsWindow.Core
{
    public class LoadingOverlay : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _fadeDuration = 0.33f;
        [SerializeField] private Ease _fadeEase = Ease.OutQuad;

        private MotionHandle _fadeMotion;
        private bool _isVisible;

        public void Show()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public async UniTask HideAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}
