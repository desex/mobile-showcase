using System;
using System.Threading;
using R3;

namespace Sotbify.MobileApp.RelicListWindow
{
    public interface IDailyRelicTimerService
    {
        public ReadOnlyReactiveProperty<TimeSpan> TimeRemaining { get; }
        public void StartTimer(CancellationToken cancellationToken = default);
        public void StopTimer();

    }
}