using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Utils.Extensions;

namespace Sotbify.MobileApp.RelicListWindow.DailyRelicTimer
{
    public class DailyRelicTimerService : IDailyRelicTimerService
    {
        private readonly ILogger _logger;
        private readonly ReactiveProperty<TimeSpan> _timeRemaining = new();
        private CancellationTokenSource _cancellationTokenSource;

        public DailyRelicTimerService(ILogger logger)
        {
            _logger = logger;
        }

        public ReadOnlyReactiveProperty<TimeSpan> TimeRemaining => _timeRemaining;

        public void StartTimer(CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private async UniTask StartTimerInternal(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void StopTimer()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}