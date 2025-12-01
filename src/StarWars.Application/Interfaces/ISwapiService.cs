using StarWars.Application.DTOs;

namespace StarWars.Application.Interfaces;

public interface ISwapiService
{
    // Retrieve a paginated list of characters (SWAPI uses page-based pagination)
    Task<IEnumerable<CharacterDto>> GetPeopleAsync(int page = 1);
    
    // Search for characters by name
    Task<IEnumerable<CharacterDto>> SearchPeopleAsync(string name);
}