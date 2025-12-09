using Microsoft.EntityFrameworkCore;
using ResourceBookingSystem.Models;

namespace ResourceBookingSystem.Data
{
    /// <summary>
    /// This is a special class that acts as a bridge between  
    /// your C# code (your application) and your database (where your data is stored)
    /// Knows which models become database tables and how they relate.
    /// Manages the connection to the database.
    /// </summary>
    public class ApplicationDbContext: DbContext
    {
        // This constructor receives the database connection settings from the system
        // and passes them to the base DbContext class so EF Core can connect to the database.
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // These properties represent tables in the database
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        // This method is used to configure the relationships between the models
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Resource>() // Configure Resource entity
                .HasMany(r => r.Bookings)  // A resource has many bookings
                .WithOne(b => b.Resource) // Each booking has one resource
                .HasForeignKey(b => b.ResourceId); // Foreign key in booking pointing to resource
        }
    }
}
