using System;
using System.Runtime.CompilerServices;

namespace UState
{
    public static class StateResolverExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TState Resolve<TState>(this IStateResolver stateResolver) => 
            (TState)stateResolver.Resolve(typeof(TState));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryResolve(this IStateResolver stateResolver, Type type, out object instance)
        {
            instance = stateResolver.Resolve(type);
            return ReferenceEquals(instance, null) == false;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryResolve<TState>(this IStateResolver stateResolver, out TState instance)
        {
            instance = stateResolver.Resolve<TState>();
            return ReferenceEquals(instance, null) == false;
        }
    }
}