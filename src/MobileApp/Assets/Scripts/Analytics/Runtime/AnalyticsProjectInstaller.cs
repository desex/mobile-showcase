using System;
using System.Collections.Generic;
using Sotbifay.Analytics;
using Sotbify.MobileApp.Analytics.AnalyticsSinks;
using UnityEngine.Pool;
using VContainer;
using VContainer.Unity;

namespace Sotbify.MobileApp.Analytics
{
    public sealed class AnalyticsProjectInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
#if USE_APPMETRICA
            builder.RegisterEntryPoint<AppMetricaAnalyticsSink>()
                .AsSelf();
#endif
            
            builder.RegisterEntryPoint<SotbifayAnalyticsSink>()
                .AsSelf();
            
            builder.Register<AnalyticsLogger>(Lifetime.Singleton)
                .As<IAnalyticsLogger>();
            
            builder.Register<AnalyticsCommonParametersResolver>(Lifetime.Singleton)
                .As<IAnalyticsCommonParametersResolver>();
            
            builder.RegisterEntryPoint<AnalyticsService>()
                .WithParameter<IReadOnlyCollection<KeyValuePair<AnalyticsDestination, IAnalyticsSink>>>(resolver =>
                {
                    var result = new Dictionary<AnalyticsDestination, IAnalyticsSink>();

#if USE_APPMETRICA
                    if (resolver.TryResolve<AppMetricaAnalyticsSink>(out var appMetricaAnalyticsSink)) 
                        result.Add(AnalyticsDestination.AppMetrica, appMetricaAnalyticsSink);
#endif

                    if (resolver.TryResolve<SotbifayAnalyticsSink>(out var sotbifayAnalyticsSink)) 
                        result.Add(AnalyticsDestination.Sotbifay, sotbifayAnalyticsSink);

                    return result;
                })
                .As<IAnalyticsService<AnalyticsDestination>>();
            
            builder.RegisterAnalyticsEventBuilder<AnalyticsDestination>();
            RegisterCommonParameters(builder);
            builder.Register<AnalyticsEvents>(Lifetime.Singleton)
                .AsSelf();
        }

        private static void RegisterCommonParameters(IContainerBuilder builder)
        {
            builder.Register<IAnalyticsCommonParameter, SessionIdCommonParameter>(Lifetime.Singleton);
            builder.Register<IAnalyticsCommonParameter, CreationTimeCommonParameter>(Lifetime.Singleton);
            builder.Register<IAnalyticsCommonParameter, ApplicationVersionCommonParameter>(Lifetime.Singleton);

            builder.Register<IAnalyticsCommonParameter, ApplicationStateCommonParameter>(Lifetime.Singleton)
                .AsSelf();
        }
    }
}