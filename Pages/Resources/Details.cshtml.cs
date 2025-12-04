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
    /// Displays detailed information for a single resource, including
    /// all upcoming bookings tied to that resource.
    /// </summary>
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DetailsModel> _logger;

        public DetailsModel(ApplicationDbContext context, ILogger<DetailsModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        
        // The resource retrieved from the database along with its related bookings
        public Resource? Resource { get; set; }

        
        // Retrieves a resource by ID, including its related bookings.
        // Applies ordering to bookings for cleaner display
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // Ensure a valid ID was provided
            if (id == null)
            {
                _logger.LogWarning("Details page accessed with a null resource ID.");
                return NotFound();
            }

            try
            {
                // Retrieve the resource and include its relationship data
                Resource = await _context.Resources
                    .Include(r => r.Bookings)
                    .FirstOrDefaultAsync(r => r.Id == id);

                // Resource not found
                if (Resource == null)
                {
                    _logger.LogWarning("Resource not found. ResourceId={ResourceId}", id);
                    return NotFound();
                }

                // Order the bookings for display purposes (chronologically)
                Resource.Bookings = Resource.Bookings
                    .OrderBy(b => b.StartTime)
                    .ToList();

                _logger.LogInformation("Loaded details for ResourceId={ResourceId}", id);
            }
            catch (Exception ex)
            {
                // Catch unexpected errors, log, and return appropriate result
                _logger.LogError(ex, "Error retrieving details for ResourceId={ResourceId}", id);
                return StatusCode(500, "An error occurred while retrieving resource details.");
            }

            return Page();
        }
    }
}
