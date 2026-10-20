using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine.Pool;

namespace Sotbify.MobileApp.Common.Collections
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> AsEnumerable<T>(this T value)
        {
            yield return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> enumerable)
        {
            return enumerable is not null 
                ? enumerable.ToDictionary(pair => pair.Key, pair => pair.Value) 
                : throw new ArgumentNullException(nameof(enumerable));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<Tuple<TKey, TValue>> enumerable)
        {
            return enumerable is not null
                ? enumerable.ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2)
                : throw new ArgumentNullException(nameof(enumerable));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<ValueTuple<TKey, TValue>> enumerable)
        {
            return enumerable is not null
                ? enumerable.ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2)
                : throw new ArgumentNullException(nameof(enumerable));
        }
        
        // Спи + жена(о) с исходиников .net 6 
        public static IEnumerable<IEnumerable<TSource>> Chunk<TSource>(this IEnumerable<TSource> source, int size)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            if (size < 1)
                throw new ArgumentOutOfRangeException(nameof(size));

            using var e = source.GetEnumerator();
            while (e.MoveNext())
            {
                var chunk = new TSource[size];
                chunk[0] = e.Current;

                var i = 1;
                for (; i < chunk.Length && e.MoveNext(); i++)
                    chunk[i] = e.Current;

                if (i == chunk.Length)
                {
                    yield return chunk;
                    continue;
                }

                Array.Resize(ref chunk, i);
                yield return chunk;
                yield break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<TItem> SafeToList<TItem>(this IEnumerable<TItem> enumerable)
        {
            if (enumerable is null)
                throw new ArgumentNullException(nameof(enumerable));

            return enumerable as List<TItem> ?? enumerable.ToList();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IReadOnlyList<TItem> SafeToReadOnlyList<TItem>(this IEnumerable<TItem> enumerable)
        {
            if (enumerable is null)
                throw new ArgumentNullException(nameof(enumerable));

            return enumerable as IReadOnlyList<TItem> ?? enumerable.ToList();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDisposable ToPooledList<TItem>(this IEnumerable<TItem> enumerable, out List<TItem> list)
        {
            if (enumerable is null)
                throw new ArgumentNullException(nameof(enumerable));

            var disposable = ListPool<TItem>.Get(out list);
            list.AddRange(enumerable);

            return disposable;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDisposable ToPooledHashSet<TItem>(this IEnumerable<TItem> enumerable, out HashSet<TItem> set)
        {
            if (enumerable is null)
                throw new ArgumentNullException(nameof(enumerable));

            var disposable = HashSetPool<TItem>.Get(out set);
            set.AddRange(enumerable);

            return disposable;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDisposable ToPooledDictionary<TKey, TValue>(this IEnumerable<Tuple<TKey, TValue>> enumerable, out Dictionary<TKey, TValue> dictionary)
        {
            if (enumerable is null)
                throw new ArgumentNullException(nameof(enumerable));

            var disposable = DictionaryPool<TKey, TValue>.Get(out dictionary);
            foreach (var (key, value) in enumerable) 
                dictionary[key] = value;

            return disposable;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDisposable ToPooledDictionary<TKey, TValue>(this IEnumerable<ValueTuple<TKey, TValue>> enumerable, out Dictionary<TKey, TValue> dictionary)
        {
            if (enumerable is null)
                throw new ArgumentNullException(nameof(enumerable));

            var disposable = DictionaryPool<TKey, TValue>.Get(out dictionary);
            foreach (var (key, value) in enumerable) 
                dictionary[key] = value;

            return disposable;
        }
        
        //<summary>Finds the index of the first occurrence of an item in an enumerable.</summary>
        ///<param name="enumerable">The enumerable to search.</param>
        ///<param name="item">The item to find.</param>
        ///<returns>The index of the first matching item, or -1 if the item was not found.</returns>
        public static int IndexOf<TItem>(this IEnumerable<TItem> enumerable, TItem item)
        {
            var comparer = EqualityComparer<TItem>.Default;
            return enumerable.IndexOf(i => comparer.Equals(item, i));
        }
        
        ///<summary>Finds the index of the first item matching an expression in an enumerable.</summary>
        ///<param name="enumerable">The enumerable to search.</param>
        ///<param name="predicate">The expression to test the items against.</param>
        ///<returns>The index of the first matching item, or -1 if no items match.</returns>
        public static int IndexOf<TItem>(this IEnumerable<TItem> enumerable, Predicate<TItem> predicate)
        {
            if (enumerable is null)
                throw new ArgumentNullException(nameof(enumerable));

            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));
            
            var index = 0;
            foreach (var item in enumerable)
            {
                if (predicate(item))
                    return index;
                
                index++;
            }
            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForEach<TItem>(this IEnumerable<TItem> enumerable, Action<TItem> action)
        {
            if (enumerable is null)
                throw new ArgumentNullException(nameof(enumerable));

            if (action is null)
                throw new ArgumentNullException(nameof(action));

            foreach (var item in enumerable)
                action(item);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForEach<TItem, TState>(this IEnumerable<TItem> enumerable, TState state, Action<TItem, TState> action)
        {
            if (enumerable is null)
                throw new ArgumentNullException(nameof(enumerable));

            if (action is null)
                throw new ArgumentNullException(nameof(action));

            foreach (var item in enumerable)
                action(item, state);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddRange<TItem>(this ISet<TItem> set, IEnumerable<TItem> range)
        {
            if (set is null)
                throw new ArgumentNullException(nameof(set));

            foreach (var item in range)
                set.Add(item);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddRange<T>(this Queue<T> queue, IEnumerable<T> enumerable)
        {
            foreach (var item in enumerable)
                queue.Enqueue(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsAny<TItem>(this ISet<TItem> set, IEnumerable<TItem> items)
        {
            if (set is null)
                throw new ArgumentNullException(nameof(set));
            
            if (items is null)
                throw new ArgumentNullException(nameof(items));
            
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var item in items)
            {
                if (set.Contains(item))
                    return true;
            }

            return false;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsAll<TItem>(this ISet<TItem> set, IEnumerable<TItem> items)
        {
            if (set is null)
                throw new ArgumentNullException(nameof(set));
            
            if (items is null)
                throw new ArgumentNullException(nameof(items));
            
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var item in items)
            {
                if (set.Contains(item) == false)
                    return false;
            }

            return true;
        }
    }
}