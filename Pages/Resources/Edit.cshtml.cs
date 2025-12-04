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

namespace ResourceBookingSystem.Pages.Resources
{
    /// <summary>
    /// Handles editing of an existing Resource.
    /// Includes validation, error handling, and logging for reliability.
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


        // The Resource entity bound to the form inputs.
        [BindProperty]
        public Resource Resource { get; set; } = default!;


        // Loads the resource to be edited.
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                // The user navigated without specifying an ID
                return NotFound();
            }

            // Attempt to load the resource; if not found, show 404
            var resource = await _context.Resources.FirstOrDefaultAsync(m => m.Id == id);
            if (resource == null)
            {
                return NotFound();
            }

            Resource = resource;
            return Page();
        }


        // Handles form submission for editing a resource.
        // Includes validation, concurrency handling, and logging.
        public async Task<IActionResult> OnPostAsync()
        {
            // Ensure validation attributes on the model passed
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Mark the entity as modified so EF Core updates it
            _context.Attach(Resource).State = EntityState.Modified;

            try
            {
                // Attempt to save changes
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Resource updated successfully. ResourceId={ResourceId}",
                    Resource.Id);

                return RedirectToPage("./Index");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Resource was deleted between the GET and POST
                if (!ResourceExists(Resource.Id))
                {
                    _logger.LogWarning(
                        "Resource not found during edit (possible delete). ResourceId={ResourceId}",
                        Resource.Id);

                    return NotFound();
                }

                // A true concurrency conflict occurred
                _logger.LogError(
                    ex,
                    "Concurrency exception while editing ResourceId={ResourceId}",
                    Resource.Id);

                // Show helpful feedback to the user
                ModelState.AddModelError(string.Empty,
                    "Another user modified this resource at the same time. Please reload and try again.");

                return Page();
            }
            catch (Exception ex)
            {
                // Generic error catch to prevent application crash
                _logger.LogError(
                    ex,
                    "Unexpected error while updating ResourceId={ResourceId}",
                    Resource.Id);

                ModelState.AddModelError(string.Empty,
                    "An unexpected error occurred while updating this resource. Please try again.");

                return Page();
            }
        }


        // Helper method: checks if the resource still exists in the database
        private bool ResourceExists(int id)
        {
            return _context.Resources.Any(e => e.Id == id);
        }
    }
}
