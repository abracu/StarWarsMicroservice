using System.Text.Json.Serialization;

namespace StarWars.Application.DTOs;

public class CharacterDto
{
    // [NOTE]: Use JsonPropertyName to automatically map JSON snake_case (e.g., 'birth_year') 
    // to C# PascalCase conventions.
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("gender")]
    public string Gender { get; set; } = string.Empty;

    [JsonPropertyName("birth_year")]
    public string BirthYear { get; set; } = string.Empty;

    [JsonPropertyName("height")]
    public string Height { get; set; } = string.Empty;

    // The URL is critical as it serves as the unique identifier within the SWAPI ecosystem
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}