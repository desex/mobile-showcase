using System;

namespace UState
{
    public interface IStateResolver
    {
        object Resolve(Type type);
    }
}