using System.Threading;
using Cysharp.Threading.Tasks;

namespace UState
{
    public abstract class State<TStateMachine> : State<TStateMachine, EmptyPayload>
        where TStateMachine : class, IStateMachine<TStateMachine>
    {
        protected sealed override UniTask ExecuteAsync(EmptyPayload payload, CancellationToken cancellationToken) => 
            ExecuteAsync(cancellationToken);

        protected abstract UniTask ExecuteAsync(CancellationToken cancellationToken);
    }

    public abstract class State<TStateMachine, TPayload> : IState<TStateMachine, TPayload>
        where TStateMachine : class, IStateMachine<TStateMachine>
    {
        private TPayload _payload;
        
        public virtual bool ShouldBeAddedToHistory => true;
        
        UniTask IExecutableState.ExecuteAsync(CancellationToken cancellationToken) => 
            ExecuteAsync(_payload,  cancellationToken);
        
        void IPayloadSetter<TPayload>.SetPayload(TPayload payload) => 
            _payload = payload;

        UniTask IExitableState.ExitAsync(CancellationToken cancellationToken) => 
            ExitAsync(cancellationToken);

        protected abstract UniTask ExecuteAsync(TPayload payload, CancellationToken cancellationToken);
        
        protected virtual UniTask ExitAsync(CancellationToken cancellationToken) =>
            UniTask.CompletedTask;
    }
} 