using System.ComponentModel;
using Nuke.Common.Tooling;

namespace Types;

[TypeConverter(typeof(TypeConverter<UnityBuildType>))]
public sealed class UnityBuildType : Enumeration
{
    public static readonly UnityBuildType Debug = nameof(Debug);
    public static readonly UnityBuildType Release = nameof(Release);
    
    public static implicit operator UnityBuildType(string value) => new() { Value = value };
}