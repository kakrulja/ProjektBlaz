using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ProjektBlaz.Data
{
    // The class should inherit IdentityDbContext<ApplicationUser>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        // Constructor that accepts DbContextOptions<ApplicationDbContext> and passes it to the base constructor
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // If needed, you can override the OnModelCreating method here for custom configurations
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Additional model configurations (if needed) can go here

            // If you have specific configurations for the Ticket entity, you can define them here
            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasKey(t => t.Id); // Example: Define primary key
            });
        }

        // Define your DbSet below OnModelCreating
        public DbSet<Ticket> Tickets { get; set; }


    }
}
