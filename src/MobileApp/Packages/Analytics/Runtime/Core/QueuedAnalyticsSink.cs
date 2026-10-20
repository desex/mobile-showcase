using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Pool;

namespace Sotbifay.Analytics
{
    internal sealed class QueuedAnalyticsSink : IAnalyticsSink, IDisposable
    {
        #region PendingEvent

        private readonly struct PendingEvent : IDisposable
        {
            private readonly List<KeyValuePair<string, object>> _parameters;

            public PendingEvent(string eventName, IEnumerable<KeyValuePair<string, object>> parameters)
            {
                EventName = eventName ?? string.Empty;

                _parameters = ListPool<KeyValuePair<string, object>>.Get();
                if (parameters is not null)
                    _parameters.AddRange(parameters);
            }

            public string EventName { get; }
            public IEnumerable<KeyValuePair<string, object>> Parameters => _parameters;

            public void Dispose()
            {
                ListPool<KeyValuePair<string, object>>.Release(_parameters);
            }
        }

        #endregion

        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly Queue<PendingEvent> _pendingEvents = new();

        private readonly IAnalyticsSink _innerSink;
        private readonly IAnalyticsLogger _logger;

        private bool _isDisposed;
        private bool _isReady;

        public QueuedAnalyticsSink
        (
            IAnalyticsSink innerSink,
            IAnalyticsLogger logger
        )
        {
            _innerSink = innerSink ?? throw new ArgumentNullException(nameof(innerSink));
            _logger = logger;

            if (_innerSink.IsInitialized)
            {
                _isReady = true;
            }
            else
            {
                FlushWhenReadyAsync(_cancellationTokenSource.Token).Forget();
            }
        }

        public bool IsInitialized => true;

        public void Send(string eventName, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            if (_isDisposed)
                return;

            if (_isReady == false)
            {
                _pendingEvents.Enqueue(new PendingEvent(eventName, parameters));
                return;
            }

            SendToInnerSink(eventName, parameters);
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();

            DropPendingEvents();
        }

        private async UniTaskVoid FlushWhenReadyAsync(CancellationToken cancellationToken)
        {
            var isCanceled = await UniTask.WaitUntil(_innerSink, static sink => sink.IsInitialized, cancellationToken: cancellationToken)
                .SuppressCancellationThrow();

            if (isCanceled || _isDisposed)
                return;

            while (_pendingEvents.TryDequeue(out var pendingEvent))
            {
                using (pendingEvent)
                {
                    SendToInnerSink(pendingEvent.EventName, pendingEvent.Parameters);
                }
            }

            _isReady = true;
        }

        private void SendToInnerSink(string eventName, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            try
            {
                _innerSink.Send(eventName, parameters);
            }
            catch (Exception exception)
            {
                if (_logger.IsEnabledError())
                    _logger.Error($"Error sending event {eventName}", exception, context: this);
            }
        }

        private void DropPendingEvents()
        {
            var droppedCount = 0;
            while (_pendingEvents.TryDequeue(out var pendingEvent))
            {
                pendingEvent.Dispose();
                droppedCount++;
            }

            if (droppedCount > 0 && _logger.IsEnabledWarning())
                _logger.Warning($"Dropped {droppedCount} pending events", context: this);
        }
    }
}