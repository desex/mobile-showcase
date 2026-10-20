using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Io.AppMetrica;
using Newtonsoft.Json;
using Sotbifay.Analytics;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Common.Preferences;
using UnityEngine;
using UnityEngine.Pool;
using VContainer.Unity;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.Analytics.AnalyticsSinks
{
    public class AppMetricaAnalyticsSink : IAnalyticsSink, IAsyncStartable
    {
        private const string IsFirstInitializationPreferenceKey =
            nameof(AppMetricaAnalyticsSink) + "_IsFirstInitialization";

        private const string ApiKey =
#if PARTNER_CRYPTOBOSS && DEBUG == false 
                "Секрет"
#elif PARTNER_RIOBET && DEBUG == false
                "Секрет"
#else
                "Секрет"
#endif
            ;

        private readonly IPreferences _preferences;
        private readonly ILogger _logger;

        public AppMetricaAnalyticsSink(IPreferences preferences, ILogger logger)
        {
            _preferences = preferences ?? throw new ArgumentNullException(nameof(preferences));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public bool IsInitialized { get; private set; } = false;
        
        public void Send(string eventName, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            using (DictionaryPool<string, object>.Get(out var parametersDictionary))
            {
                foreach (var (key, value) in parameters)
                    parametersDictionary.TryAdd(key, value);

                if (parametersDictionary.Count <= 0)
                {
                    AppMetrica.ReportEvent(eventName);
                    return;
                }

                var json = JsonConvert.SerializeObject(parametersDictionary, Formatting.Indented);
                AppMetrica.ReportEvent(eventName, json);
            }
        }
        
        public async UniTask StartAsync(CancellationToken cancellationToken)
        {
            var isFirstInitialization = _preferences.GetBool(IsFirstInitializationPreferenceKey, true);
            
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.Information("Initialized", context: this);
            
            var config = GetConfig(isFirstInitialization);
            await ActivationAsync(config, cancellationToken);
            
            _preferences.SetBool(IsFirstInitializationPreferenceKey, false);
            IsInitialized = true;
        }
        
        private static async UniTask ActivationAsync(AppMetricaConfig config, CancellationToken cancellationToken)
        {
            if (AppMetrica.IsActivated())
                return;
            
            var completionSource = new UniTaskCompletionSource();
            try
            {
                AppMetrica.OnActivation += AppMetricaOnOnActivation;
                AppMetrica.Activate(config);
                
                await completionSource.Task
                    .AttachExternalCancellation(cancellationToken);
            }
            finally
            {
                AppMetrica.OnActivation -= AppMetricaOnOnActivation;
            }
            return;

            void AppMetricaOnOnActivation(AppMetricaConfig appMetricaConfig) =>
                completionSource.TrySetResult();
        }

        private AppMetricaConfig GetConfig(bool isFirstInitialization)
        {
            var config = new AppMetricaConfig(ApiKey)
            {
                AppVersion = Application.version,
                CrashReporting = true,
                DataSendingEnabled = true,
                FirstActivationAsUpdate = isFirstInitialization == false,
                LocationTracking = true,
                Logs = _logger.IsEnabled(LogLevel.Debug),
                NativeCrashReporting = true
            };

            return config;
        }
    }
}