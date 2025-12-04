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
    /// <summary>
    /// Handles the confirmation and deletion of a Resource.
    /// Includes defensive checks, logging, and error handling.
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

        
        // Holds the resource to display on the delete confirmation page.
        [BindProperty]
        public Resource Resource { get; set; } = default!;

        
        // Loads the resource that is about to be deleted.
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Delete page accessed with a null resource ID.");
                return NotFound();
            }

            try
            {
                Resource = await _context.Resources.FirstOrDefaultAsync(r => r.Id == id);

                if (Resource == null)
                {
                    _logger.LogWarning("Resource not found when loading delete page. ResourceId={ResourceId}", id);
                    return NotFound();
                }

                _logger.LogInformation("Loaded resource delete confirmation page. ResourceId={ResourceId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading delete page for ResourceId={ResourceId}", id);
                return StatusCode(500, "An error occurred while loading the delete page.");
            }

            return Page();
        }

        
        // Deletes the selected resource from the database.
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Delete attempt failed: null resource ID.");
                return NotFound();
            }

            try
            {
                var resourceToDelete = await _context.Resources.FindAsync(id);

                if (resourceToDelete == null)
                {
                    _logger.LogWarning("Delete attempt failed: resource not found. ResourceId={ResourceId}", id);
                    return NotFound();
                }

                _context.Resources.Remove(resourceToDelete);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Resource deleted successfully. ResourceId={ResourceId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting resource. ResourceId={ResourceId}", id);
                ModelState.AddModelError(string.Empty, "An error occurred while deleting the resource.");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
