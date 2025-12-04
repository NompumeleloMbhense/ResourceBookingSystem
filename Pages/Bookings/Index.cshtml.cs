using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ResourceBookingSystem.Data;
using ResourceBookingSystem.Models;

namespace ResourceBookingSystem.Pages.Bookings
{
    /// <summary>
    /// Displays a list of all bookings in the system.
    /// Includes loading of related Resource data and full logging.
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

        
        // Holds all bookings to be displayed on the page.
        // Includes related Resource information.
        public IList<Booking> Booking { get; set; } = new List<Booking>();


        
        // Loads all bookings when the page is accessed (GET).
        // Includes related Resource data using EF Core's Include method.
        // Logs success or errors
        public async Task OnGetAsync()
        {
            try
            {
                _logger.LogInformation("Loading all bookings from database...");

                Booking = await _context.Bookings
                    .Include(b => b.Resource) // Load associated resource (name, etc.)
                    .OrderBy(b => b.StartTime) // Order by date
                    .ToListAsync();

                _logger.LogInformation("Successfully loaded {Count} bookings.", Booking.Count);
            }
            catch (Exception ex)
            {
                // Log unexpected issues (DB unavailable, connection issues, etc.)
                _logger.LogError(ex, "Error loading bookings from database.");

                // Prevents null reference in UI
                Booking = new List<Booking>();
            }
        }
    }
}
