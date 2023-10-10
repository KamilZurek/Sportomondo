using Microsoft.EntityFrameworkCore;
using Sportomondo.Api.Context;
using Sportomondo.Api.Models;

namespace Sportomondo.Api.Seeders
{
    public class DataSeeder
    {
        private readonly SportomondoDbContext _dbContext;

        public DataSeeder(SportomondoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void ApplyPendingMigrations()
        {
            if (_dbContext.Database.CanConnect())
            {
                var pendingMigrations = _dbContext.Database.GetPendingMigrations();

                if (pendingMigrations != null && pendingMigrations.Any())
                {
                    _dbContext.Database.Migrate();
                }
            }
        }

        public void Seed()
        {
            if (_dbContext.Database.CanConnect())
            {
                SeedUsers();
                //seed others?
            }
        }

        private void SeedUsers()
        {
            if (!_dbContext.Users.Any())
            {
                var users = GetUsersToSeed();

                _dbContext.Users.AddRange(users);
                _dbContext.SaveChanges();
            }
        }

        private static IEnumerable<User> GetUsersToSeed()
        {
            return new List<User>()
            {
                new User()
                {
                    FirstName = "Admin",
                    LastName = "1",
                    DateOfBirth = new DateTime(1995, 2, 27),
                    Weight = 109.0m
                    //add password hash etc?
                }
            };
        }
    }
}
