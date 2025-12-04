using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ResourceBookingSystem.Data;
using ResourceBookingSystem.Models;
using System;
using System.Threading.Tasks;

namespace ResourceBookingSystem.Pages.Bookings
{
    /// <summary>
    /// Handles creation of new bookings. Includes:
    /// - Server-side validation
    /// - Time conflict detection
    /// - Safe database operations with transactions
    /// - Full logging for troubleshooting and audit trail
    /// </summary>
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(ApplicationDbContext context, ILogger<CreateModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        
        // The Booking object bound to the form inputs.
        [BindProperty]
        public Booking Booking { get; set; } = default!;


        
        /// Loads the Create Booking page.
        /// Preloads the Resource dropdown list
        public IActionResult OnGet()
        {
            _logger.LogInformation("Displaying Create Booking page.");

            ViewData["ResourceId"] = new SelectList(_context.Resources, "Id", "Name");
            return Page();
        }


        
        // Handles form submission:
        // - Validates model
        // - Checks for booking conflicts
        // - Saves using a database transaction
        // - Logs success or failures
        public async Task<IActionResult> OnPostAsync()
        {
            // MODEL VALIDATION
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Booking creation failed validation.");
                ViewData["ResourceId"] = new SelectList(_context.Resources, "Id", "Name");
                return Page();
            }

            _logger.LogInformation(
                "Attempting to create booking for ResourceId={ResourceId}, Start={Start}, End={End}",
                Booking.ResourceId, Booking.StartTime, Booking.EndTime);


            // --------------------------------------------------------------
            // CHECK FOR TIME CONFLICTS
            // --------------------------------------------------------------
            var conflictExists = await _context.Bookings.AnyAsync(b =>
                b.ResourceId == Booking.ResourceId &&
                (
                    (Booking.StartTime >= b.StartTime && Booking.StartTime < b.EndTime) ||
                    (Booking.EndTime > b.StartTime && Booking.EndTime <= b.EndTime) ||
                    (Booking.StartTime <= b.StartTime && Booking.EndTime >= b.EndTime)
                ));

            if (conflictExists)
            {
                _logger.LogWarning(
                    "Booking conflict detected for ResourceId={ResourceId}, Start={Start}, End={End}",
                    Booking.ResourceId, Booking.StartTime, Booking.EndTime);

                ModelState.AddModelError(string.Empty,
                    "This resource is already booked during the selected time range.");

                ViewData["ResourceId"] = new SelectList(_context.Resources, "Id", "Name");
                return Page();
            }


            // --------------------------------------------------------------
            // SAVE BOOKING WITH TRANSACTION
            // --------------------------------------------------------------
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Bookings.Add(Booking);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                _logger.LogInformation(
                    "Booking created successfully. BookingId={BookingId}, ResourceId={ResourceId}",
                    Booking.Id, Booking.ResourceId);

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                // Rollback to avoid corrupting the database
                await transaction.RollbackAsync();

                _logger.LogError(
                    ex,
                    "Unexpected error while creating booking. ResourceId={ResourceId}",
                    Booking.ResourceId);

                ModelState.AddModelError(string.Empty,
                    "An error occurred while saving the booking. Please try again.");

                ViewData["ResourceId"] = new SelectList(_context.Resources, "Id", "Name");
                return Page();
            }
        }
    }
}
