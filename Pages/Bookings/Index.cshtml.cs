using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ResourceBookingSystem.Data;
using ResourceBookingSystem.Models;

namespace ResourceBookingSystem.Pages.Bookings
{
    /// <summary>
    /// Displays all bookings and supports filtering by BookedBy name and date.
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

        public IList<Booking> Booking { get; set; } = new List<Booking>();

        // The two filter inputs
        [BindProperty(SupportsGet = true)]
        public string? SearchName { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? SearchDate { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Begin query with Resource include
                var query = _context.Bookings
                    .Include(b => b.Resource)
                    .AsQueryable();

                // Filter by name
                if (!string.IsNullOrWhiteSpace(SearchName))
                {
                    query = query.Where(b =>
                        b.BookedBy!.ToLower().Contains(SearchName.ToLower()));
                }

                // Filter by date (matches any booking where the selected day falls between Start & End)
                if (SearchDate.HasValue)
                {
                    var selectedDate = SearchDate.Value.Date;

                    query = query.Where(b =>
                        b.StartTime.Date == selectedDate);
                }

                Booking = await query
                    .OrderBy(b => b.StartTime)
                    .ToListAsync();

                _logger.LogInformation("Loaded {Count} bookings with filters. Name={SearchName}, Date={SearchDate}",
                    Booking.Count, SearchName, SearchDate);

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading filtered bookings.");
                ModelState.AddModelError(string.Empty, "An error occurred while loading bookings.");
                return Page();
            }
        }
    }
}
