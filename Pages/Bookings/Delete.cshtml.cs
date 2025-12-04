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
    /// Handles loading and deleting a booking.
    /// Includes full logging, error handling, and safe database operations.
    /// </summary>
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(ApplicationDbContext context, ILogger<DeleteModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        
        // The booking to be displayed on the confirmation page.
        [BindProperty]
        public Booking Booking { get; set; } = default!;


        
        // Loads the Booking for the delete confirmation page.
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Delete requested with null booking ID.");
                return NotFound();
            }

            try
            {
                Booking? booking = await _context.Bookings
                    .Include(b => b.Resource)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (booking == null)
                {
                    _logger.LogWarning("Booking not found for delete. BookingId={BookingId}", id);
                    return NotFound();
                }

                Booking = booking;

                _logger.LogInformation("Loaded booking for deletion. BookingId={BookingId}", id);
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error loading booking for deletion. BookingId={BookingId}", id);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }


        
        // Handles deletion of the booking after confirmation.
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Delete POST called with null booking ID.");
                return NotFound();
            }

            try
            {
                Booking? booking = await _context.Bookings.FindAsync(id);

                if (booking == null)
                {
                    _logger.LogWarning("Booking not found during deletion. BookingId={BookingId}", id);
                    return NotFound();
                }

                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Booking deleted successfully. BookingId={BookingId}", id);

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while deleting booking. BookingId={BookingId}", id);
                return StatusCode(500, "An unexpected error occurred while deleting the booking.");
            }
        }
    }
}
