using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Pool;

namespace Sotbify.MobileApp.Utils.Extensions
{
    public static class EnumerableExtensions
    {
        public static void AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> range)
        {
            foreach (var obj in range)
                hashSet.Add(obj);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDisposable ToPooledDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> enumerable, out Dictionary<TKey, TValue> dictionary)
        {
            if (enumerable is null)
                throw new ArgumentNullException(nameof(enumerable));
            
            var disposable = DictionaryPool<TKey, TValue>.Get(out dictionary);
            foreach (var (key, value) in enumerable) 
                dictionary[key] = value;

            return disposable;
        }
    }
}