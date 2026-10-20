using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UState
{
    internal sealed class Transition<TPayload> : Transition
    {
        private readonly TPayload _payload;
        private readonly Func<TPayload, CancellationToken, UniTask> _executeTaskFactory;
        private readonly Func<TPayload, CancellationToken, UniTask> _exitTaskFactory;

        public Transition
        (
            TransitionRequest request,
            TPayload payload,
            Func<TPayload, CancellationToken, UniTask> executeTaskFactory,
            Func<TPayload, CancellationToken, UniTask> exitTaskFactory,
            bool shouldBeAddedToHistory,
            bool fromHistory
        ) : base(request, shouldBeAddedToHistory, fromHistory)
        {
            _payload = payload;
            _executeTaskFactory = executeTaskFactory;
            _exitTaskFactory = exitTaskFactory;
        }

        public override Transition Clone(TransitionRequest transitionRequest, bool fromHistory = false) => 
            new Transition<TPayload>(transitionRequest, _payload, _executeTaskFactory, _exitTaskFactory, ShouldBeAddedToHistory, fromHistory);

        protected override UniTask ExecuteInternalAsync(CancellationToken cancellationToken) =>
            _executeTaskFactory.Invoke(_payload, cancellationToken);

        protected override UniTask ExitInternalAsync(CancellationToken cancellationToken) =>
            _exitTaskFactory.Invoke(_payload, cancellationToken);
    }

    internal abstract class Transition
    {
        protected Transition(TransitionRequest request, bool shouldBeAddedToHistory, bool fromHistory)
        {
            Request = request;
            ShouldBeAddedToHistory = shouldBeAddedToHistory;
            FromHistory = fromHistory;
        }

        public bool ShouldBeAddedToHistory { get; }
        public bool FromHistory { get; }
        public TransitionRequest Request { get; }

        public abstract Transition Clone(TransitionRequest transitionRequest, bool fromHistory = false);
        
        public async UniTask ExecuteAsync(CancellationToken cancellationToken)
        {
            Request.MakeEntered();
            await ExecuteInternalAsync(cancellationToken);
            Request.MakeExecuted();
        }

        public async UniTask ExitAsync(CancellationToken cancellationToken)
        {
            await ExitInternalAsync(cancellationToken);
            Request.MakeItExit();
        }

        protected abstract UniTask ExecuteInternalAsync(CancellationToken cancellationToken);
        protected abstract UniTask ExitInternalAsync(CancellationToken cancellationToken);
    }
}