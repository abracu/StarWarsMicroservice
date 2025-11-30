using Microsoft.EntityFrameworkCore;
using StarWars.Domain.Entities;

namespace StarWars.Infrastructure.Persistence;

public class StarWarsDbContext : DbContext
{
    public StarWarsDbContext(DbContextOptions<StarWarsDbContext> options) : base(options)
    {
    }

    // Aquí definimos las tablas
    public DbSet<RequestLog> RequestLogs { get; set; }
    // public DbSet<FavoriteCharacter> FavoriteCharacters { get; set; } // (La crearemos luego)

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de RequestLog
        modelBuilder.Entity<RequestLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.HttpMethod)
                .IsRequired()
                .HasMaxLength(10); // GET, POST, DELETE no son muy largos

            entity.Property(e => e.Endpoint)
                .IsRequired()
                .HasMaxLength(250);

            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP"); // Valor por defecto en Postgres
        });
    }
}