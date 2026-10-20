namespace UState
{
    public interface IStateHistoryPolicy
    {
        bool ShouldBeAddedToHistory { get; }
    }
}