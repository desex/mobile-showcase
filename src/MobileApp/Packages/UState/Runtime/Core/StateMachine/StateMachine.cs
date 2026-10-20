using System;
using System.Collections.Concurrent;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UState
{
    public abstract class StateMachine<TStateMachine> : IStateMachine<TStateMachine>, IDisposable
        where TStateMachine : class, IStateMachine<TStateMachine>
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly ConcurrentQueue<Transition> _pendingTransitions = new();
        private readonly SemaphoreSlim _executeSemaphore = new(1, 1);

        private readonly IStateResolver _stateResolver;
        private readonly DropOutStack<Transition> _history;

        private Transition _currentTransition;
        private CancellationTokenSource _currentTransitionTokenSource;

        protected StateMachine(IStateResolver stateResolver, int historyCapacity = 20)
        {
            _stateResolver = stateResolver ?? throw new ArgumentNullException(nameof(stateResolver));
            _history = new DropOutStack<Transition>(historyCapacity);
        }

        public ITransitionRequest CurrentRequest => _currentTransition?.Request;

        public event Action CurrentRequestChanged;

        public bool TryRequestTransition<TState>(out ITransitionRequest transitionRequest, bool force = false)
            where TState : class, IState<TStateMachine, EmptyPayload> =>
            TryRequestTransition<TState, EmptyPayload>(EmptyPayload.Default, out transitionRequest, force);

        public ITransitionRequest RequestTransition<TState>(bool force = false)
            where TState : class, IState<TStateMachine, EmptyPayload> =>
            RequestTransition<TState, EmptyPayload>(EmptyPayload.Default, force);

        public bool TryRequestTransition<TState, TPayload>(TPayload payload, out ITransitionRequest transitionRequest, bool force = false)
            where TState : class, IState<TStateMachine, TPayload>
        {
            var currentStateType = _currentTransition?.Request?.StateType;
            transitionRequest = null;

            if (currentStateType == typeof(TState))
                return false;

            transitionRequest = RequestTransition<TState, TPayload>(payload, force);
            return true;
        }

        public ITransitionRequest RequestTransition<TState, TPayload>(TPayload payload, bool force = false)
            where TState : class, IState<TStateMachine, TPayload>
        {
            if (_stateResolver.TryResolve<TState>(out var state) == false)
                throw new StateNotFoundException(typeof(TState).Name);

            if (force)
                CancelAllTransitions();

            var transitionRequest = new TransitionRequest(typeof(TState));
            var transition = new Transition<(TState State, TPayload Payload)>
            (
                transitionRequest,
                (state, payload),
                static (transitionPayload, cancellationToken) =>
                {
                    var state = transitionPayload.State;
                    state.SetPayload(transitionPayload.Payload);
                    return state.ExecuteAsync(cancellationToken);
                },
                static (transitionPayload, cancellationToken) =>
                {
                    var state = transitionPayload.State;
                    return state.ExitAsync(cancellationToken);
                },
                state.ShouldBeAddedToHistory,
                false
            );

            _pendingTransitions.Enqueue(transition);

            return transitionRequest;
        }

        public bool TryUndo(out ITransitionRequest transitionRequest, bool force = false)
        {
            if (_history.TryPop(out var transitionFromHistory) == false)
            {
                transitionRequest = null;
                return false;
            }

            if (force)
                CancelAllTransitions();

            var transitionRequestFromHistory = transitionFromHistory.Request;
            var newTransitionRequest = new TransitionRequest(transitionRequestFromHistory.StateType);
            var transition = transitionFromHistory.Clone(newTransitionRequest, true);
            _pendingTransitions.Enqueue(transition);

            transitionRequest = newTransitionRequest;
            return true;
        }

        public ITransitionRequest Undo(bool force = false)
        {
            if (_history.TryPop(out var transitionFromHistory) == false)
                throw new EmptyHistoryException(GetType().Name);

            if (force)
                CancelAllTransitions();

            var transitionRequestFromHistory = transitionFromHistory.Request;
            var newTransitionRequest = new TransitionRequest(transitionRequestFromHistory.StateType);
            var transition = transitionFromHistory.Clone(newTransitionRequest, true);
            _pendingTransitions.Enqueue(transition);

            return newTransitionRequest;
        }

        public async UniTask ExecuteAsync(CancellationToken cancellationToken = default)
        {
            using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);
            var acquired = await _executeSemaphore.WaitAsync(0, cancellationTokenSource.Token);
            if (acquired == false)
                throw new InvalidOperationException("Already running");

            try
            {
                while (cancellationTokenSource.IsCancellationRequested == false)
                {
                    if (_pendingTransitions.TryDequeue(out var transition) == false)
                    {
                        await UniTask.NextFrame(cancellationTokenSource.Token);
                        continue;
                    }

                    await HandleTransitionAsync(transition, cancellationTokenSource.Token);
                }
            }
            finally
            {
                try
                {
                    _executeSemaphore.Release();
                }
                catch (ObjectDisposedException)
                {
                }
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();

            CancelAllTransitions();
            _executeSemaphore.Dispose();

            OnDispose();
        }

        protected virtual void OnDispose()
        {
        }

        private void CancelAllTransitions()
        {
            _currentTransitionTokenSource?.Cancel();

            while (_pendingTransitions.TryDequeue(out var pendingTransition))
            {
                var request = pendingTransition.Request;
                request.MakeCancelled();
            }
        }

        private async UniTask HandleTransitionAsync(Transition transition, CancellationToken cancellationToken)
        {
            try
            {
                if (ReferenceEquals(_currentTransition, null) == false)
                {
                    await _currentTransition.ExitAsync(cancellationToken);

                    if (_currentTransition.ShouldBeAddedToHistory && transition.FromHistory == false)
                        _history.Push(_currentTransition);
                }
                
                _currentTransitionTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

                _currentTransition = transition;
                CurrentRequestChanged?.Invoke();
                await transition.ExecuteAsync(_currentTransitionTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                var request = transition.Request;
                request.MakeCancelled();
                // Ignore 🤡
            }
            finally
            {
                _currentTransitionTokenSource.Cancel();
                _currentTransitionTokenSource.Dispose();
                _currentTransitionTokenSource = null;
            }
        }
    }
}