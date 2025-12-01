using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using StarWars.Application.DTOs;
using StarWars.Application.Interfaces;

namespace StarWars.Infrastructure.ExternalApi;

public class CachedSwapiService : ISwapiService
{
    private readonly ISwapiService _innerService;
    private readonly IDistributedCache _cache; // Cambiamos IMemoryCache por IDistributedCache

    public CachedSwapiService(ISwapiService innerService, IDistributedCache cache)
    {
        _innerService = innerService;
        _cache = cache;
    }

    public async Task<IEnumerable<CharacterDto>> GetPeopleAsync(int page = 1)
    {
        string cacheKey = $"people_page_{page}";

        // 1. Intentar obtener string de Redis
        var cachedData = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedData))
        {
            // HIT: Deserializamos el JSON de vuelta a objetos C#
            return JsonSerializer.Deserialize<IEnumerable<CharacterDto>>(cachedData) 
                   ?? Enumerable.Empty<CharacterDto>();
        }

        // MISS: Llamamos al servicio real
        var result = await _innerService.GetPeopleAsync(page);

        // 2. Guardar en Redis (Serializando a JSON)
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };

        var serializedData = JsonSerializer.Serialize(result);
        await _cache.SetStringAsync(cacheKey, serializedData, options);

        return result;
    }

    public async Task<IEnumerable<CharacterDto>> SearchPeopleAsync(string name)
    {
        string cacheKey = $"search_people_{name.ToLower()}";

        var cachedData = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedData))
        {
            return JsonSerializer.Deserialize<IEnumerable<CharacterDto>>(cachedData) 
                   ?? Enumerable.Empty<CharacterDto>();
        }

        var result = await _innerService.SearchPeopleAsync(name);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };

        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), options, CancellationToken.None);

        return result;
    }
}