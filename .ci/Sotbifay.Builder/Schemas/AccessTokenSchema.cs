using System.Text.Json.Serialization;

namespace Schemas;

public record AccessTokenSchema(
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("token_type")] string TokenType
);