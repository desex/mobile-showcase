using Sotbifay.MobileApp.Logger;
using Sotbifay.MobileApp.Logger.Sinks.Runtime;
using VContainer;
using VContainer.Unity;

namespace Sotbify.MobileApp.Logger
{
    public sealed class LoggerProjectInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<ILogSink, UnityLogSink>(Lifetime.Singleton);
            builder.Register<ILogSink, AppMetricaLogSink>(Lifetime.Singleton);

#if UNITY_EDITOR == false
            builder.Register<ILogSink, FirebaseCrashlyticsLogSink>(Lifetime.Singleton);
#endif
            
            builder.Register<Sotbifay.MobileApp.Logger.Logger>(Lifetime.Singleton)
                .As<ILogger>();
            
            builder.RegisterBuildCallback(resolver =>
            {
                var logger = resolver.Resolve<ILogger>();

#if UNITY_EDITOR
                logger.MinLevel = LogLevel.Verbose;
#elif DEBUG
                logger.MinLevel = LogLevel.Debug;
#else
                logger.MinLevel = LogLevel.Information;
#endif
                logger.Information($"Min log level: {logger.MinLevel}");
                StaticLogger.Logger = logger;
            });
        }
    }
}