using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sportomondo.Api.Context;
using Sportomondo.Api.Models;
using System.Text.Json;

namespace Sportomondo.Api.Seeders
{
    public class DataSeeder
    {
        private readonly SportomondoDbContext _dbContext;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;

        public DataSeeder(SportomondoDbContext dbContext, IPasswordHasher<User> passwordHasher, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
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
                SeedRoles();
                SeedRolePermissions(); //nowe akcje - nowy seed od nowa!!!!!
                SeedUsers();

                //seed others?
            }
        }

        private void SeedRoles()
        {
            if (!_dbContext.Roles.Any())
            {
                var roles = GetRolesToSeed();

                _dbContext.Roles.AddRange(roles);
                _dbContext.SaveChanges();
            }
        }

        private void SeedRolePermissions()
        {
            if (!_dbContext.RolePermissions.Any())
            {
                var rolePermissions = GetRolePermissionsToSeed();

                _dbContext.RolePermissions.AddRange(rolePermissions);
                _dbContext.SaveChanges();
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

        private IEnumerable<Role> GetRolesToSeed()
        {
            return new List<Role>()
            {
                new Role()
                {
                    Name = "Admin",
                    Description = "System administrator. Can do everything."
                },
                new Role()
                {
                    Name = "Support",
                    Description = "Helpdesk. Can get and modify all data (without user management)."
                },
                new Role()
                {
                    Name = "Member",
                    Description = "Basic user. Can manage its own activities."
                }
            };
        }

        private IEnumerable<RolePermission> GetRolePermissionsToSeed()
        {
            var filePath = Directory.GetCurrentDirectory() + "\\Seeders\\RolePermissionsSeedData.json";

            if (!File.Exists(filePath))
            {
                throw new Exception($"Cannot seed role permissions - file {filePath} does not exist.");
            }

            var json = File.ReadAllText(filePath);
            var rolePermissionsFromJson = JsonSerializer.Deserialize<List<RolePermissionSeedObject>>(json);

            return MapJsonRolePermissions(rolePermissionsFromJson);
        }

        private IEnumerable<RolePermission> MapJsonRolePermissions(List<RolePermissionSeedObject> dataFromJson)
        {
            var results = new List<RolePermission>();

            foreach (var item in dataFromJson)
            {
                var roleId = _dbContext.Roles
                    .First(x => x.Name == item.RoleName)
                    .Id;

                var rolePermission = new RolePermission()
                {
                    Name = item.Name,
                    Description = item.Description,
                    Enabled = item.Enabled,
                    RoleId = roleId
                };

                results.Add(rolePermission);
            }

            return results;
        }

        private IEnumerable<User> GetUsersToSeed()
        {
            var adminRoleId = _dbContext.Roles
                .First(x => x.Name == "Admin")
                .Id;
            var adminEmail = _configuration.GetValue<string>("BaseAdminEmail");

            var admin = new User()
            {
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "Main",
                DateOfBirth = new DateTime(1995, 2, 27),
                Weight = 109.0m,
                RoleId = adminRoleId
            };

            admin.PasswordHash = _passwordHasher.HashPassword(admin, 
                _configuration.GetValue<string>("BaseAdminPasswordToChange"));

            return new List<User>() { admin };
        }
    }
}
