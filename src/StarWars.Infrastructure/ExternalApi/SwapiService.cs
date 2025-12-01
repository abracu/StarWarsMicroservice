using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using StarWars.Application.DTOs;
using StarWars.Application.Interfaces;

namespace StarWars.Infrastructure.ExternalApi;

public class SwapiService : ISwapiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SwapiService> _logger;

    // Inject the HttpClient configured via IHttpClientFactory
    public SwapiService(HttpClient httpClient, ILogger<SwapiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IEnumerable<CharacterDto>> GetPeopleAsync(int page = 1)
    {
        // SWAPI endpoint: /people/?page=X
        var response = await _httpClient.GetAsync($"people/?page={page}");
        return await ProcessResponse(response);
    }

    public async Task<IEnumerable<CharacterDto>> SearchPeopleAsync(string name)
    {
        // SWAPI endpoint: /people/?search=name
        var response = await _httpClient.GetAsync($"people/?search={name}");
        return await ProcessResponse(response);
    }

    private async Task<IEnumerable<CharacterDto>> ProcessResponse(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Error fetching data from SWAPI: {response.StatusCode}");
            return Enumerable.Empty<CharacterDto>(); // Or throw a custom exception
        }

        var content = await response.Content.ReadAsStringAsync();
        
        // Deserialize using the wrapper class (defined below)
        var result = JsonSerializer.Deserialize<SwapiResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return result?.Results ?? Enumerable.Empty<CharacterDto>();
    }

    // Helper class to map the SWAPI JSON structure
    // (JsonPropertyName attributes in DTOs handle the snake_case mapping)
    private class SwapiResponse
    {
        public List<CharacterDto> Results { get; set; } = new();
    }
}