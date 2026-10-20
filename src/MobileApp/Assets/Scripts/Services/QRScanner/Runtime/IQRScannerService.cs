using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Sotbify.MobileApp.Services.QRScanner
{
    public interface IQRScannerService
    {
        public WebCamTexture Start();
        public UniTask<string> ScanAsync(CancellationToken token = default);
        public void Stop();
    }
}