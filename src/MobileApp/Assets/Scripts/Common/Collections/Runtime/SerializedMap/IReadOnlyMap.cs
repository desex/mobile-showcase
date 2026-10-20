using System.Collections.Generic;

namespace Sotbify.MobileApp.Common.Collections.SerializedMap
{
    public interface IReadOnlyMap<TKey1, TKey2>
    {
        TKey2 this[TKey1 key] { get; }
        TKey1 this[TKey2 key] { get; }
        
        IReadOnlyDictionary<TKey1, TKey2> Forward { get; }
        IReadOnlyDictionary<TKey2, TKey1> Reverse { get; }
        
        bool TryGet(TKey1 key1, out TKey2 key2);
        bool TryGet(TKey2 key1, out TKey1 key2);
    }
}