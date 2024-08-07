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

            // Populate the imageURLs based on local assets
            foreach (var starship in starships)
            {
                starship.ImageURL = GetImageUrlForStarship(starship.Name);
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

    private string GetImageUrlForStarship(string starshipName)
    {
        // Sanitize starship name to create a valid image file name
        var sanitizedStarshipName = starshipName
            .Replace(" ", "-")
            .Replace("/", "-")
            .ToLower();

        // List of possible image extensions
        var extensions = new[] { ".png", ".jpg", ".jpeg"};
        
        // Construct file path and check each extension
        foreach (var extension in extensions)
        {
            var imageFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", $"{sanitizedStarshipName}{extension}");

            if (File.Exists(imageFilePath))
            {
                return $"/Assets/{sanitizedStarshipName}{extension}";
            }
        }

        // Return placeholder image URL if no file exists
        return "https://via.placeholder.com/";
    }

}
