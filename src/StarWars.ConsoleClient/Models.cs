using System.Text.Json.Serialization;

namespace StarWars.ConsoleClient;

// Usamos JsonPropertyName para asegurar que el cliente lea bien lo que manda la API
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