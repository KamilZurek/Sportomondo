using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sportomondo.Api.Context;
using Sportomondo.Api.Exceptions;
using Sportomondo.Api.Models;
using System.Data;
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
                if (_dbContext.Database.IsRelational())
                {
                    var pendingMigrations = _dbContext.Database.GetPendingMigrations();

                    if (pendingMigrations != null && pendingMigrations.Any())
                    {
                        _dbContext.Database.Migrate();
                    }
                }  
            }
        }

        public void Seed()
        {
            if (_dbContext.Database.CanConnect())
            {
                SeedRoles();
                SeedRolePermissions(); //nowe akcje - nowy seed od zera (jeżeli można)
                SeedUsers();
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
                    Name = Role.AdminRoleName,
                    Description = "System administrator. Can do everything."
                },
                new Role()
                {
                    Name = Role.SupportRoleName,
                    Description = "Helpdesk. Can get and modify all data (without user management)."
                },
                new Role()
                {
                    Name = Role.MemberRoleName,
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
                var role = _dbContext.Roles
                    .FirstOrDefault(x => x.Name == item.RoleName);

                if (role == null)
                {
                    throw new Exception($"Missing role '{item.RoleName}' in database - cannot seed role permissions");
                }

                var rolePermission = new RolePermission()
                {
                    Name = item.Name,
                    Description = item.Description,
                    Enabled = item.Enabled,
                    RoleId = role.Id
                };

                results.Add(rolePermission);
            }

            return results;
        }

        private IEnumerable<User> GetUsersToSeed()
        {
            var adminRole = _dbContext.Roles
                .FirstOrDefault(x => x.Name == Role.AdminRoleName);

            if (adminRole == null)
            {
                throw new Exception($"Missing role '{Role.AdminRoleName}' in database - cannot seed users");
            }

            var adminEmail = _configuration.GetValue<string>("BaseAdminEmail");

            var admin = new User()
            {
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "Main",
                DateOfBirth = new DateTime(1995, 2, 27),
                Weight = 109.0m,
                RoleId = adminRole.Id
            };

            admin.PasswordHash = _passwordHasher.HashPassword(admin, 
                _configuration.GetValue<string>("BaseAdminPasswordToChange"));

            return new List<User>() { admin };
        }
    }
}
