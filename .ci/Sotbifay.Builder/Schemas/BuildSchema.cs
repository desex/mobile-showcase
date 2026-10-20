using System.Text.Json.Serialization;

namespace Schemas;

public record BuildSchema(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("version")] string Version,
    [property: JsonPropertyName("version_code")] long VersionCode,
    [property: JsonPropertyName("checksum")] string Checksum,
    [property: JsonPropertyName("build_url")] string Url,
    [property: JsonPropertyName("is_active")] bool IsActive,
    [property: JsonPropertyName("is_force_build")] bool IsForceBuild,
    [property: JsonPropertyName("created_at")] string CreatedAt
);