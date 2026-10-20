using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace UState
{
    public interface IStateMachine<in TStateMachine> where TStateMachine : class, IStateMachine<TStateMachine>
    {
        ITransitionRequest CurrentRequest { get; }

        event Action CurrentRequestChanged;

        bool TryRequestTransition<TState>(out ITransitionRequest transitionRequest, bool force = false) 
            where TState : class, IState<TStateMachine, EmptyPayload>;

        ITransitionRequest RequestTransition<TState>(bool force = false)
            where TState : class, IState<TStateMachine, EmptyPayload>;

        bool TryRequestTransition<TState, TPayload>([CanBeNull] TPayload payload, out ITransitionRequest transitionRequest, bool force = false) 
            where TState : class, IState<TStateMachine, TPayload>;

        ITransitionRequest RequestTransition<TState, TPayload>([CanBeNull] TPayload payload, bool force = false)
            where TState : class, IState<TStateMachine, TPayload>;

        bool TryUndo(out ITransitionRequest transitionRequest, bool force = false);

        ITransitionRequest Undo(bool force = false);

        UniTask ExecuteAsync(CancellationToken cancellationToken = default);
    }
}