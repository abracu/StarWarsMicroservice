using Microsoft.AspNetCore.Mvc;
using StarWars.Application.Interfaces;

namespace StarWars.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CharactersController : ControllerBase
{
    private readonly ISwapiService _swapiService;

    public CharactersController(ISwapiService swapiService)
    {
        _swapiService = swapiService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int page = 1)
    {
        var characters = await _swapiService.GetPeopleAsync(page);
        return Ok(characters);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string name)
    {
        var characters = await _swapiService.SearchPeopleAsync(name);
        return Ok(characters);
    }
}