using System.Text.Json.Serialization;

namespace Schemas;

public record AssignBuildFileSchema(
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("key")] string Key
);