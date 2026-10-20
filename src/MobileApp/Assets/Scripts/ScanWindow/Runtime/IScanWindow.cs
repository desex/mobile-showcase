using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.Utils.Enums;
using UnityEngine;

namespace Sotbify.MobileApp.ScanWindow
{
    public interface IScanWindow
    {
        public ReadOnlyReactiveProperty<EPermissionStates> CameraState { get; }
        public Observable<WebCamTexture> WebTexture { get; }
        public Observable<Unit> CloseWindow { get; }
        public Observable<Unit> GoToStabApp { get; }
        public Observable<Unit> GoToApp { get; }

        public void StartScan();
        public UniTask<string> ScanQRCode(CancellationToken token = default);
        public void StopScan();
        
        public UniTask RequestCameraAccess(CancellationToken cancellationToken);
        public void OpenSettings();
        public void Close();
        public UniTask<EScanWindowState> TryAuthenticate(string qrContent, CancellationToken cancellationToken);
        public void QrCodeSucceed(string qrCode);
        public void Reset();
    }
}