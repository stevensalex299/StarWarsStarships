using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

public class DbSeeder
{
    private readonly StarshipContext _context;
    private readonly StarshipService _starshipService;
    private readonly ILogger<DbSeeder> _logger;

    public DbSeeder(StarshipContext context, StarshipService starshipService, ILogger<DbSeeder> logger)
    {
        _context = context;
        _starshipService = starshipService;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            // Add migrations and create database if necessary
            await _context.Database.MigrateAsync();

            // Check if database is populated, seed if not
            if (await _context.Starships.AnyAsync())
            {
                _logger.LogInformation("Database is already seeded.");
                return;
            }

            var starships = await _starshipService.GetAllStarshipsAsync();

            // Check that there are starships to be seeded, seed if so
            if (starships == null || !starships.Any())
            {
                _logger.LogWarning("No starship data found.");
                return;
            }

            await _context.Starships.AddRangeAsync(starships);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Database has been seeded with starship data.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred during seeding.");
        }
    }
}
