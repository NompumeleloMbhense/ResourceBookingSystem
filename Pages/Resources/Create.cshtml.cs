using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ResourceBookingSystem.Data;
using ResourceBookingSystem.Models;

namespace ResourceBookingSystem.Pages.Resources
{
    /// <summary>
    /// Handles creation of new resources. Includes validation, logging,
    /// and safe database operations with proper error handling.
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


        // Holds the Resource object bound to the form inputs.
        [BindProperty]
        public Resource Resource { get; set; } = default!;


        // Displays the Create Resource page.
        // No data needs to be preloaded here.
        public IActionResult OnGet()
        {
            return Page();
        }

        
        // Handles submission of the Create form.
        // Performs server-side validation, database persistence,
        // and logs success or failure.
        public async Task<IActionResult> OnPostAsync()
        {
            // Validate properties according to the DataAnnotations on the Resource model
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Resource creation failed validation.");
                return Page();
            }

            try
            {
                // Begin tracking the new resource
                _context.Resources.Add(Resource);

                // Save new record to database
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Resource created successfully. ResourceId={ResourceId}, Name={ResourceName}",
                    Resource.Id,
                    Resource.Name);

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                // Capture unexpected errors to prevent application crash
                _logger.LogError(
                    ex,
                    "Unexpected error occurred while creating resource. Name={ResourceName}",
                    Resource.Name);

                // Provide meaningful feedback to the user
                ModelState.AddModelError(string.Empty,
                    "An error occurred while saving the resource. Please try again.");

                return Page();
            }
        }
    }
}
