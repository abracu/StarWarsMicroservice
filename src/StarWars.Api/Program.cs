using Microsoft.EntityFrameworkCore;
using StarWars.Infrastructure.Persistence;
using StarWars.Application.Interfaces;
using StarWars.Infrastructure.ExternalApi;
using StarWars.Api.Middleware;
using Microsoft.Extensions.Caching.Distributed;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. Service Configuration
// ==========================================

builder.Services.AddControllers();

// Swagger Configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Generamos el nombre del archivo XML basándonos en el nombre del ensamblado (StarWars.Api.xml)
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    // Combinamos con la ruta base de ejecución (funciona en Docker y Local)
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Database Context
// [NOTE]: Retrieve connection string from environment variables
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<StarWarsDbContext>(options =>
    options.UseNpgsql(connectionString));

// Redis Caching Configuration
// [NOTE]: Using IDistributedCache backed by Redis (Scalable)
builder.Services.AddStackExchangeRedisCache(options =>
{
    // Retrieve connection string from env vars (defined in docker-compose as ConnectionStrings__Redis)
    // Fallback to localhost for local development if null
    options.Configuration = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
    options.InstanceName = "StarWars_"; // Prefix for all keys in Redis
});

// SWAPI Integration (Concrete Service Registration)
builder.Services.AddHttpClient<SwapiService>(client =>
{
    client.BaseAddress = new Uri("https://swapi.dev/api/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Decorator Registration (Cached Service)
builder.Services.AddScoped<ISwapiService>(provider =>
{
    // 1. Get the real logic service
    var swapiService = provider.GetRequiredService<SwapiService>();
    
    // 2. Get the Distributed Cache (Redis) instead of Memory Cache
    var cache = provider.GetRequiredService<IDistributedCache>(); 
    
    // 3. Return the decorator wrapping the real service
    return new CachedSwapiService(swapiService, cache);
});

var app = builder.Build();

// ==========================================
// 2. HTTP Request Pipeline
// ==========================================

// Apply migrations automatically at startup
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