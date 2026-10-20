using System;
using System.Threading;
using AppTrackingTransparency;
using AppTrackingTransparency.Common;
using Cysharp.Threading.Tasks;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Utils.Extensions;

namespace Services.AppTransparencyService
{
    public class AppTransparencyService : IAppTransparencyService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private IAppTrackingTransparencyManager _appTrackingTransparencyManager;
        
        public AppTrackingTransparencyAuthorizationStatus Status => _appTrackingTransparencyManager.TrackingAuthorizationStatus;

        public AppTransparencyService(ILogger logger)
        {
            _logger = logger;
        }

        public void RequestPermission()
        {
            if (AppTrackingTransparencyModule.IsSupported == false)
                return;
            
            _appTrackingTransparencyManager.RequestTrackingAuthorization
            (status =>
                {
                    if (status != AppTrackingTransparencyAuthorizationStatus.Authorized) 
                        return;
                    
                    if (_logger.IsEnabled(LogLevel.Information))
                        _logger.Information("Authorization transparency given");
                }
            );
        }

        public void Track()
        {
            if (AppTrackingTransparencyModule.IsSupported == false)
                return;

            _appTrackingTransparencyManager = AppTrackingTransparencyModule.CreateManager();
            
            TrackInternalAsync(_cancellationTokenSource.Token).ForgetIgnoreOperationCancelledException(_logger, "Exception while track transparency", this);
        }

        private async UniTask TrackInternalAsync(CancellationToken cancellationToken)
        {
            while (cancellationToken.IsCancellationRequested == false)
            {
                _appTrackingTransparencyManager.Update();
                await UniTask.WaitForEndOfFrame(cancellationToken);
            }   
        }

        public void Dispose()
        {
            _cancellationTokenSource.Dispose();
        }
    }
}