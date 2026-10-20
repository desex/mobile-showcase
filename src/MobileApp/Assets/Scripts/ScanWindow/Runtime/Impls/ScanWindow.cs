using System;
using System.Net;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.Common.Preferences;
using Sotbify.MobileApp.PermissionService;
using Sotbify.MobileApp.Server;
using Sotbify.MobileApp.Server.HttpClient.Core.Exceptions;
using Sotbify.MobileApp.Services.QRScanner;
using Sotbify.MobileApp.Services.SettingsOpener;
using Sotbify.MobileApp.Services.TutorialService;
using Sotbify.MobileApp.Utils;
using Sotbify.MobileApp.Utils.Data;
using Sotbify.MobileApp.Utils.Enums;
using UnityEngine;
using UnityEngine.Android;

namespace Sotbify.MobileApp.ScanWindow
{
    public class ScanWindow : IScanWindow, IDisposable
    {
        private readonly ReactiveProperty<EPermissionStates> _cameraState = new();
        private readonly Subject<WebCamTexture> _webTexture = new();
        private readonly Subject<Unit> _closeWindow = new();
        private readonly Subject<Unit> _goToStabApp = new();
        private readonly Subject<Unit> _goToApp = new();
        private readonly IPermissionService _permissionService;
        private readonly IQRScannerService _qrScannerService;
        private readonly Client _client;
        private readonly ISettingsOpener _settingsOpener;
        private readonly ITutorialStepService _tutorialStepService;
        private readonly IPreferences _preferences;

        public ScanWindow(
            IPermissionService permissionService,
            IQRScannerService qrScannerService,
            Client client,
            ISettingsOpener settingsOpener,
            ITutorialStepService tutorialStepService,
            IPreferences preferences
        )
        {
            _permissionService = permissionService;
            _qrScannerService = qrScannerService;
            _client = client;
            _settingsOpener = settingsOpener;
            _tutorialStepService = tutorialStepService;
            _preferences = preferences;
        }
        
        public ReadOnlyReactiveProperty<EPermissionStates> CameraState => _cameraState;
        public Observable<WebCamTexture> WebTexture => _webTexture;
        public Observable<Unit> CloseWindow => _closeWindow;
        public Observable<Unit> GoToStabApp => _goToStabApp;
        public Observable<Unit> GoToApp => _goToApp;

        public void ChangeCameraState(EPermissionStates permissionState) 
            => _cameraState.Value = permissionState;

        public void StartScan()
        {
#if UNITY_EDITOR_OSX
            return;
#endif
            
            var webCamTexture = _qrScannerService.Start();
            if (webCamTexture == null)
                throw new NullReferenceException("WebCamTexture is null");
            
            _webTexture.OnNext(webCamTexture);
        }
        
        public UniTask<string> ScanQRCode(CancellationToken token = default)
        {
            return _qrScannerService.ScanAsync(token);
        }

        public void StopScan()
        {
            _qrScannerService.Stop();
            _webTexture.OnNext(null);
        }

        public async UniTask RequestCameraAccess(CancellationToken cancellationToken)
        {
#if UNITY_EDITOR
            await UniTask.WaitForSeconds(1f, cancellationToken: cancellationToken);
            _cameraState.Value = EPermissionStates.Granted;
#elif UNITY_ANDROID
            _cameraState.Value = await _permissionService.RequestPermissionAndroidAsync(Permission.Camera, cancellationToken);
#elif UNITY_IOS
            _cameraState.Value = await _permissionService.RequestPermissionIOSAsync(Permission.Camera, cancellationToken);
#endif
        }

        public void OpenSettings() 
            => _settingsOpener.OpenSettings();

        public void Close()
        {
            _closeWindow.OnNext(Unit.Default);
            _cameraState.Value = EPermissionStates.None;
        }

        public async UniTask<EScanWindowState> TryAuthenticate(string qrContent, CancellationToken cancellationToken)
        {
            if (IsTokenStab(qrContent))
                return EScanWindowState.Success;
            
            try
            {
                var result = await _client.TryAuthenticateWithHttpException(qrContent, cancellationToken);
                return result ? EScanWindowState.Success : EScanWindowState.Failed;
            }
            catch (HttpResponseException httpResponseException)
            {
                return httpResponseException.StatusCode == (int)HttpStatusCode.NotFound ? 
                    EScanWindowState.Failed : 
                    EScanWindowState.ApiFailed;
            }
        }

        public void QrCodeSucceed(string qrCode)
        {
            _preferences.SetString(SavePath.QRSavePath, qrCode);
            
            if (IsTokenStab(qrCode))
            {
                _goToStabApp.OnNext(Unit.Default);
                return;
            }
            
            _goToApp.OnNext(Unit.Default);
        }

        public void Reset()
        {
            _cameraState.Value = EPermissionStates.None;
        }

        public void Dispose()
        {
            _cameraState?.Dispose();
            _closeWindow?.Dispose();
        }
        
        private bool IsTokenStab(string token)
        {
            var stabTokens = _preferences.Get<StabQRTokens>(SavePath.StabQRs);
            
            foreach (var stabToken in stabTokens.Tokens)
            {
                if (token != stabToken)
                    continue;

                return true;
            }

            return false;
        }
    }
}