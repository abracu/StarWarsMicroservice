using System.Diagnostics;
using StarWars.Domain.Entities;
using StarWars.Infrastructure.Persistence;

namespace StarWars.Api.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    // Optional: Inject ILogger for standard console logging
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    // [NOTE]: DbContext is injected here (Method Injection) instead of the constructor
    // because Middleware is Singleton but DbContext is Scoped.
    public async Task InvokeAsync(HttpContext context, StarWarsDbContext dbContext)
    {
        // 1. Start the stopwatch
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // 2. Pass the request to the next component in the pipeline
            await _next(context);
        }
        finally
        {
            // 3. Stop the timer and log (executes even if an exception occurred)
            stopwatch.Stop();

            // Construct the log entity
            var logEntry = new RequestLog
            {
                Id = Guid.NewGuid(),
                HttpMethod = context.Request.Method,
                Endpoint = context.Request.Path,
                QueryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : null,
                StatusCode = context.Response.StatusCode,
                Timestamp = DateTime.UtcNow,
                DurationMs = stopwatch.ElapsedMilliseconds,
                // Retrieve IP address (might be null in local development)
                IpAddress = context.Connection.RemoteIpAddress?.ToString()
            };

            // 4. Persist to Database
            // [NOTE]: In high-load production scenarios, this should be offloaded to a 
            // message queue (e.g., RabbitMQ) to avoid blocking the response.
            // Direct DB persistence is acceptable for this scope.
            dbContext.RequestLogs.Add(logEntry);
            await dbContext.SaveChangesAsync();
            
            _logger.LogInformation($"Request {logEntry.HttpMethod} {logEntry.Endpoint} completed in {logEntry.DurationMs}ms");
        }
    }
}