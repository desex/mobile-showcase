namespace UState
{
    public interface IState<out TStateMachine, in TPayload> :
        IExecutableState,
        IPayloadSetter<TPayload>,
        IExitableState,
        IStateHistoryPolicy
        where TStateMachine : class, IStateMachine<TStateMachine>
    {
    }
}