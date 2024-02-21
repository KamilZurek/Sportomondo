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
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<Reminder> Reminders { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Activity
            modelBuilder.Entity<Activity>()
                .Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Activity>()
                .Property(r => r.City)
                .IsRequired()
                .HasMaxLength(50);
            #endregion

            #region Weather
            modelBuilder.Entity<Weather>()
                .Property(r => r.City)
                .IsRequired()
                .HasMaxLength(50);
            #endregion

            #region User
            modelBuilder.Entity<User>()
                .Property(r => r.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<User>()
                .Property(r => r.LastName)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<User>()
                .Property(r => r.Email)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(r => r.PasswordHash)
                .IsRequired();
            #endregion

            #region Role
            modelBuilder.Entity<Role>()
               .Property(r => r.Name)
               .IsRequired()
               .HasMaxLength(50);
            #endregion

            #region RolePermission
            modelBuilder.Entity<RolePermission>()
                .Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(50);
            #endregion

            #region Achievement
            modelBuilder.Entity<Achievement>()
               .Property(r => r.Name)
               .IsRequired()
               .HasMaxLength(50);
            #endregion

            #region Reminder
            modelBuilder.Entity<Reminder>()
               .Property(r => r.Type)
               .IsRequired()
               .HasMaxLength(50);
            #endregion
        }
    }
}
