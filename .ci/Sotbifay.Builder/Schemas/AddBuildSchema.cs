using System.Text.Json.Serialization;

namespace Schemas;

public record AddBuildSchema(
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("version")] string Version,
    [property: JsonPropertyName("version_code")] long VersionCode,
    [property: JsonPropertyName("checksum")] string Checksum,
    [property: JsonPropertyName("is_force_build")] bool IsForceBuild
);