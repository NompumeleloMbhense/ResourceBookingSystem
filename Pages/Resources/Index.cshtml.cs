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
    public class IndexModel : PageModel
    {
        private readonly ResourceBookingSystem.Data.ApplicationDbContext _context;

        public IndexModel(ResourceBookingSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Resource> Resource { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Resource = await _context.Resources.ToListAsync();
        }
    }
}
