using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.Utils.Enums;
using UnityEngine;

namespace Sotbify.MobileApp.GlassesWindow
{
    public interface IGlassesWindow
    {
        public ReadOnlyReactiveProperty<ScanState> ScanState { get; }
        public ReadOnlyReactiveProperty<WebCamTexture> WebTexture { get; }
        
        public UniTask<string> ScanQRCode(CancellationToken token = default);
        public void Start();
        public void Stop();
        public UniTask ChangeState(EScanWindowState scanWindowState, CancellationToken token = default);
    }
}