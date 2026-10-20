using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.Services.QRScanner;
using Sotbify.MobileApp.Utils.Enums;
using UnityEngine;

namespace Sotbify.MobileApp.GlassesWindow
{
    public class GlassesWindow : IGlassesWindow, IDisposable
    {
        private readonly ReactiveProperty<ScanState> _scanState = new();
        private readonly ReactiveProperty<WebCamTexture> _webTexture = new();
        
        private readonly IQRScannerService _qrScannerService;
        private bool _isAnimating = false;

        public GlassesWindow(IQRScannerService qrScannerService)
        {
            _qrScannerService = qrScannerService;
        }

        public ReadOnlyReactiveProperty<ScanState> ScanState => _scanState;
        public ReadOnlyReactiveProperty<WebCamTexture> WebTexture => _webTexture;
        
        public void Start()
        {
#if UNITY_EDITOR_OSX
            return;
#endif
            
            var webCamTexture = _qrScannerService.Start();
            if (webCamTexture == null)
                throw new NullReferenceException("WebCamTexture is null");
            
            _webTexture.Value = webCamTexture;
        }
        public UniTask<string> ScanQRCode(CancellationToken token = default)
        {
            return _qrScannerService.ScanAsync(token);
        }
        
        public void Stop()
        {
            _qrScannerService.Stop();
            _webTexture.Value = null;
        }

        public async UniTask ChangeState(EScanWindowState scanWindowState, CancellationToken token = default)
        {
            _isAnimating = true;
            
            _scanState.OnNext(new ScanState
            {
                WindowState = scanWindowState,
                CancellableAnimation = new()
                {
                    CancellationToken = token,
                    Callback = OnAnimationEnded,
                },
            });

            while (_isAnimating)
                await UniTask.WaitForEndOfFrame(token);
        }

        public void Dispose()
        {
            _scanState?.Dispose();
            _webTexture?.Dispose();
        }
        
        private void OnAnimationEnded()
        {
            _isAnimating = false;
        }
    }

    public struct ScanState
    {
        public EScanWindowState WindowState;
        public CancellableAnimation CancellableAnimation;
    }

    public struct CancellableAnimation
    {
        public CancellationToken CancellationToken;
        public Action Callback;
    }
}