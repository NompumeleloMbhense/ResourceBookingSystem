using System;
using System.ComponentModel.DataAnnotations;

namespace ResourceBookingSystem.Models
{

    /// <summary>
    /// Represents a booking made for a resource.
    /// </summary>
    public class Booking :IValidatableObject
    {
        // Primary Key of the Booking
        public int Id { get; set; } 

        // Foreign Key
        // Links a booking to a specific resource
        [Display(Name = "Resource")]
        [Required(ErrorMessage = "Please select a resource.")]
        public int ResourceId { get; set; }
        


        // StartTime must be provided
        [Display(Name = "Start Time")]
        [Required(ErrorMessage = "Start time is required.")]
        public DateTime StartTime { get; set; }

        // EndTime must be provided
        [Display(Name = "End Time")]
        [Required(ErrorMessage = "End time is required.")]
        public DateTime EndTime { get; set; }

        // Person making the booking
        [Display(Name = "Booked By")]
        [Required(ErrorMessage = "Booked By is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string BookedBy { get; set; } = string.Empty;

        // Booking purpose
        [Display(Name = "Purpose")]
        [Required(ErrorMessage = "Purpose is required.")]
        [StringLength(200, ErrorMessage = "Purpose cannot exceed 200 characters.")]
        public string Purpose { get; set; } = string.Empty;

        // Navigation property for resource 
        // Creates a relationship between Booking and Resource
        public Resource? Resource { get; set; } 

        // ----------------------------------------------------
        // Custom Validation: EndTime must be after StartTime
        // To ensure EndTime is strictly greater than StartTime.
        // ----------------------------------------------------

        // Implementing IValidateObject for custom validation
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndTime <= StartTime)
            {
                yield return new ValidationResult(
                    "End time must be after the start time.",
                    new[] { nameof(EndTime) });
            }
        }
    }
}
