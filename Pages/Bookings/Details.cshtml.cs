using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ResourceBookingSystem.Data;
using ResourceBookingSystem.Models;

namespace ResourceBookingSystem.Pages.Bookings
{
    /// <summary>
    /// Displays detailed information for a single booking.
    /// Includes error handling, logging, and loading related Resource data.
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

        
        // The booking to display, loaded from database.
        public Booking Booking { get; set; } = default!;


        
        // Handles GET requests for displaying booking details.
        // Loads the booking including its related Resource.
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Details page requested with null booking ID.");
                return NotFound();
            }

            try
            {
                Booking? booking = await _context.Bookings
                    .Include(b => b.Resource)  // Load related resource
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (booking == null)
                {
                    _logger.LogWarning("Booking not found. BookingId={BookingId}", id);
                    return NotFound();
                }

                Booking = booking;

                _logger.LogInformation("Loaded booking details successfully. BookingId={BookingId}", id);
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while loading booking details. BookingId={BookingId}", id);
                return StatusCode(500, "An unexpected error occurred while loading booking details.");
            }
        }
    }
}
