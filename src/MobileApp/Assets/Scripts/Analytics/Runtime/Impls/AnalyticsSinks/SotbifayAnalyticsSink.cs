using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbifay.Analytics;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Server;
using UnityEngine.Pool;
using VContainer.Unity;

namespace Sotbify.MobileApp.Analytics.AnalyticsSinks
{
    public sealed class SotbifayAnalyticsSink : IAnalyticsSink, IAsyncStartable, IDisposable
    {
        private static readonly TimeSpan RequestQueueSendingInterval = TimeSpan.FromSeconds(1f);
        
        private readonly struct PendingEvent : IDisposable
        {
            private readonly Dictionary<string, object> _parameters;

            public PendingEvent(string name, IEnumerable<KeyValuePair<string, object>> parameters)
            {
                Name = name;
                _parameters = DictionaryPool<string, object>.Get();

                if (parameters is null)
                    return;

                foreach (var (key, value) in parameters)
                    _parameters.TryAdd(key, value);
            }

            public string Name { get; }
            public IEnumerable<KeyValuePair<string, object>> Parameters => _parameters;

            public void Dispose()
            {
                DictionaryPool<string, object>.Release(_parameters);
            }
        }
        
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly ConcurrentQueue<PendingEvent> _pendingEvents = new();
        
        private readonly Client _client;
        private readonly IServer _server;
        private readonly ILogger _logger;

        public SotbifayAnalyticsSink(Client client, IServer server, ILogger logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _server = server ?? throw new ArgumentNullException(nameof(server));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool IsInitialized { get; private set; }
        
        public void Send(string eventName, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            var request = new PendingEvent(eventName, parameters);
            _pendingEvents.Enqueue(request);
        }

        public async UniTask StartAsync(CancellationToken cancellationToken)
        {
            await UniTask.WaitWhile(_server, static server => string.IsNullOrEmpty(server.AccessToken), cancellationToken: cancellationToken);
            IsInitialized = true;
            
            ProcessLoopAsync(_cancellationTokenSource.Token).Forget();
        }
        
        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        private async UniTaskVoid ProcessLoopAsync(CancellationToken cancellationToken)
        {
            while (cancellationToken.IsCancellationRequested == false)
            {
                try
                {
                    if (_pendingEvents.IsEmpty)
                        continue;

                    var isSuccessSentCompositeEvent = await SendAnalyticsEventsAsCompositeAsync(cancellationToken);
                    if (isSuccessSentCompositeEvent)
                        continue;

                    if (_logger.IsEnabled(LogLevel.Warning))
                        _logger.Warning("Failed to send analytics events as composite. Falling back to sending separately", context: this);

                    await SendAnalyticsEventsSeparatelyAsync(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception e)
                {
                    _logger.Error("Failed to process analytics event queue", e, this);
                }
                finally
                {
                    await UniTask.Delay(RequestQueueSendingInterval, cancellationToken: cancellationToken, ignoreTimeScale: true);
                }
            }
        }

        private async UniTask<bool> SendAnalyticsEventsAsCompositeAsync(CancellationToken cancellationToken)
        {
            var result = false;
            using (ListPool<PendingEvent>.Get(out var eventRequests))
            {
                while (_pendingEvents.TryDequeue(out var request))
                    eventRequests.Add(request);

                using (ListPool<KeyValuePair<string, IEnumerable<KeyValuePair<string, object>>>>.Get(out var eventsList))
                {
                    // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
                    foreach (var eventRequest in eventRequests)
                    {
                        var @event = new KeyValuePair<string, IEnumerable<KeyValuePair<string, object>>>(eventRequest.Name, eventRequest.Parameters);
                        eventsList.Add(@event);
                    }

                    result = await _client.SendAnalyticsEventsAsync(eventsList, cancellationToken);
                }

                if (result)
                {
                    foreach (var eventRequest in eventRequests)
                        eventRequest.Dispose();

                    if (_logger.IsEnabled(LogLevel.Debug))
                        _logger.Debug($"Sent {eventRequests.Count} analytics event(s) as composite", context: this);
                }
                else
                {
                    foreach (var eventRequest in eventRequests)
                        _pendingEvents.Enqueue(eventRequest);

                    if (_logger.IsEnabled(LogLevel.Warning))
                        _logger.Warning($"Composite send failed, returned {eventRequests.Count} analytics event(s) to the queue", context: this);
                }
            }
            
            return result;
        }

        private async UniTask<bool> SendAnalyticsEventsSeparatelyAsync(CancellationToken cancellationToken)
        {
            using (ListPool<PendingEvent>.Get(out var pendingEvents))
            {
                while (_pendingEvents.TryDequeue(out var pendingEvent))
                    pendingEvents.Add(pendingEvent);

                var sentCount = 0;
                var failedCount = 0;
                foreach (var eventRequest in pendingEvents)
                {
                    var result = await _client.SendAnalyticsEventAsync(eventRequest.Name, eventRequest.Parameters, cancellationToken);
                    if (result)
                    {
                        eventRequest.Dispose();
                        sentCount++;
                    }
                    else
                    {
                        _pendingEvents.Enqueue(eventRequest);
                        failedCount++;
                    }
                }

                if (sentCount > 0 && _logger.IsEnabled(LogLevel.Debug))
                    _logger.Debug($"Sent {sentCount} analytics event(s) separately", context: this);

                if (failedCount > 0 && _logger.IsEnabled(LogLevel.Warning))
                    _logger.Warning($"Returned {failedCount} analytics event(s) to the queue after separate send", context: this);

                return failedCount == 0;
            }
        }
    }
}