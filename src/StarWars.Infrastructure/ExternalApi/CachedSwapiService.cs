using Microsoft.Extensions.Caching.Memory;
using StarWars.Application.DTOs;
using StarWars.Application.Interfaces;

namespace StarWars.Infrastructure.ExternalApi;

public class CachedSwapiService : ISwapiService
{
    private readonly ISwapiService _innerService;
    private readonly IMemoryCache _cache;

    public CachedSwapiService(ISwapiService innerService, IMemoryCache cache)
    {
        _innerService = innerService;
        _cache = cache;
    }

    public async Task<IEnumerable<CharacterDto>> GetPeopleAsync(int page = 1)
    {
        string cacheKey = $"people_page_{page}";
        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
            return await _innerService.GetPeopleAsync(page);
        }) ?? Enumerable.Empty<CharacterDto>();
    }

    public async Task<IEnumerable<CharacterDto>> SearchPeopleAsync(string name)
    {
        string cacheKey = $"search_people_{name.ToLower()}";
        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return await _innerService.SearchPeopleAsync(name);
        }) ?? Enumerable.Empty<CharacterDto>();
    }
}