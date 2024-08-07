using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class StarshipsController : Controller
{
    private readonly StarshipContext _context;
    private readonly StarshipService _starshipService;
    private readonly ILogger<StarshipsController> _logger;
    private const int PageSize = 5;

    public StarshipsController(StarshipContext context, StarshipService starshipService, ILogger<StarshipsController> logger)
    {
        _context = context;
        _starshipService = starshipService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        try
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

            // AJAX approach to paginate without reloading entire page
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching starships.");
            return StatusCode(500, "Internal server error");
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var starship = await GetStarshipByIdAsync(id);
            if (starship == null) return NotFound();

            return View(starship);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching details for starship with ID {id}.");
            return StatusCode(500, "Internal server error");
        }
    }

    public async Task<IActionResult> CreateEdit(int? id)
    {
        try
        {
            ViewBag.ReturnUrl = Request.Headers["Referer"].ToString() ?? Url.Action("Index");
            if (id == null)
            {
                return View(new Starship());
            }
            else
            {
                var starship = await GetStarshipByIdAsync(id);
                if (starship == null) return NotFound();

                return View(starship);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching starship with ID {id}.");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEdit(Starship starship, IFormFile ImageFile, string returnURL, int starshipId, string imageURL)
    {
        try
        {
            ModelState.Remove("ImageFile");

            if (ModelState.IsValid)
            {
                starship.StarshipId = starshipId;
                starship.ImageURL = await SaveImageAsync(ImageFile, imageURL);

                if (starship.StarshipId == 0)
                {
                    _context.Add(starship);
                }
                else
                {
                    _context.Update(starship);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = starship.StarshipId });
            }

            ViewBag.returnURL = returnURL;
            return View(starship);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating or editing starship with ID {starshipId}.");
            return StatusCode(500, "Internal server error");
        }
    }

    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var starship = await GetStarshipByIdAsync(id);
            if (starship == null) return NotFound();

            return View(starship);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching starship for deletion with ID {id}.");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var starship = await GetStarshipByIdAsync(id);
            if (starship == null) return NotFound();

            // Check if the starship image is local and delete if so
            if (!string.IsNullOrEmpty(starship.ImageURL) && starship.ImageURL.Contains("Assets"))
            {
                var imageFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", Path.GetFileName(starship.ImageURL));
                if (System.IO.File.Exists(imageFilePath))
                {
                    System.IO.File.Delete(imageFilePath);
                }
            }

            _context.Starships.Remove(starship);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting starship with ID {id}.");
            return StatusCode(500, "Internal server error");
        }
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

    public static async Task<string> SaveImageAsync(IFormFile imageFile, string currentImageUrl)
    {
        if (imageFile == null || imageFile.Length == 0)
        {
            return currentImageUrl;
        }

        // Delete the old image if it exists
        if (!string.IsNullOrEmpty(currentImageUrl))
        {
            var oldImageFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", Path.GetFileName(currentImageUrl.TrimStart('/')));
            if (System.IO.File.Exists(oldImageFilePath))
            {
                System.IO.File.Delete(oldImageFilePath);
            }
        }

        // Sanitize file name
        var fileName = Path.GetFileNameWithoutExtension(imageFile.FileName).Replace(" ", "-").ToLower();
        var extension = Path.GetExtension(imageFile.FileName);
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", $"{fileName}{extension}");

        // Save the new file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        // Return the URL for the new image
        return $"/Assets/{fileName}{extension}";
    }
}
