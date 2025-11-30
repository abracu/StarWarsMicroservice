using StarWars.Application.DTOs;

namespace StarWars.Application.Interfaces;

public interface ISwapiService
{
    Task<IEnumerable<CharacterDto>> GetPeopleAsync(int page = 1);
    Task<IEnumerable<CharacterDto>> SearchPeopleAsync(string name);
}