using System;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace Sotbifay.Analytics
{
    public sealed class AnalyticsEvent<TDestination> : IAnalyticsEvent
    {
        private readonly IAnalyticsService<TDestination> _analyticsService;
        private readonly TDestination _destination;
        private readonly string _eventName;
        private readonly Dictionary<string, object> _parameters;

        private bool _isDisposed;

        public AnalyticsEvent
        (
            IAnalyticsService<TDestination> analyticsService,
            TDestination destination,
            string eventName,
            IEnumerable<KeyValuePair<string, object>> parameters = null
        )
        {
            _analyticsService = analyticsService ?? throw new ArgumentNullException(nameof(analyticsService));
            _destination = destination ?? throw new ArgumentNullException(nameof(destination));
            _eventName = string.IsNullOrEmpty(eventName) == false
                ? eventName
                : throw new ArgumentException("Cannot be null or empty", nameof(eventName));

            _parameters = DictionaryPool<string, object>.Get();
            // ReSharper disable once InvertIf
            if (parameters is not null)
            {
                foreach (var parameter in parameters)
                    _parameters.TryAdd(parameter.Key, parameter.Value);
            }
        }

        public void Send()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(AnalyticsEvent<TDestination>));

            _analyticsService.Send(_destination, _eventName, _parameters);
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;
            DictionaryPool<string, object>.Release(_parameters);
        }
    }
}