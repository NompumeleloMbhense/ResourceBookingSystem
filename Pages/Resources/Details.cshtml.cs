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
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Resource? Resource { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Load the resource and include its related bookings
            Resource = await _context.Resources
                .Include(r => r.Bookings) // Load related bookings
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Resource == null)
            {
                return NotFound();
            }

            // order bookings by start time
            Resource.Bookings = Resource.Bookings
                .OrderBy(b => b.StartTime)
                .ToList();

            return Page();
        }
    }
}
