using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ResourceBookingSystem.Data;
using ResourceBookingSystem.Models;

namespace ResourceBookingSystem.Pages.Bookings
{
    public class CreateModel : PageModel
    {
        private readonly ResourceBookingSystem.Data.ApplicationDbContext _context;

        public CreateModel(ResourceBookingSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["ResourceId"] = new SelectList(_context.Resources, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public Booking Booking { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            // Ensure the model passed validation (required fields, types, etc.)

            if (!ModelState.IsValid)
            {
                // Reload resource list for the dropdown if validation fails
                ViewData["ResourceId"] = new SelectList(_context.Resources, "Id", "Name");
                return Page();
            }


            // --------------------------------------------------------
            // Check for booking conflicts 
            // --------------------------------------------------------

            var existingBookings = await _context.Bookings
                .Where(b => b.ResourceId == Booking.ResourceId)
                .ToListAsync();

            // Checking if the new booking overlaps with any existing booking
            bool conflict = existingBookings.Any(b =>
                (Booking.StartTime >= b.StartTime && Booking.StartTime < b.EndTime) ||
                (Booking.EndTime > b.StartTime && Booking.EndTime <= b.EndTime) ||
                (Booking.StartTime <= b.StartTime && Booking.EndTime >= b.EndTime)
            );

            // 
            if (conflict)
            {
                // Add an error to the ModelState so it displays on the Razor Page
                ModelState.AddModelError(string.Empty, "This resource is already booked during the selected time range.");

                // Reload resource name for dropdown
                ViewData["ResourceId"] = new SelectList(_context.Resources, "Id", "Name");
                return Page();
            }

            // -------------------------------------------------
            // NO CONFLICT SO CREATE BOOKING
            //--------------------------------------------------


            _context.Bookings.Add(Booking);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
