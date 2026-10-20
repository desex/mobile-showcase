using System.Collections.Generic;
using UnityEngine.Pool;

namespace Sotbify.MobileApp.Common.Collections
{
    public class QueuePool<T>
    {
        private static readonly ObjectPool<Queue<T>> _pool = new(() => new Queue<T>(), actionOnRelease: l => l.Clear());
        
        public static Queue<T> Get() => _pool.Get();
        
        public static PooledObject<Queue<T>> Get(out Queue<T> value) => 
            _pool.Get(out value);

        public static void Release(Queue<T> toRelease) => 
            _pool.Release(toRelease);
    }
}