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
        private readonly ResourceBookingSystem.Data.ApplicationDbContext _context;

        public DetailsModel(ResourceBookingSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Resource Resource { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resource = await _context.Resources.FirstOrDefaultAsync(m => m.Id == id);
            if (resource == null)
            {
                return NotFound();
            }
            else
            {
                Resource = resource;
            }
            return Page();
        }
    }
}
