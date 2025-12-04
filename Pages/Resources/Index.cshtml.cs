using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ResourceBookingSystem.Data;
using ResourceBookingSystem.Models;

namespace ResourceBookingSystem.Pages.Resources
{
    /// <summary>
    /// Displays a list of all resources stored in the system.
    /// Includes logging and error handling for reliability.
    /// </summary>
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ApplicationDbContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }


        // Holds the list of resources to be displayed on the Index page.
        public IList<Resource> Resource { get; set; } = new List<Resource>();


        // Holds the search term from the UI (supports ?SearchTerm=Meeting)
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        // Loads resources, applying a search filter if provided.
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Base query
                var query = _context.Resources.AsQueryable();

                // Apply search filter if user entered a search term
                if (!string.IsNullOrWhiteSpace(SearchTerm))
                {
                    query = query.Where(r =>
                        r.Name.Contains(SearchTerm) ||
                        r.Description.Contains(SearchTerm));
                }

                // Execute query
                Resource = await query.ToListAsync();

                _logger.LogInformation(
                    "Loaded {Count} resources for the Index page. SearchTerm={SearchTerm}",
                    Resource.Count, SearchTerm);

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading resources for the Index page.");

                ModelState.AddModelError(string.Empty,
                    "An error occurred while loading resources.");

                return Page();
            }
        }
    }
}
