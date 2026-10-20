using System.Collections.Generic;

namespace Sotbifay.Analytics
{
    public interface IAnalyticsCommonParametersResolver
    {
        void Get(IList<KeyValuePair<string, object>> result);
    }
}