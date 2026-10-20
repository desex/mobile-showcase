using System;
using System.Threading;
using Best.ServerSentEvents;
using Cysharp.Threading.Tasks;
using R3;
using Sotbifay.MobileApp.Logger;

namespace Sotbify.MobileApp.Server.SSE
{
    public sealed class SSEFacade : ISSEFacade, IDisposable
    {
        private readonly Subject<SSEFacadeWrapper> _openCommand = new();
        private readonly Subject<SSEFacadeWrapper> _reconnectCommand = new();
        private readonly Subject<SSEFacadeWrapper> _closeCommand = new();
        private readonly Subject<SSEFacadeWrapper> _errorCommand = new();
        private readonly Subject<SSEFacadeWrapper> _retryCommand = new();
        private readonly Subject<SSEFacadeWrapper> _messageCommand = new();
        private readonly Subject<SSEFacadeWrapper> _commentCommand = new();
        private readonly Subject<SSEFacadeWrapper> _stateChangeCommand = new();
        
        private readonly Uri _uri;
        private readonly IAuthenticatorProvider _authenticatorProvider;
        private readonly ILogger _logger;

        private readonly TimeSpan _connectionTimeout = TimeSpan.FromSeconds(10);
        private readonly TimeSpan _reconnectDelay = TimeSpan.FromSeconds(10);

        private EventSource _eventSource;
        private CancellationTokenSource _cancellationTokenSource;
        private DateTime _lastPingTime;
        private bool _isReconnecting;

        public bool IsOpened => _eventSource is { IsClosed: false };
        public bool AutoReconnect { get; set; } = true;

        public SSEFacade
        (
            Uri uri,
            IAuthenticatorProvider authenticatorProvider,
            ILogger logger
        )
        {
            _uri = uri ?? throw new ArgumentNullException(nameof(uri));
            _authenticatorProvider = authenticatorProvider ?? throw new ArgumentNullException(nameof(authenticatorProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public Observable<SSEFacadeWrapper> OpenObservable => _openCommand;
        public Observable<SSEFacadeWrapper> ReconnectObservable => _reconnectCommand;
        public Observable<SSEFacadeWrapper> CloseObservable => _closeCommand;
        public Observable<SSEFacadeWrapper> ErrorObservable => _errorCommand;
        public Observable<SSEFacadeWrapper> RetryObservable => _retryCommand;
        public Observable<SSEFacadeWrapper> MessageObservable => _messageCommand;
        public Observable<SSEFacadeWrapper> CommentObservable => _commentCommand;
        public Observable<SSEFacadeWrapper> StateChangeObservable => _stateChangeCommand;

        public void Start()
        {
            if (IsOpened)
            {
                throw new InvalidOperationException("Sse is already opened!");
            }

            _eventSource = new EventSource(_uri);

            var authenticator = _authenticatorProvider.GetAuthenticator();
            if (authenticator != null)
            {
                _eventSource.InternalRequest.Authenticator = authenticator;
            }

            _eventSource.OnOpen += OnEventSourceOpened;
            _eventSource.OnClosed += OnEventSourceClosed;
            _eventSource.OnError += OnEventSourceError;
            _eventSource.OnMessage += OnEventSourceMessage;
            _eventSource.OnComment += OnEventSourceComment;
            _eventSource.OnStateChanged += OnEventSourceStateChanged;

            _eventSource.Open();
        }

        public void Stop()
        {
            if (IsOpened == false)
            {
                return;
            }

            DisposeCurrentEventSource();
            DisposeCancellation();
        }
        
        public void Dispose()
        {
            Stop();
            DisposeCancellation();
            
            _openCommand.Dispose();
            _reconnectCommand.Dispose();
            _closeCommand.Dispose();
            _errorCommand.Dispose();
            _retryCommand.Dispose();
            _messageCommand.Dispose();
            _commentCommand.Dispose();
            _stateChangeCommand.Dispose();
        }

        private void RunDelayedReconnection()
        {
            DisposeCancellation();
            _cancellationTokenSource = new CancellationTokenSource();
            TryReconnect(_cancellationTokenSource.Token).Forget();
        }

        private async UniTaskVoid CheckConnection(CancellationToken cancellationToken)
        {
            while (cancellationToken.IsCancellationRequested == false)
            {
                await UniTask.Delay(_connectionTimeout, cancellationToken: cancellationToken);

                var diff = DateTime.UtcNow - _lastPingTime;
                if (diff <= _connectionTimeout) 
                    continue;
                
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.Warning($"Sse Connection to {_uri} timed out!", context: this);
                Stop();
                RunDelayedReconnection();
                break;
            }
        }

        private async UniTaskVoid TryReconnect(CancellationToken cancellationToken)
        {
            _isReconnecting = true;
            await UniTask.Delay(_reconnectDelay, cancellationToken: cancellationToken);
            
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.Debug($"Try reconnect sse to {_uri}", context: this);
            
            Start();
        }

        private void UpdatePingTime()
        {
            _lastPingTime = DateTime.UtcNow;
        }

        private void DisposeCurrentEventSource()
        {
            if (_eventSource == null)
            {
                return;
            }

            _eventSource.OnOpen -= OnEventSourceOpened;
            _eventSource.OnClosed -= OnEventSourceClosed;
            _eventSource.OnError -= OnEventSourceError;
            _eventSource.OnMessage -= OnEventSourceMessage;
            _eventSource.OnComment -= OnEventSourceComment;
            _eventSource.OnStateChanged -= OnEventSourceStateChanged;

            _eventSource.Close();
            _eventSource.Dispose();
            _eventSource = null;
        }

        private void DisposeCancellation()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }


        #region EventSourceEvents

        private void OnEventSourceMessage(EventSource eventSource, Message message)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.Debug($"Message - {message.Data} ({_uri})", context: this);

            UpdatePingTime();
            _messageCommand?.OnNext(new SSEFacadeWrapper
            {
                EventSource = this,
                Message = message.Data
            });
        }

        private void OnEventSourceComment(EventSource eventSource, string comment)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.Debug($"Comment - {comment} ({_uri})", context: this);

            UpdatePingTime();
            _commentCommand?.OnNext(new SSEFacadeWrapper
            {
                EventSource = this,
                Message = comment
            });
        }

        private void OnEventSourceStateChanged(EventSource eventSource, States oldState, States newState)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.Debug($"State changed [{oldState}]->[{newState}] ({_uri})", context: this);
            _stateChangeCommand?.OnNext(new SSEFacadeWrapper
            {
                EventSource = this,
                Message = string.Empty
            });
        }

        private void OnEventSourceError(EventSource eventSource, string error)
        {
            if (_logger.IsEnabled(LogLevel.Error))
                _logger.Error($"Error - {error} ({_uri})", context: this);
            _errorCommand?.OnNext(new SSEFacadeWrapper
            {
                EventSource = this,
                Message = error
            });
        }

        private void OnEventSourceOpened(EventSource eventSource)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.Debug($"Connection opened ({_uri})", context: this);
            DisposeCancellation();
            _cancellationTokenSource = new();

            if (AutoReconnect)
            {
                CheckConnection(_cancellationTokenSource.Token).Forget();
            }

            _openCommand?.OnNext(new SSEFacadeWrapper
            {
                EventSource = this,
                Message = string.Empty
            });

            if (!_isReconnecting) 
                return;
            
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.Debug($"Reconnection succeed ({_uri})", context: this);
            _isReconnecting = false;
            _reconnectCommand?.OnNext(new SSEFacadeWrapper
            {
                EventSource = this,
                Message = string.Empty
            });
        }

        private void OnEventSourceClosed(EventSource eventSource)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.Debug($"Connection closed ({_uri})", context: this);
            _closeCommand?.OnNext(new SSEFacadeWrapper
            {
                EventSource = this,
                Message = string.Empty
            });

            if (AutoReconnect)
            {
                RunDelayedReconnection();
            }
        }

        #endregion
    }
}