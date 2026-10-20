using System.Collections.Generic;

namespace Sotbifay.Analytics
{
    public interface IAnalyticsEventBuilder<in TDestination>
    {
        IAnalyticsEventBuilder<TDestination> WithDestination(TDestination destination);
        IAnalyticsEventBuilder<TDestination> WithName(string eventName);
        IAnalyticsEventBuilder<TDestination> WithParameter(string key, object value);
        IAnalyticsEventBuilder<TDestination> WithParameters(IEnumerable<KeyValuePair<string, object>> parameters);
        IAnalyticsEventBuilder<TDestination> WithParameters(params KeyValuePair<string, object>[] parameters);

        IAnalyticsEvent Build();
    }
}