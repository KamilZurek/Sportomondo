using Microsoft.EntityFrameworkCore;
using Sportomondo.Api.Models;

namespace Sportomondo.Api.Context
{
    public class SportomondoDbContext : DbContext
    {
        public SportomondoDbContext(DbContextOptions<SportomondoDbContext> options)
            : base(options) { }

        public DbSet<Activity> Activities { get; set; }
        public DbSet<Weather> Weathers { get; set; }
        public DbSet<User> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Activity>()
                .Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Activity>()
                .Property(r => r.City)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Weather>()
                .Property(r => r.City)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<User>()
                .Property(r => r.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<User>()
                .Property(r => r.LastName)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}
