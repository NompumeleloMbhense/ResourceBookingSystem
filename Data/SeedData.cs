using System.Linq;
using Microsoft.EntityFrameworkCore;
using ResourceBookingSystem.Models;

namespace ResourceBookingSystem.Data
{
    /// <summary>
    /// Handles initial database seeding.
    /// Runs automatically at application startup.
    /// </summary>
    public static class SeedData
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Ensure database exists and migrations are applied
            context.Database.Migrate();

            // Seed Resources only if table is empty
            if (!context.Resources.Any())
            {
                context.Resources.AddRange(
                    new Resource
                    {
                        Name = "Meeting Room Alpha",
                        Description = "Large room with projector and whiteboard",
                        Location = "3rd Floor, West Wing",
                        Capacity = 12,
                        IsAvailable = true
                    },
                    new Resource
                    {
                        Name = "Company Car 1",
                        Description = "Compact sedan for staff use",
                        Location = "Parking Bay 5",
                        Capacity = 4,
                        IsAvailable = true
                    },
                    new Resource
                    {
                        Name = "Conference Room Beta",
                        Description = "Medium room with video conferencing",
                        Location = "2nd Floor, East Wing",
                        Capacity = 8,
                        IsAvailable = true
                    }
                );

                context.SaveChanges();
            }
        }
    }
}
