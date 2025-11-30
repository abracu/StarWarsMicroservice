using System.Diagnostics;
using StarWars.Domain.Entities;
using StarWars.Infrastructure.Persistence;

namespace StarWars.Api.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    // Opcional: Inyectar ILogger para logs de consola también
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, StarWarsDbContext dbContext)
    {
        // 1. Iniciar cronómetro
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // 2. Pasar la petición al siguiente componente (El Controlador)
            await _next(context);
        }
        finally
        {
            // 3. Detener cronómetro y registrar (se ejecuta incluso si hubo error)
            stopwatch.Stop();

            // Construimos la entidad de log
            var logEntry = new RequestLog
            {
                Id = Guid.NewGuid(),
                HttpMethod = context.Request.Method,
                Endpoint = context.Request.Path,
                QueryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : null,
                StatusCode = context.Response.StatusCode,
                Timestamp = DateTime.UtcNow,
                DurationMs = stopwatch.ElapsedMilliseconds,
                // Obtenemos la IP (puede ser null en local)
                IpAddress = context.Connection.RemoteIpAddress?.ToString()
            };

            // 4. Guardar en Base de Datos
            // Nota: En producción real de altísima carga, esto se suele enviar a una cola (RabbitMQ)
            // para no frenar la respuesta al usuario. Para esta prueba, guardar directo es aceptable.
            dbContext.RequestLogs.Add(logEntry);
            await dbContext.SaveChangesAsync();
            
            _logger.LogInformation($"Request {logEntry.HttpMethod} {logEntry.Endpoint} completed in {logEntry.DurationMs}ms");
        }
    }
}