using System;
using System.Collections.Generic;

namespace Sotbifay.Analytics
{
    public sealed class AnalyticsCommonParametersResolver : IAnalyticsCommonParametersResolver
    {
        private readonly Dictionary<string, object> _cacheParameters = new();

        private readonly IReadOnlyList<IAnalyticsCommonParameter> _commonParameters;
        private readonly IAnalyticsLogger _logger;

        public AnalyticsCommonParametersResolver
        (
            IReadOnlyList<IAnalyticsCommonParameter> commonParameters = null,
            IAnalyticsLogger logger = null
        )
        {
            _commonParameters = commonParameters ?? Array.Empty<IAnalyticsCommonParameter>();
            _logger = logger;
        }

        public void Get(IList<KeyValuePair<string, object>> result)
        {
            if (_commonParameters.Count == 0)
                return;

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < _commonParameters.Count; i++)
            {
                var parameter = _commonParameters[i];
                var name = parameter.Name;
                var canBeCached = parameter.CanBeCached;

                if (canBeCached && _cacheParameters.TryGetValue(name, out var value))
                {
                    result.Add(new KeyValuePair<string, object>(name, value));
                    continue;
                }

                try
                {
                    value = parameter.GetValue();
                }
                catch (Exception exception)
                {
                    if (_logger.IsEnabledError())
                        _logger.Error($"Error getting common parameter {name}", exception, context: this);

                    continue;
                }

                if (canBeCached && value is not null)
                    _cacheParameters[name] = value;

                result.Add(new KeyValuePair<string, object>(name, value));
            }
        }
    }
}