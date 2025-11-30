using System.Text.Json.Serialization;

namespace StarWars.Application.DTOs;

public class CharacterDto
{
    // Usamos JsonPropertyName para mapear el snake_case de la API (birth_year)
    // al PascalCase de C# (BirthYear) automáticamente.
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("gender")]
    public string Gender { get; set; } = string.Empty;

    [JsonPropertyName("birth_year")]
    public string BirthYear { get; set; } = string.Empty;

    [JsonPropertyName("height")]
    public string Height { get; set; } = string.Empty;

    // La URL es vital porque actúa como ID único en SWAPI
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}