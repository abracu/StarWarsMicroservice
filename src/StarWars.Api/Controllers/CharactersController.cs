using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Required for .ToListAsync() extension method
using StarWars.Application.DTOs;
using StarWars.Application.Interfaces;
using StarWars.Domain.Entities;
using StarWars.Infrastructure.Persistence;

namespace StarWars.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CharactersController : ControllerBase
{
    private readonly ISwapiService _swapiService;
    private readonly StarWarsDbContext _context; // [NOTE]: Injecting DbContext directly for pragmatic simplicity

    public CharactersController(ISwapiService swapiService, StarWarsDbContext context)
    {
        _swapiService = swapiService;
        _context = context;
    }

    /// <summary>
    /// Retrieves a paginated list of characters from SWAPI.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int page = 1)
    {
        var characters = await _swapiService.GetPeopleAsync(page);
        return Ok(characters);
    }

    /// <summary>
    /// Searches for characters by name via SWAPI.
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string name)
    {
        var characters = await _swapiService.SearchPeopleAsync(name);
        return Ok(characters);
    }

    // ==========================================
    // Favorites Management (Local DB Persistence)
    // ==========================================

    /// <summary>
    /// Retrieves the list of favorite characters stored in the local database.
    /// </summary>
    [HttpGet("favorites")]
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
    [HttpPost("favorites")]
    public async Task<IActionResult> AddFavorite([FromBody] CreateFavoriteDto dto)
    {
        // Basic validation: Prevent duplicates based on URL
        if (await _context.FavoriteCharacters.AnyAsync(f => f.Url == dto.Url))
        {
            return Conflict("This character is already in your favorites.");
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
    [HttpDelete("favorites")]
    public async Task<IActionResult> RemoveFavorite([FromQuery] string url)
    {
        var favorite = await _context.FavoriteCharacters
            .FirstOrDefaultAsync(f => f.Url == url);

        if (favorite == null)
        {
            return NotFound("Character not found in favorites.");
        }

        _context.FavoriteCharacters.Remove(favorite);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}