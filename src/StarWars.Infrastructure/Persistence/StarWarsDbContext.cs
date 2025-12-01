using Microsoft.EntityFrameworkCore;
using StarWars.Domain.Entities;

namespace StarWars.Infrastructure.Persistence;

public class StarWarsDbContext : DbContext
{
    public StarWarsDbContext(DbContextOptions<StarWarsDbContext> options) : base(options)
    {
    }

    // Define database tables (DbSets)
    public DbSet<RequestLog> RequestLogs { get; set; }
    // public DbSet<FavoriteCharacter> FavoriteCharacters { get; set; } // (To be implemented later)

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // RequestLog Entity Configuration
        modelBuilder.Entity<RequestLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.HttpMethod)
                .IsRequired()
                .HasMaxLength(10); // HTTP methods (GET, POST, DELETE) are short strings

            entity.Property(e => e.Endpoint)
                .IsRequired()
                .HasMaxLength(250);

            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP"); // Default value in PostgreSQL
        });
    }
}