using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Sotbify.MobileApp.Services.FirebaseCrashlyticsService
{
    public interface IFirebaseCrashlyticsService
    {
        public bool IsInitialized { get; }
        public UniTask Initialize(CancellationToken cancellationToken = default);
        public void Log(string message);
        public void LogException(Exception exception);
    }
}