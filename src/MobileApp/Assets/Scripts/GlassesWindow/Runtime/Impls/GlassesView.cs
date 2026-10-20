using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.Db.GlassesParameters;
using Sotbify.MobileApp.UI.Window.View;
using Sotbify.MobileApp.Utils;
using Sotbify.MobileApp.Utils.Enums;
using Sotbify.MobileApp.Utils.Extensions;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using VContainer;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.GlassesWindow
{
    public class GlassesView : FadeWindowView
    {
        [SerializeField] private RectTransform _glassesRectTransform;
        [SerializeField] private GameObject _scanFrameParent;
        [SerializeField] private CanvasGroup _glassesParent;
        [SerializeField] private AspectRatioFitter _imageRatioFitter;
        [SerializeField] private RawImage _maskedImage;
        
        [Space(10)] 
        [Header("States")] 
        [SerializeField] private Image _maskColoredImage;
        [SerializeField] private Image _frameImage;

        [SerializeField] private Image _pendingIcon;
        [SerializeField] private Image _successIcon;
        [SerializeField] private Image _failedIcon;

        private IGlassesParameters _glassesParameters;

        private Vector2 _baseFrameSize;
        private ILogger _logger;
        private CancellationTokenSource _cancellationTokenSource;

        [Inject]
        public void Construct(
            IGlassesParameters glassesParameters,
            ILogger logger
        )
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        public async UniTask AssignCameraTexture(WebCamTexture texture, CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        public async UniTask ChangeGlassesState(EScanWindowState windowState, Action callback,
            CancellationToken token = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        private async UniTask CorrectTextureOrientation(WebCamTexture texture, CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        private async UniTask PlayPendingDisableAnimation(CancellationToken token)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        private async UniTask SuccessAnimation(CancellationToken token = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        private async UniTask FailAnimation(CancellationToken token = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override UniTask ShowInternalAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void ResetAnimationsValues()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}