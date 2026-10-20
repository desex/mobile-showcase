using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.FAQWindow;
using Sotbify.MobileApp.NexusWindow;
using Sotbify.MobileApp.UI.Common.Providers;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils.Extensions;
using UState;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.ApplicationUI.States
{
    public class FAQWindowState : State<IApplicationUI>
    {
        private readonly IWindowService _windowService;
        private readonly IApplicationUI _applicationUI;
        private readonly IRootsRectProvider _rootsRectProvider;
        private readonly ILogger _logger;
        private readonly INexus _nexus;

        public FAQWindowState(
            IWindowService windowService,
            IApplicationUI applicationUI,
            IRootsRectProvider rootsRectProvider,
            INexus nexus,
            ILogger logger
        )
        {
            _windowService = windowService;
            _applicationUI = applicationUI;
            _rootsRectProvider = rootsRectProvider;
            _nexus = nexus;
            _logger = logger;
        }

        protected override async UniTask ExecuteAsync(CancellationToken cancellationToken)
        {
            var faq = await _windowService.OpenAsync<IFAQ>(_rootsRectProvider.OverlayRoot,
                cancellationToken: cancellationToken);
            
            var faqCategoryTask = faq.OpenCategoryObservable
                .FirstAsync(cancellationToken)
                .AsUniTask();
            
            var openFaqContentTask = faq.OpenContent
                .FirstAsync(cancellationToken)
                .AsUniTask();

            ExecuteIfCanceled(cancellationToken).ForgetIgnoreOperationCancelledException(_logger, "Exception while executing if canceled", this);
            
            var (winTaskIndex, fistCategory, faqInfo) = await UniTask.WhenAny(faqCategoryTask, openFaqContentTask);

            switch (winTaskIndex)
            {
                case 0:
                    _windowService.CloseAsync<IFAQ>(cancellationToken)
                        .ForgetIgnoreOperationCancelledException(_logger, "Failed to close faq window", this);
                    
                    _nexus.ChangeDockScreen(fistCategory);
                    return;
                case 1:
                    _applicationUI.RequestTransition<FAQCategoryState, FAQCategoryState.Payload>
                    (
                        new FAQCategoryState.Payload
                        {
                            FAQInfo = faqInfo
                        }
                    );
                    return;
            }
        }

        private async UniTask ExecuteIfCanceled(CancellationToken cancellationToken)
        {
            await cancellationToken.WaitUntilCanceled();

            _windowService.CloseAsync<IFAQ>(cancellationToken)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to close faq window", this);
        }
    }
}