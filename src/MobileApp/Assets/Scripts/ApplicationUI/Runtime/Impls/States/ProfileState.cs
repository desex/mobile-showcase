using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.ProfileWindow;
using Sotbify.MobileApp.PinPassword;
using Sotbify.MobileApp.UI.Common.Providers;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils.Extensions;
using UState;

namespace Sotbify.MobileApp.ApplicationUI.States
{
    public class ProfileState : State<IApplicationUI>
    {
        private readonly IWindowService _windowService;
        private readonly IRootsRectProvider _rootsRectProvider;
        private readonly IProfile _profile;
        private readonly IPinPasswordService _pinPasswordService;
        private readonly IApplicationUI _applicationUI;
        private readonly ILogger _logger;

        public ProfileState
        (
            IWindowService windowService,
            IRootsRectProvider rootsRectProvider,
            IProfile profile,
            IPinPasswordService pinPasswordService,
            IApplicationUI applicationUI,
            ILogger logger
        )
        {
            _windowService = windowService ?? throw new ArgumentNullException(nameof(windowService));
            _rootsRectProvider = rootsRectProvider ?? throw new ArgumentNullException(nameof(rootsRectProvider));
            _profile = profile ?? throw new ArgumentNullException(nameof(profile));
            _pinPasswordService = pinPasswordService ?? throw new ArgumentNullException(nameof(pinPasswordService));
            _applicationUI = applicationUI ?? throw new ArgumentNullException(nameof(applicationUI));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public override bool ShouldBeAddedToHistory => true;

        protected override async UniTask ExecuteAsync(CancellationToken cancellationToken)
        {
            _profile.LoadProfileData(cancellationToken)
                .SuppressCancellationThrow()
                .Forget(e => _logger.Error("Failed to load profile", e, context: this));
            
            var hasPassword = _pinPasswordService.HasPassword;
            _profile.SetPasswordEnabled(hasPassword);
            await _windowService
                .OpenAsync<IProfile>(_rootsRectProvider.NexusRoot, cancellationToken: cancellationToken);

            var closeTask = _profile.CloseProfile
                .FirstAsync(cancellationToken)
                .AsUniTask();
            
            var editTask = _profile.EditCharacterObservable
                .FirstAsync(cancellationToken)
                .AsUniTask();
            
            var passwordLoginTask = _profile.PasswordLoginObservable
                .FirstAsync(cancellationToken)
                .AsUniTask();
            
            var (winTask, _, _, _) = await UniTask.WhenAny(closeTask, editTask, passwordLoginTask);

            switch (winTask)
            {
                case 0:
                    _applicationUI.Undo();
                    break;
                case 1:
                    _applicationUI.RequestTransition<ProfileCharacterChooseState>();
                    break;
                case 2:
                    if (hasPassword)
                        _applicationUI.RequestTransition<TwoFactorWindowState>();
                    else
                        _applicationUI.RequestTransition<HiddenModeWindowState>();
                    break;
            }
        }

        protected override UniTask ExitAsync(CancellationToken cancellationToken)
        {
            _windowService.CloseAsync<IProfile>(cancellationToken)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to close profile", this);
            return UniTask.CompletedTask;
        }
    }
}
