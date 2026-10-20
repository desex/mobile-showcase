using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.Common.OSA;
using Sotbify.MobileApp.UI.Window.Presenter;

namespace Sotbify.MobileApp.RelicListWindow
{
    public class RelicListWindowPresenter : WindowPresenter<IRelicListWindow, RelicListWindowView>
    {
        private readonly IDailyRelicTimerService _dailyRelicTimerService;
        private IDisposable _bindings;

        public RelicListWindowPresenter(IDailyRelicTimerService dailyRelicTimerService)
        {
            _dailyRelicTimerService = dailyRelicTimerService;
        }

        public override void Initialize()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override UniTask BeforeShowAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override UniTask AfterHideAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override void OnDispose()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}