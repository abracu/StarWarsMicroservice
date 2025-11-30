using Microsoft.EntityFrameworkCore;
using StarWars.Infrastructure.Persistence;
using StarWars.Application.Interfaces;
using StarWars.Infrastructure.ExternalApi;
using StarWars.Api.Middleware;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. Service Configuration
// ==========================================

builder.Services.AddControllers();

// Swagger Configuration (Swashbuckle)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database Context
//! [NOTE]: Retrieve connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<StarWarsDbContext>(options =>
    options.UseNpgsql(connectionString));

// Caching
builder.Services.AddMemoryCache();

// SWAPI Integration (Dependency Injection)
builder.Services.AddHttpClient<SwapiService>(client =>
{
    client.BaseAddress = new Uri("https://swapi.dev/api/");
});

builder.Services.AddScoped<ISwapiService>(provider =>
{
    var swapiService = provider.GetRequiredService<SwapiService>();
    var cache = provider.GetRequiredService<IMemoryCache>();
    return new CachedSwapiService(swapiService, cache);
});

var app = builder.Build();

// ==========================================
// 2. HTTP Request Pipeline
// ==========================================

// Apply migrations automatically
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<StarWarsDbContext>();
    try 
    {
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error applying migrations: {ex.Message}");
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run(); 