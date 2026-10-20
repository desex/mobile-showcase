using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Db.ScanParameters;
using Sotbify.MobileApp.PermissionService;
using Sotbify.MobileApp.UI.Window.Presenter;
using Sotbify.MobileApp.Utils.Enums;
using Sotbify.MobileApp.Utils.Extensions;
using UnityEngine.Android;

namespace Sotbify.MobileApp.ScanWindow
{
    public class ScanWindowPresenter : WindowPresenter<IScanWindow, ScanWindowView>
    {
        private readonly IPermissionService _permissionService;
        private readonly IScanWindowParameters _scanWindowParameters;
        private readonly ILogger _logger;
        private IDisposable _bindings;

        public ScanWindowPresenter(
            IPermissionService permissionService,
            IScanWindowParameters scanWindowParameters,
            ILogger logger
        )
        {
            _permissionService = permissionService;
            _scanWindowParameters = scanWindowParameters;
            _logger = logger;
        }

        public override void Initialize()
        {
            var disposableBuilder = new DisposableBuilder();
            
            Model.CameraState
                .Where(state => state != EPermissionStates.None)
                .SubscribeAwait(View, static (state, view, cancellationToken) 
                    => view.ChangeCameraState(state, cancellationToken))
                .AddTo(ref disposableBuilder);
            
            Model.WebTexture
                .Subscribe(View, static (texture, view) => view.AssignCameraTexture(texture))
                .AddTo(ref disposableBuilder);
            
            View.OpenSettingsButtonObservable
                .Subscribe(Model,  static (_, model) => model.OpenSettings())
                .AddTo(ref disposableBuilder);
            
            View.GrantAccessButtonObservable
                .SubscribeAwait(Model, static (_, model, cancellationToken) 
                    => model.RequestCameraAccess(cancellationToken))
                .AddTo(ref disposableBuilder);
            
            View.ScanAgainButtonObservable.
                SubscribeAwait((this, Model, View), static (_, @params, cancellationToken) 
                    =>
                {
                    var (@this, model, view) = @params;
                    view.ResetState();
                    model.Reset();
                    return @this.ProceedStates(cancellationToken);
                })
                .AddTo(ref disposableBuilder);
            
            View.OpenSettingsButtonObservable
                .Subscribe(Model, static (_, model) => model.OpenSettings())
                .AddTo(ref disposableBuilder);
            
            View.CloseWindowButtonObservable
                .Subscribe(Model, static (_, model) => model.Close())
                .AddTo(ref disposableBuilder);
            
            View.NotNowButtonObservable
                .Subscribe(Model, static (_, model) => model.Close())
                .AddTo(ref disposableBuilder);
            
            _bindings?.Dispose();
            _bindings = disposableBuilder.Build();
        }

        protected override async UniTask BeforeShowAsync(CancellationToken cancellationToken)
        {
            if (_permissionService.HasPermission(Permission.Camera))
                await View.ChangeCameraState(EPermissionStates.Granted, cancellationToken);
            else
                await View.ChangeCameraState(EPermissionStates.None, cancellationToken);
        }

        protected override UniTask AfterShowAsync(CancellationToken cancellationToken)
        {
            ProceedStates(cancellationToken)
                .ForgetIgnoreOperationCancelledException(_logger, "An error occured in ProceedStates", this);
            
            return UniTask.CompletedTask;
        }

        private async UniTask ProceedStates(CancellationToken cancellationToken)
        {
            if (_permissionService.HasPermission(Permission.Camera) == false)
                await Model.CameraState
                    .FirstAsync(state => state == EPermissionStates.Granted, cancellationToken)
                    .AsUniTask();
            
            Model.StartScan();
            
            await View.ChangeScanState(EScanWindowState.WaitForQr, cancellationToken);
            
#if UNITY_EDITOR
            var qrContent = _scanWindowParameters.EditorQRCode;
#else
            var qrContent = await Model.ScanQRCode(cancellationToken);
#endif

            await View.ChangeScanState(EScanWindowState.Pending, cancellationToken);
            
            var result = await Model.TryAuthenticate(qrContent, cancellationToken);
            
            await View.ChangeScanState(result, cancellationToken);

            if (result == EScanWindowState.Success) 
                Model.QrCodeSucceed(qrContent);
        }

        protected override UniTask AfterHideAsync(CancellationToken cancellationToken)
        {
            Model.StopScan();
            return base.AfterHideAsync(cancellationToken);
        }

        protected override void OnDispose()
        {
            _bindings?.Dispose();
            _bindings = null;
        }
    }
}