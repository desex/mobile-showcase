using System.Collections.Generic;

namespace Sotbifay.Analytics
{
    public interface IAnalyticsService<in TDestination>
    {
        void Send(TDestination destination, string eventName, IEnumerable<KeyValuePair<string, object>> parameters = null);
    }
}