using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Crashlytics;

namespace Sotbify.MobileApp.Services.FirebaseCrashlyticsService
{
    public class FirebaseCrashlyticsService : IFirebaseCrashlyticsService
    {
        public bool IsInitialized { get; private set; }

        public async UniTask Initialize(CancellationToken cancellationToken = default)
        {
            var dependencyStatus = await FirebaseApp
                .CheckAndFixDependenciesAsync()
                .AsUniTask()
                .AttachExternalCancellation(cancellationToken);

            if (dependencyStatus == DependencyStatus.Available)
            {
                Crashlytics.ReportUncaughtExceptionsAsFatal = true;
                IsInitialized = true;
            }
        }

        public void Log(string message)
        {
            Crashlytics.Log(message);
        }

        public void LogException(Exception exception)
        {
            Crashlytics.LogException(exception);
        }
    }
}