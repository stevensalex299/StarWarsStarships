using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class StarshipsController : Controller
{
    private readonly StarshipContext _context;
    private readonly StarshipService _starshipService;
    private const int PageSize = 5;

    public StarshipsController(StarshipContext context, StarshipService starshipService)
    {
        _context = context;
        _starshipService = starshipService;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        // Calculate total pages and ensure the page number is valid
        var totalStarships = await _context.Starships.CountAsync();
        var totalPages = (int)Math.Ceiling(totalStarships / (double)PageSize);
        page = Math.Max(1, Math.Min(page, totalPages));

        // Fetch the starships for the current page
        var starships = await _context.Starships
            .OrderBy(s => s.Name)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        // Check if the request is an AJAX request
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            // Return the partial view for AJAX requests
            return PartialView("_StarshipList", new { Starships = starships, CurrentPage = page, TotalPages = totalPages });
        }

        // Fetch a random starship for non-AJAX requests
        var randomStarship = await GetRandomStarship();

        // Set ViewBag properties for the view
        ViewBag.RandomStarship = randomStarship;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;

        // Return the view with the starships for the current page
        return View(starships);
    }

    public async Task<IActionResult> Details(int id)
    {
        var starship = await GetStarshipByIdAsync(id);
        if (starship == null) return NotFound();

        return View(starship);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var starship = await GetStarshipByIdAsync(id);
        if (starship == null) return NotFound();

        return View(starship);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var starship = await GetStarshipByIdAsync(id);
        if (starship == null) return NotFound();

        _context.Starships.Remove(starship);
        await _context.SaveChangesAsync();
        
        return RedirectToAction(nameof(Index));
    }

    // Helpers
    private bool StarshipExists(int id)
    {
        return _context.Starships.Any(e => e.StarshipId == id);
    }

    private async Task<Starship?> GetStarshipByIdAsync(int? id)
    {
        if (id == null) return null;

        return await _context.Starships
            .FirstOrDefaultAsync(m => m.StarshipId == id);
    }

    private async Task<Starship?> GetRandomStarship()
    {
        var count = await _context.Starships.CountAsync();
        if (count == 0) return null;

        var randomIndex = new Random().Next(count);
        return await _context.Starships.Skip(randomIndex).FirstOrDefaultAsync();
    }
}
