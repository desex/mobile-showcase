using System;

namespace UState
{
    internal sealed class TransitionRequest : ITransitionRequest
    {
        public TransitionRequest(Type stateType)
        {
            StateType = stateType ?? throw new ArgumentNullException(nameof(stateType));
        }

        public Type StateType { get; }
        public bool InState { get; private set; } = false;
        public bool IsExecuted { get; private set; } = false;
        public bool IsCancelled { get; private set; } = false;

        public event Action Entered;
        public event Action Executed;
        public event Action Exited;
        public event Action Canceled;

        public void MakeEntered()
        {
            InState = true;
            Entered?.Invoke();
        }
        
        public void MakeExecuted()
        {
            IsExecuted = true;
            Executed?.Invoke();
        }

        public void MakeItExit()
        {
            InState = false;
            Exited?.Invoke();
        }

        public void MakeCancelled()
        {
            IsExecuted = false;
            IsCancelled = true;
            Canceled?.Invoke();
        }
    }
}