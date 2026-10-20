using System;
using System.Text.Json.Serialization;

namespace Models;

public readonly record struct BuildReport(
    Guid BuildId,
    
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    BuildType BuildType,
    
    string ApplicationName,
    string ApplicationVersion,
    
    long StartBuildTimestamp,
    long FinishBuildTimestamp,
    
    BuildOptions BuildOptions,
    string Error
);