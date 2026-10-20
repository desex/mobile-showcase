using System.Collections.Generic;

namespace Sotbifay.Analytics
{
    public interface IAnalyticsSink
    {
        bool IsInitialized { get; }
        
        void Send(string eventName, IEnumerable<KeyValuePair<string, object>> parameters);
    }
}