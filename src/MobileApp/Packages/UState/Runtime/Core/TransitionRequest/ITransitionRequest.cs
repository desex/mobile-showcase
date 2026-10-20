using System;

namespace UState
{
    public interface ITransitionRequest
    {
        Type StateType { get; }
        bool InState { get; }
        bool IsExecuted { get; }
        bool IsCancelled { get; }

        event Action Entered;
        event Action Executed;
        event Action Canceled;
        event Action Exited;
    }
}