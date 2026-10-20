using System.Collections.Generic;
using UnityEngine.Pool;

namespace Sotbify.MobileApp.Common.Collections
{
    public class StackPool<T>
    {
        private static readonly ObjectPool<Stack<T>> _pool = new(() => new Stack<T>(), actionOnRelease: l => l.Clear());
        
        public static Stack<T> Get() => _pool.Get();
        
        public static PooledObject<Stack<T>> Get(out Stack<T> value) => 
            _pool.Get(out value);

        public static void Release(Stack<T> toRelease) => 
            _pool.Release(toRelease);
    }
}