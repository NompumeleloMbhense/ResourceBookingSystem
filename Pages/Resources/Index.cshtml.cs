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

        
        // Loads all resources from the database when the page is accessed.
        // Includes logging and safe error handling.
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                Resource = await _context.Resources.ToListAsync();

                _logger.LogInformation("Loaded {Count} resources for the Index page.", Resource.Count);

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading resources for the Index page.");

                // Display a simple error message instead of crashing
                ModelState.AddModelError(string.Empty, "An error occurred while loading resources.");
                return Page();
            }
        }
    }
}
