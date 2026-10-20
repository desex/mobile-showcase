using System;
using System.Collections.Generic;

namespace Sotbifay.Analytics
{
    public sealed class AnalyticsEventBuilder<TDestination> : IAnalyticsEventBuilder<TDestination>
    {
        private readonly IAnalyticsService<TDestination> _analyticsService;
        private readonly Dictionary<string, object> _parameters = new();

        private string _eventName = string.Empty;
        private TDestination _destination;

        public AnalyticsEventBuilder(IAnalyticsService<TDestination> analyticsService)
        {
            _analyticsService = analyticsService ?? throw new ArgumentNullException(nameof(analyticsService));
        }

        public IAnalyticsEventBuilder<TDestination> WithDestination(TDestination destination)
        {
            _destination = destination ?? throw new ArgumentNullException(nameof(destination));
            return this;
        }

        public IAnalyticsEventBuilder<TDestination> WithName(string eventName)
        {
            _eventName = string.IsNullOrEmpty(eventName) == false
                ? eventName
                : throw new ArgumentException("Cannot be null or empty", nameof(eventName));

            return this;
        }

        public IAnalyticsEventBuilder<TDestination> WithParameter(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Cannot be null or empty", nameof(key));

            if (value is null)
                throw new ArgumentNullException(nameof(value));

            return _parameters.TryAdd(key, value)
                ? this
                : throw new InvalidOperationException($"Parameter with key '{key}' already exists");
        }

        public IAnalyticsEventBuilder<TDestination> WithParameters(IEnumerable<KeyValuePair<string, object>> parameters)
        {
            foreach (var parameter in parameters)
                WithParameter(parameter.Key, parameter.Value);

            return this;
        }

        public IAnalyticsEventBuilder<TDestination> WithParameters(params KeyValuePair<string, object>[] parameters)
        {
            foreach (var parameter in parameters)
                WithParameter(parameter.Key, parameter.Value);

            return this;
        }

        public IAnalyticsEvent Build() => new AnalyticsEvent<TDestination>(_analyticsService, _destination, _eventName, _parameters);

        public void Clear()
        {
            _parameters.Clear();
            _eventName = string.Empty;
            _destination = default;
        }
    }
}