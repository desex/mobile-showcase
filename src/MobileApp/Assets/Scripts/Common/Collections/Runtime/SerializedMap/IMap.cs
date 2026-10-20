namespace Sotbify.MobileApp.Common.Collections.SerializedMap
{
    public interface IMap<TKey1, TKey2> : IReadOnlyMap<TKey1, TKey2>
    {
        void Add(TKey1 key1, TKey2 key2);
    }
}