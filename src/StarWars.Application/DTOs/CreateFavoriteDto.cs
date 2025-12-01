using System.Text.Json.Serialization;

namespace StarWars.Application.DTOs;

public class CreateFavoriteDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}