using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ResourceBookingSystem.Data;
using ResourceBookingSystem.Models;

///<summary>
/// Dashboard page model to display today's and upcoming bookings.
/// depends on ApplicationDbContext to fetch booking data from the database.
///</summary>

namespace ResourceBookingSystem.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DashboardModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Booking> TodaysBookings { get; set; } = new();
        public List<Booking> UpcomingBookings { get; set; } = new();

        public async Task OnGetAsync()
        {
            var today = DateTime.Today;

            // Get bookings happening today
            TodaysBookings = await _context.Bookings
                .Include(b => b.Resource)
                .Where(b => b.StartTime.Date == today)
                .OrderBy(b => b.StartTime)
                .ToListAsync();

            // Get next 5 upcoming bookings after today
            UpcomingBookings = await _context.Bookings
                .Include(b => b.Resource)
                .Where(b => b.StartTime > today)
                .OrderBy(b => b.StartTime)
                .Take(5)
                .ToListAsync();
        }
    }
}
