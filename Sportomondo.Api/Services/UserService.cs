using Microsoft.AspNetCore.Identity;
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

        }

        public async Task ChangeRoleAsync(int id, string newRoleName)
        {
            
        }
    }
}
