using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ResourceBookingSystem.Data;
using ResourceBookingSystem.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ResourceBookingSystem.Pages.Bookings
{
    /// <summary>
    /// Handles editing of existing bookings.
    /// Includes:
    /// - Model validation
    /// - Time conflict detection (excluding the current booking)
    /// - Logging
    /// - Safe database update with proper error handling
    /// </summary>
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EditModel> _logger;

        public EditModel(ApplicationDbContext context, ILogger<EditModel> logger)
        {
            _context = context;
            _logger = logger;
        }


        // Booking being edited, bound to the form.
        [BindProperty]
        public Booking Booking { get; set; } = default!;



        // Loads the Edit page and fetches the booking.
        // Populates the Resource dropdown.
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Edit page requested with null booking ID.");
                return NotFound();
            }

            Booking? booking = await _context.Bookings
                .Include(b => b.Resource)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null)
            {
                _logger.LogWarning("Booking not found. BookingId={Id}", id);
                return NotFound();
            }

            Booking = booking;

            // Populate dropdown with Resource names
            ViewData["ResourceId"] = new SelectList(_context.Resources, "Id", "Name", Booking.ResourceId);

            return Page();
        }



        // Saves the edited booking.
        // Includes conflict detection and full error logging.
        public async Task<IActionResult> OnPostAsync()
        {
            // Validate fields
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Booking update failed validation.");
                ViewData["ResourceId"] = new SelectList(_context.Resources, "Id", "Name", Booking.ResourceId);
                return Page();
            }

            _logger.LogInformation(
                "Attempting to update BookingId={BookingId}, ResourceId={ResourceId}",
                Booking.Id, Booking.ResourceId);


            // --------------------------------------------------------------------
            // CHECK FOR BOOKING CONFLICTS — EXCLUDING the current booking
            // --------------------------------------------------------------------
            bool conflictExists = await _context.Bookings.AnyAsync(b =>
                b.Id != Booking.Id && // exclude itself
                b.ResourceId == Booking.ResourceId &&
                (
                    (Booking.StartTime >= b.StartTime && Booking.StartTime < b.EndTime) ||
                    (Booking.EndTime > b.StartTime && Booking.EndTime <= b.EndTime) ||
                    (Booking.StartTime <= b.StartTime && Booking.EndTime >= b.EndTime)
                ));

            if (conflictExists)
            {
                _logger.LogWarning(
                    "Conflict detected when editing BookingId={BookingId}. ResourceId={ResourceId}",
                    Booking.Id, Booking.ResourceId);

                ModelState.AddModelError(string.Empty,
                    "This resource is already booked during the selected time range.");

                ViewData["ResourceId"] = new SelectList(_context.Resources, "Id", "Name", Booking.ResourceId);
                return Page();
            }


            // --------------------------------------------------------------------
            // UPDATE BOOKING
            // --------------------------------------------------------------------
            try
            {
                _context.Attach(Booking).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Booking updated successfully. BookingId={BookingId}", Booking.Id);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!BookingExists(Booking.Id))
                {
                    _logger.LogWarning("Booking no longer exists during update. BookingId={BookingId}", Booking.Id);
                    return NotFound();
                }

                _logger.LogError(ex,
                    "Concurrency error while updating BookingId={BookingId}", Booking.Id);

                throw; // throw to avoid hiding a real concurrency issue
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Unexpected error while updating BookingId={BookingId}", Booking.Id);

                ModelState.AddModelError(string.Empty,
                    "Unexpected error occurred while updating the booking. Please try again.");

                ViewData["ResourceId"] = new SelectList(_context.Resources, "Id", "Name", Booking.ResourceId);
                return Page();
            }

            return RedirectToPage("./Index");
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.Id == id);
        }
    }
}
