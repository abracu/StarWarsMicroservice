using System.Text.Json.Serialization;

namespace StarWars.ConsoleClient;

public record Character(
    [property: JsonPropertyName("name")] string Name, 
    [property: JsonPropertyName("gender")] string Gender, 
    [property: JsonPropertyName("birth_year")] string BirthYear, 
    [property: JsonPropertyName("url")] string Url
);

public record Favorite(
    [property: JsonPropertyName("name")] string Name, 
    [property: JsonPropertyName("url")] string Url, 
    [property: JsonPropertyName("addedAt")] DateTime AddedAt
);

public record ApiLog(
    [property: JsonPropertyName("method")] string HttpMethod, 
    [property: JsonPropertyName("endpoint")] string Endpoint, 
    [property: JsonPropertyName("statusCode")] int StatusCode, 
    [property: JsonPropertyName("durationMs")] long DurationMs, 
    [property: JsonPropertyName("date")] DateTime Timestamp
);