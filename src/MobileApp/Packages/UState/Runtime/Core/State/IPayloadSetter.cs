namespace UState
{
    public interface IPayloadSetter<in TPayload>
    {
        void SetPayload(TPayload payload);
    }
}