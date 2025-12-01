using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarWars.Application.DTOs;
using StarWars.Application.Interfaces;
using StarWars.Domain.Entities;
using StarWars.Infrastructure.Persistence;

namespace StarWars.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")] // Define que esta API habla JSON
public class CharactersController : ControllerBase
{
    private readonly ISwapiService _swapiService;
    private readonly StarWarsDbContext _context;

    public CharactersController(ISwapiService swapiService, StarWarsDbContext context)
    {
        _swapiService = swapiService;
        _context = context;
    }

    /// <summary>
    /// Retrieves a paginated list of characters from SWAPI.
    /// </summary>
    /// <remarks>
    /// This endpoint uses a **Distributed Cache (Redis)** strategy. 
    /// Repeated requests for the same page will be served instantly from the cache (TTL: 10 mins).
    /// </remarks>
    /// <param name="page">Page number to retrieve (starts at 1).</param>
    /// <returns>A list of Star Wars characters.</returns>
    /// <response code="200">Returns the list of characters.</response>
    /// <response code="500">If SWAPI is down or unreachable.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CharacterDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get([FromQuery] int page = 1)
    {
        var characters = await _swapiService.GetPeopleAsync(page);
        return Ok(characters);
    }

    /// <summary>
    /// Searches for characters by name via SWAPI.
    /// </summary>
    /// <remarks>
    /// Search results are also cached to optimize performance and reduce external API calls.
    /// </remarks>
    /// <param name="name">The name (or partial name) to search for.</param>
    /// <returns>A list of matching characters.</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<CharacterDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Search([FromQuery] string name)
    {
        var characters = await _swapiService.SearchPeopleAsync(name);
        return Ok(characters);
    }

    // ==========================================
    // Favorites Management
    // ==========================================

    /// <summary>
    /// Retrieves the list of favorite characters stored in the local database.
    /// </summary>
    /// <response code="200">Returns your saved favorites ordered by date added.</response>
    [HttpGet("favorites")]
    [ProducesResponseType(typeof(IEnumerable<FavoriteCharacter>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFavorites()
    {
        var favorites = await _context.FavoriteCharacters
            .OrderByDescending(f => f.AddedAt)
            .ToListAsync();
            
        return Ok(favorites);
    }

    /// <summary>
    /// Adds a character to the favorites list.
    /// </summary>
    /// <remarks>
    /// The URL serves as the unique identifier. Trying to add the same URL twice will result in a 409 Conflict.
    /// </remarks>
    /// <param name="dto">The character data (Name and URL).</param>
    /// <response code="201">Character successfully added.</response>
    /// <response code="409">Character already exists in favorites.</response>
    [HttpPost("favorites")]
    [ProducesResponseType(typeof(FavoriteCharacter), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddFavorite([FromBody] CreateFavoriteDto dto)
    {
        if (await _context.FavoriteCharacters.AnyAsync(f => f.Url == dto.Url))
        {
            return Conflict(new { message = "This character is already in your favorites." });
        }

        var favorite = new FavoriteCharacter
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Url = dto.Url,
            AddedAt = DateTime.UtcNow
        };

        _context.FavoriteCharacters.Add(favorite);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetFavorites), new { id = favorite.Id }, favorite);
    }

    /// <summary>
    /// Removes a character from favorites using their URL as the identifier.
    /// </summary>
    /// <response code="204">Character removed successfully (no content returned).</response>
    /// <response code="404">Character URL not found in favorites.</response>
    [HttpDelete("favorites")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveFavorite([FromQuery] string url)
    {
        var favorite = await _context.FavoriteCharacters
            .FirstOrDefaultAsync(f => f.Url == url);

        if (favorite == null)
        {
            return NotFound(new { message = "Character not found in favorites." });
        }

        _context.FavoriteCharacters.Remove(favorite);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}