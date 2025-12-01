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
    
    // We uncomment this now as the entity exists
    public DbSet<FavoriteCharacter> FavoriteCharacters { get; set; } 

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

        // FavoriteCharacter Entity Configuration
        modelBuilder.Entity<FavoriteCharacter>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Url).IsRequired().HasMaxLength(250);

            // Unique Index: Prevents duplicate entries for the same character
            entity.HasIndex(e => e.Url).IsUnique();
        });
    }
}