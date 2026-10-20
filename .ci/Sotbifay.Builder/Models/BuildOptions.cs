#nullable enable
using System.Text.Json.Serialization;

namespace Models;

public readonly record struct BuildOptions(
    [property: JsonPropertyName("scenes")]
    string[]? Scenes,
    
    [property: JsonPropertyName("locationPathName")]
    string? LocationPathName,
    
    [property: JsonPropertyName("assetBundleManifestPath")]
    string? AssetBundleManifestPath,
    
    [property: JsonPropertyName("targetGroup")]
    string? TargetGroup,
    
    [property: JsonPropertyName("target")]
    string? Target,
    
    [property: JsonPropertyName("subtarget")]
    int Subtarget,
    
    [property: JsonPropertyName("options")]
    string? Options,
    
    [property: JsonPropertyName("extraScriptingDefines")]
    string[]? ExtraScriptingDefines
);