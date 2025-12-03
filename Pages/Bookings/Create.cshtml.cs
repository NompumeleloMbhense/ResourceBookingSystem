using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ResourceBookingSystem.Data;
using ResourceBookingSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ResourceBookingSystem.Pages.Bookings
{

    /// <summary>
    /// Handles creation of new bookings. Includes full validation,
    /// conflict detection and safe database saving with error handling.
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


        // Loads list of resources when the Create Booking form is opened
        public IActionResult OnGet()
        {
            ViewData["ResourceId"] = new SelectList(_context.Resources, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public Booking Booking { get; set; } = default!;


        // Handles booking submission: validates fields, checks for conflicts,
        // and saves using a database transaction.
        public async Task<IActionResult> OnPostAsync()
        {
            // Model validation (required fields, correct types)
            if (!ModelState.IsValid)
            {
                // Reload resource list for the dropdown if validation fails
                ViewData["ResourceId"] = new SelectList(_context.Resources, "Id", "Name");
                return Page();
            }



            // Booking conflict validation
            // Prevent overlapping bookings for the same resource.
            var conflictExists = await _context.Bookings
                .AnyAsync(b => b.ResourceId == Booking.ResourceId &&
                    (
                        (Booking.StartTime >= b.StartTime && Booking.StartTime < b.EndTime) ||
                        (Booking.EndTime > b.StartTime && Booking.EndTime <= b.EndTime) ||
                        (Booking.StartTime <= b.StartTime && Booking.EndTime >= b.EndTime)
                    )
                );

            if (conflictExists)
            {
                ModelState.AddModelError(string.Empty,
                    "This resource is already booked during the selected time range.");

                ViewData["ResourceId"] = new SelectList(_context.Resources, "Id", "Name");
                return Page();
            }


            // Save booking inside a safe DB transaction
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Bookings.Add(Booking);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                _logger.LogInformation("Booking created successfully. BookingId={BookingId}, ResourceId={ResourceId}",
                    Booking.Id, Booking.ResourceId);

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(ex,
                    "Error while creating booking for ResourceId={ResourceId}", Booking.ResourceId);

                ModelState.AddModelError(string.Empty,
                    "An error occurred while saving the booking. Please try again.");

                ViewData["ResourceId"] = new SelectList(_context.Resources, "Id", "Name");
                return Page();
            }
        }
    }
}
