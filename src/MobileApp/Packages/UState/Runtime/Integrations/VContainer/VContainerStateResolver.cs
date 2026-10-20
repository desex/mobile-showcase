#if VCONTAINER_SUPPORT

using System;
using VContainer;

namespace UState.VContainer
{
    public sealed class VContainerStateResolver : IStateResolver
    {
        private readonly IObjectResolver _resolver;

        public VContainerStateResolver(IObjectResolver resolver)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        public object Resolve(Type type)
        {
            return _resolver.Resolve(type);
        }
    }
}

#endif
