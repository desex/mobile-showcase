using System.Text.Json.Serialization;

namespace Schemas;

public record AuthenticationSchema(
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("password")] string Password
);