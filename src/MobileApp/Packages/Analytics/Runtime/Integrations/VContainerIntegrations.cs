#if VCONTAINER_SUPPORT

using System;
using System.Collections.Generic;
using UnityEngine.Pool;
using VContainer;
using VContainer.Unity;

namespace Sotbifay.Analytics
{
    public static class VContainerIntegrations
    {
        public static void RegisterAnalyticsEventBuilder<TDestination>(this IContainerBuilder builder)
        {
            builder.RegisterFactory<AnalyticsEventBuilder<TDestination>>(resolver =>
            {
                var service = resolver.Resolve<IAnalyticsService<TDestination>>();
                return () => new AnalyticsEventBuilder<TDestination>(service);
            }, Lifetime.Singleton);

            builder.RegisterEntryPoint
                (resolver =>
                    {
                        var analyticsEventBuilderFactory = resolver.Resolve<Func<AnalyticsEventBuilder<TDestination>>>();
                        return new ObjectPool<AnalyticsEventBuilder<TDestination>>
                        (
                            analyticsEventBuilderFactory,
                            actionOnRelease: static builder => builder.Clear(),
                            actionOnDestroy: static builder => builder.Clear()
                        );
                    },
                    Lifetime.Singleton
                )
                .As<IObjectPool<AnalyticsEventBuilder<TDestination>>>();
        }
    }
}

#endif