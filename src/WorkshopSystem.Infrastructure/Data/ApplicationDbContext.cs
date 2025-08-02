using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorkshopSystem.Core.Domain.Entities;

namespace WorkshopSystem.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ServiceRecord> ServiceRecords { get; set; }
        public DbSet<Car> Cars { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure entity relationships and constraints
            modelBuilder.Entity<ServiceRecord>()
                .HasOne(sr => sr.AssignedMechanic)
                .WithMany(m => m.ServicesPerformed)
                .HasForeignKey(sr => sr.AssignedMechanicId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ServiceRecord>()
                .HasOne(sr => sr.AssignedMechanic)
                .WithMany(m => m.ServicesPerformed)
                .HasForeignKey(sr => sr.AssignedMechanicId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ServiceRecord>()
                .HasOne(sr => sr.RequestedBy)
                .WithMany(u => u.ServiceRequests)
                .HasForeignKey(sr => sr.RequestedById)
                .OnDelete(DeleteBehavior.Restrict);

            // ServiceRecord -> Customer
            modelBuilder.Entity<ServiceRecord>()
                .HasOne(sr => sr.Customer)
                .WithMany()
                .HasForeignKey(sr => sr.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // ServiceRecord -> Car
            modelBuilder.Entity<ServiceRecord>()
                .HasOne(sr => sr.Car)
                .WithMany()
                .HasForeignKey(sr => sr.CarId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure decimal precision for ServiceCost
            modelBuilder.Entity<ServiceRecord>()
                .Property(sr => sr.ServiceCost)
                .HasColumnType("decimal(18,2)");
        }
    }
}
