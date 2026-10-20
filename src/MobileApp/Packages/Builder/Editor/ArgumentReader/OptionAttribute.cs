using System;

namespace Sotbifay.Builder.Editor
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class OptionAttribute : Attribute
    {
        public string ArgumentName { get; }

        public OptionAttribute(string argumentName)
        {
            ArgumentName = argumentName ?? throw new ArgumentNullException(nameof(argumentName));
        }
    }
}