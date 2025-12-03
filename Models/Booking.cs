using System.ComponentModel.DataAnnotations;

namespace ResourceBookingSystem.Models
{
    public class Booking :IValidatableObject
    {
        public int Id { get; set; }

        // Foreign Key
        [Required(ErrorMessage = "Please select a resource.")]
        public int ResourceId { get; set; }
        public Resource? Resource { get; set; }


        // StartTime must be provided
        [Required(ErrorMessage = "Start time is required.")]
        public DateTime StartTime { get; set; }
        // EndTime must be provided
        [Required(ErrorMessage = "End time is required.")]
        public DateTime EndTime { get; set; }

        // Person making the booking
        [Required(ErrorMessage = "BookedBy is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string BookedBy { get; set; } = string.Empty;

        // Booking purpose
        [Required(ErrorMessage = "Purpose is required.")]
        [StringLength(200, ErrorMessage = "Purpose cannot exceed 200 characters.")]
        public string Purpose { get; set; } = string.Empty;

        // ----------------------------------------------------
        // Custom Validation: EndTime must be after StartTime
        // To ensure EndTime is strictly greater than StartTime.
        // ----------------------------------------------------


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
