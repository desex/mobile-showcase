namespace Sotbifay.Analytics
{
    public interface IAnalyticsCommonParameter
    {
        string Name { get; }
        bool CanBeCached { get; }
        
        object GetValue();
    }
}