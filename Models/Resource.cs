using System.ComponentModel.DataAnnotations;


namespace ResourceBookingSystem.Models
{
    public class Resource
    {
        public int Id { get; set; }

        // Name is required so users know what they are booking
        [Required(ErrorMessage = "Resource name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        // To describe the resource (room, car, projector, etc)
        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = string.Empty;

        // Tells employees where they will find the resource 
        [Required(ErrorMessage = "Location is required.")]
        public string Location { get; set; } = string.Empty;

        // Capacity must be a positive number
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0.")]
        public int Capacity { get; set; }

        // A resource is available unless marked otherwise
        public bool IsAvailable { get; set; } = true;

        // Navigation property
        public List<Booking> Bookings { get; set; } = new();
    }
}
