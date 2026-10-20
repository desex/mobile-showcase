using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.FAQWindow;
using Sotbify.MobileApp.UI.Common.Providers;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils.Extensions;
using UState;

namespace Sotbify.MobileApp.ApplicationUI.States
{
    public class FAQCategoryState : State<IApplicationUI, FAQCategoryState.Payload>
    {
        public struct Payload
        {
            public FAQInfo FAQInfo;
        }
        
        private readonly IWindowService _windowService;
        private readonly IApplicationUI _applicationUI;
        private readonly IRootsRectProvider _rootsRectProvider;
        private readonly IFAQContent _faqContent;
        private readonly ILogger _logger;

        public FAQCategoryState(
            IWindowService windowService,
            IApplicationUI applicationUI,
            IRootsRectProvider rootsRectProvider,
            IFAQContent faqContent,
            ILogger logger
        )
        {
            _windowService = windowService;
            _applicationUI = applicationUI;
            _rootsRectProvider = rootsRectProvider;
            _faqContent = faqContent;
            _logger = logger;
        }

        protected override async UniTask ExecuteAsync(Payload payload, CancellationToken cancellationToken)
        {
            _faqContent.Initialize(payload.FAQInfo);
            
            var faqContentWindow = await _windowService.OpenAsync<IFAQContent>(_rootsRectProvider.OverlayRoot, cancellationToken: cancellationToken);
            
            await faqContentWindow.BackButtonPressed
                .AnyAsync(cancellationToken)
                .AsUniTask();
            
            _applicationUI.Undo();
        }
        
        protected override UniTask ExitAsync(CancellationToken cancellationToken)
        {
            _windowService.CloseAsync<IFAQContent>(cancellationToken)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to close faq content", this);
            return base.ExitAsync(cancellationToken);
        }
    }
}