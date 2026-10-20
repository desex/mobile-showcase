using System.Collections.Generic;
using Sotbifay.Analytics;
using UnityEngine.Pool;

namespace Sotbify.MobileApp.Analytics
{
    public sealed class AnalyticsService : AnalyticsService<AnalyticsDestination>
    {
        public AnalyticsService
        (
            IReadOnlyCollection<KeyValuePair<AnalyticsDestination, IAnalyticsSink>> sinks,
            IAnalyticsCommonParametersResolver parametersResolver,
            IAnalyticsLogger logger
        ) : base(sinks, parametersResolver, logger)
        {
        }
        
        protected override IEnumerable<AnalyticsDestination> FilteringDestinations
        (
            AnalyticsDestination destination,
            IReadOnlyCollection<AnalyticsDestination> sinkDestinations
        )
        {
            using (ListPool<AnalyticsDestination>.Get(out var destinations))
            {
                destinations.AddRange(sinkDestinations);

                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < destinations.Count; i++)
                {
                    var sinkDestination = destinations[i];
                    if ((destination & sinkDestination) == 0)
                        continue;

                    yield return sinkDestination;
                }
            }
        }
    }
}