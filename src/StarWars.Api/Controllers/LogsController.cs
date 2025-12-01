using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarWars.Infrastructure.Persistence;

namespace StarWars.Api.Controllers;

// [NOTE]: We define a lightweight DTO here (or in Application layer)
// so Swagger can generate the correct Response Schema.
public record LogSummaryDto(string Method, string Endpoint, int StatusCode, long DurationMs, DateTime Date);

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class LogsController : ControllerBase
{
    private readonly StarWarsDbContext _context;

    public LogsController(StarWarsDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves the most recent API request logs (Audit Trail).
    /// </summary>
    /// <remarks>
    /// Useful for monitoring system health and inspecting traffic.
    /// The results are ordered by timestamp descending (newest first).
    /// </remarks>
    /// <param name="count">Number of logs to retrieve (Max: 100). Default: 20.</param>
    /// <returns>A list of request logs.</returns>
    /// <response code="200">Returns the requested logs.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<LogSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLogs([FromQuery] int count = 20)
    {
        // Safety: Cap the maximum count to prevent performance degradation
        if (count > 100) count = 100;

        var logs = await _context.RequestLogs
            .AsNoTracking() // Performance: We just read, no need to track changes
            .OrderByDescending(l => l.Timestamp)
            .Take(count)
            .Select(l => new LogSummaryDto(
                l.HttpMethod,
                l.Endpoint,
                l.StatusCode,
                l.DurationMs,
                l.Timestamp
            ))
            .ToListAsync();

        return Ok(logs);
    }
}