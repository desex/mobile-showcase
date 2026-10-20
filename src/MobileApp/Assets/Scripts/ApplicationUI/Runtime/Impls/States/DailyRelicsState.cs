using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.BlurredBackgroundWindow;
using Sotbify.MobileApp.LoadingScreen;
using Sotbify.MobileApp.NewRelicWindow;
using Sotbify.MobileApp.NexusWindow;
using Sotbify.MobileApp.UI.Common.Providers;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils.Enums.Nexus;
using Sotbify.MobileApp.Utils.Extensions;
using UState;

namespace Sotbify.MobileApp.ApplicationUI.States
{
    public sealed class DailyRelicsState : State<IApplicationUI>
    {
        private readonly IWindowService _windowService;
        private readonly INewRelicWindow _newRelicWindow;
        private readonly IRootsRectProvider _rootsRectProvider;
        private readonly INexus _nexus;
        private readonly ILogger _logger;

        public DailyRelicsState
        (
            IWindowService windowService,
            INewRelicWindow newRelicWindow,
            IRootsRectProvider rootsRectProvider,
            INexus nexus,
            ILogger logger
        )
        {
            _windowService = windowService ?? throw new ArgumentNullException(nameof(windowService));
            _newRelicWindow = newRelicWindow ?? throw new ArgumentNullException(nameof(newRelicWindow));
            _rootsRectProvider = rootsRectProvider ?? throw new ArgumentNullException(nameof(rootsRectProvider));
            _nexus = nexus ?? throw new ArgumentNullException(nameof(nexus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        protected override async UniTask ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                var hasDailyBonus = await _newRelicWindow.CheckDailyBonusAsync(cancellationToken);

                if (_logger.IsEnabled(LogLevel.Information))
                    _logger.Information($"Has daily bonus {hasDailyBonus}", context: this);

                if (hasDailyBonus)
                {
                    await _newRelicWindow.LoadInfoAsync(cancellationToken)
                        .Timeout(TimeSpan.FromSeconds(10f));

                    var backgroundBlurOpenTask = _windowService.OpenAsync<IBlurredBackground>
                    (
                        _rootsRectProvider.BackgroundBlurRoot,
                        cancellationToken: cancellationToken
                    );

                    var closeLoadingScreenTask = _windowService.SafeCloseAsync<ILoadingScreen>(cancellationToken);

                    var newRelicWindowOpenTask = _windowService.OpenAsync<INewRelicWindow>(cancellationToken);
                    await UniTask.WhenAll(backgroundBlurOpenTask, newRelicWindowOpenTask, closeLoadingScreenTask);

                    if (_windowService.IsOpen<ILoadingScreen>())
                    {
                        _windowService.CloseAsync<ILoadingScreen>(cancellationToken)
                            .ForgetIgnoreOperationCancelledException(_logger, "Failed to close loading screen", this);
                    }

                    await _newRelicWindow.CloseObservable
                        .AnyAsync(cancellationToken: cancellationToken)
                        .AsUniTask();

                    await _windowService.CloseAsync<INewRelicWindow>(cancellationToken);
                }
            } 
            finally
            {
                _nexus.ChangeDockScreen(ENexusCategory.Home);
            }
        }

        protected override async UniTask ExitAsync(CancellationToken cancellationToken)
        {
            await _windowService.SafeCloseAsync<INewRelicWindow>(cancellationToken: cancellationToken);
            await _newRelicWindow.ReleaseInfoAsync(cancellationToken);

            await _windowService.SafeOpenAsync<INexus>(_rootsRectProvider.NexusRoot, cancellationToken: cancellationToken);
        }
    }
}