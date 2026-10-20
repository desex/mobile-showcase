using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Firebase.RemoteConfig;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Utils.Extensions;
using VContainer.Unity;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.Services.RemoteConfig
{
    public class FirebaseRemoteConfigService : IRemoteConfigService, IAsyncStartable, IDisposable
    {
        private readonly IDictionary<string, string> _defaults;
        private readonly ILogger _logger;

        public FirebaseRemoteConfigService
        (
            IDictionary<string, string> defaults,
            ILogger logger
        )
        {
            _defaults = defaults;
            _logger = logger;
        }

        public async UniTask StartAsync(CancellationToken cancellationToken = default)
        {
            var dictionary = new Dictionary<string, object>();
            foreach (var (key, value) in _defaults)
            {
                dictionary.Add(key, value);
            }
            
            await Firebase.FirebaseApp.CheckAndFixDependenciesAsync();
            
            await Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(dictionary).AsUniTask();

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.Information("Fetching remote configs");
            
            await Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero).AsUniTask();
            
            var remoteConfig = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance;
            var info = remoteConfig.Info;
            
            if(info.LastFetchStatus != LastFetchStatus.Success) {
                _logger.Error($"Fetch was unsuccessful\n{nameof(info.LastFetchStatus)}: {info.LastFetchStatus}");
                return;
            }

            await remoteConfig.ActivateAsync().AsUniTask();
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.Information("Remote configs loaded");

            FirebaseRemoteConfig.DefaultInstance.OnConfigUpdateListener += UpdateConfig;
        }

        private void UpdateConfig(object sender, ConfigUpdateEventArgs e)
        {
            FirebaseRemoteConfig.DefaultInstance
                .ActivateAsync()
                .AsUniTask()
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to activate remote config", this);
        }

        public void Dispose()
        {
            FirebaseRemoteConfig.DefaultInstance.OnConfigUpdateListener -= UpdateConfig;
        }

        public string GetString(string key)
        {
            return FirebaseRemoteConfig.DefaultInstance
                .GetValue(key)
                .StringValue;
        }

        public bool GetBool(string key)
        {
            return FirebaseRemoteConfig.DefaultInstance
                .GetValue(key)
                .BooleanValue;
        }

        public long GetLong(string key)
        {
            return FirebaseRemoteConfig.DefaultInstance
                .GetValue(key)
                .LongValue;
        }

        public double GetDouble(string key)
        {
            return FirebaseRemoteConfig.DefaultInstance
                .GetValue(key)
                .DoubleValue;
        }
    }
}