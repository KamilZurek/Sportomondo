using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sportomondo.Api.Context;
using Sportomondo.Api.Models;
using Sportomondo.Api.Requests;

namespace Sportomondo.Api.Services
{
    public class UserService : IUserService
    {
        private readonly SportomondoDbContext _dbContext;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(SportomondoDbContext dbContext, IPasswordHasher<User> passwordHasher)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// Registering user with basic role - "Member"
        /// Validation is executed by FluentValidation
        /// </summary>
        public async Task RegisterAsync(RegisterUserRequest request)
        {
            var memberRoleId = _dbContext.Roles
               .First(x => x.Name == "Member")
               .Id;

            var user = new User()
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                Weight = request.Weight,
                RoleId = memberRoleId
            };
            
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<string> LoginAsync(LoginUserRequest request)
        {
            return "";
        }

        public async Task ChangePasswordAsync(ChangeUserPasswordRequest request)
        {
            
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return null;
        }

        public async Task DeleteAsync(int id)
        {
            var user = await GetUserFromDbAsync(id);

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task ChangeRoleAsync(int userId, string newRoleName)
        {
            var user = await GetUserFromDbAsync(userId);

            var newRole = await GetRoleFromDbAsync(newRoleName);

            user.RoleId = newRole.Id;

            await _dbContext.SaveChangesAsync();
        }

        private async Task<User> GetUserFromDbAsync(int id)
        {
            var user = await _dbContext.Users
                .Include(u => u.Role)
                .Include(u => u.Activities)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                throw new Exception($"There is no user wit Id: {id}");
            }

            return user;
        }

        private async Task<Role> GetRoleFromDbAsync(string roleName)
        {
            var role = await _dbContext.Roles
                    .FirstOrDefaultAsync(x => x.Name == roleName);

            if (role == null)
            {
                throw new Exception($"There is no role wit Name: {roleName}");
            }

            return role;
        }
    }
}
