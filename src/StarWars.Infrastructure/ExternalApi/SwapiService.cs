using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using StarWars.Application.DTOs;
using StarWars.Application.Interfaces;

namespace StarWars.Infrastructure.ExternalApi;

public class SwapiService : ISwapiService
{
    private readonly HttpClient _httpClient;

    public SwapiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<CharacterDto>> GetPeopleAsync(int page = 1)
    {
        var response = await _httpClient.GetAsync($"people/?page={page}");
        return await ProcessResponse(response);
    }

    public async Task<IEnumerable<CharacterDto>> SearchPeopleAsync(string name)
    {
        var response = await _httpClient.GetAsync($"people/?search={name}");
        return await ProcessResponse(response);
    }

    private async Task<IEnumerable<CharacterDto>> ProcessResponse(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode) return Enumerable.Empty<CharacterDto>();
        
        var content = await response.Content.ReadAsStringAsync();
        // Usamos una clase envoltorio temporal para deserializar el JSON de SWAPI
        var result = JsonSerializer.Deserialize<SwapiResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return result?.Results ?? Enumerable.Empty<CharacterDto>();
    }

    private class SwapiResponse
    {
        public List<CharacterDto> Results { get; set; } = new();
    }
}