using System.Text.Json;
using Microsoft.EntityFrameworkCore;

public class StarshipContext : DbContext
{
    public StarshipContext(DbContextOptions<StarshipContext> options) : base(options) { }

    public DbSet<Starship> Starships { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure the primary key
        modelBuilder.Entity<Starship>()
            .HasKey(s => s.StarshipId);
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<Starship>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.Created = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.Edited = now;
            }
        }
    }
}
