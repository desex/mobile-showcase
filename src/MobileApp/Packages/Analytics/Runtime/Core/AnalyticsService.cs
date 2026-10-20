using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine.Pool;

namespace Sotbifay.Analytics
{
    public abstract class AnalyticsService<TDestination> : IAnalyticsService<TDestination>, IDisposable
    {
        private readonly Dictionary<TDestination, QueuedAnalyticsSink> _queuedSinks = new();

        private readonly IAnalyticsCommonParametersResolver _parametersResolver;
        private readonly IAnalyticsLogger _logger;

        private bool _isDisposed;

        protected AnalyticsService
        (
            IReadOnlyCollection<KeyValuePair<TDestination, IAnalyticsSink>> sinks,
            IAnalyticsCommonParametersResolver parametersResolver,
            IAnalyticsLogger logger = null
        )
        {
            if (sinks is null)
                throw new ArgumentNullException(nameof(sinks));

            _parametersResolver = parametersResolver ?? throw new ArgumentNullException(nameof(parametersResolver));
            _logger = logger;

            foreach (var (destination, sink) in sinks)
            {
                if (sink is null)
                {
                    if (_logger.IsEnabledWarning())
                        _logger.Warning($"Sink is null for {destination}", context: this);

                    continue;
                }

                if (_queuedSinks.ContainsKey(destination))
                {
                    if (_logger.IsEnabledWarning())
                        _logger.Warning($"A sink for {destination} has already been assigned", context: this);
                    
                    continue;
                }

                _queuedSinks[destination] = new QueuedAnalyticsSink(sink, _logger);
            }
        }

        public void Send(TDestination destination, string eventName, IEnumerable<KeyValuePair<string, object>> parameters = null)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(GetType().Name);

            if (destination is null)
                throw new ArgumentNullException(nameof(destination));

            if (string.IsNullOrEmpty(eventName))
                throw new ArgumentException("Cannot be null or empty", nameof(eventName));

            if (PlayerLoopHelper.IsMainThread)
            {
                SendOnMainThread(destination, eventName, parameters);
                return;
            }
            
            if (_logger.IsEnabledWarning())
            {
                _logger.Warning
                (
                    $"An attempt to send the event {eventName} from a non-main thread was detected." +
                    " Common parameters will be calculated on the next frame",
                    context: this
                );
            }

            var parametersCopy = parameters?.ToList();
            PlayerLoopHelper.AddContinuation
            (
                PlayerLoopTiming.Update,
                () => SendOnMainThread(destination, eventName, parametersCopy)
            );
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;
            foreach (var queuedSink in _queuedSinks.Values)
                queuedSink.Dispose();
        }

        protected abstract IEnumerable<TDestination> FilteringDestinations
        (
            TDestination destination,
            IReadOnlyCollection<TDestination> sinkDestinations
        );

        private void SendOnMainThread(TDestination destination, string eventName, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            if (_isDisposed)
                return;

            using (ListPool<KeyValuePair<string, object>>.Get(out var parametersList))
            {
                _parametersResolver.Get(parametersList);

                if (parameters is not null)
                    parametersList.AddRange(parameters);

                var filteredDestinations = FilteringDestinations(destination, _queuedSinks.Keys);
                foreach (var filteredDestination in filteredDestinations)
                {
                    if (_queuedSinks.TryGetValue(filteredDestination, out var sink) == false)
                        continue;

                    sink.Send(eventName, parametersList);
                }
            }
        }
    }
}