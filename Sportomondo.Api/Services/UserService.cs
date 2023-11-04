using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Sportomondo.Api.Context;
using Sportomondo.Api.Exceptions;
using Sportomondo.Api.Models;
using Sportomondo.Api.Requests;
using Sportomondo.Api.Responses;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Sportomondo.Api.Services
{
    public class UserService : IUserService
    {
        private readonly SportomondoDbContext _dbContext;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;

        public UserService(SportomondoDbContext dbContext, IPasswordHasher<User> passwordHasher, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
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

        public async Task<LoginUserResponse> LoginAsync(LoginUserRequest request)
        {
            var user = await GetUserFromDbAsync(request.Email);

            var passwordVerifactionResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (passwordVerifactionResult == PasswordVerificationResult.Failed)
            {
                throw new BadRequestException("Wrong password");
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                new Claim(ClaimTypes.Role, user.Role.Name)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Jwt:Key")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(15);

            var token = new JwtSecurityToken(_configuration.GetValue<string>("Jwt:Issuer"),
                _configuration.GetValue<string>("Jwt:Audience"),
                claims,
                expires: expires,
                signingCredentials: credentials);

            return new LoginUserResponse()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expires
            };
        }

        public async Task ChangePasswordAsync(ChangeUserPasswordRequest request) //tests!
        {
            //get current user
            User user = null;

            var verifactionResultWithOldP = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.OldPassword);

            if (verifactionResultWithOldP == PasswordVerificationResult.Failed)
            {
                throw new BadRequestException("'Old password' and current password are not the same");
            }

            var verifactionResultWithNewP = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.NewPassword);

            if (verifactionResultWithNewP != PasswordVerificationResult.Failed)
            {
                throw new BadRequestException("'New password' and current password cannot be the same");
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var users = await _dbContext.Users
                .Include(x => x.Role)
                .Include(u => u.Activities)
                .ToListAsync();

            return users;
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
                throw new Exception($"There is no user with Id: {id}");
            }

            return user;
        }

        private async Task<User> GetUserFromDbAsync(string email)
        {
            var user = await _dbContext.Users
                .Include(u => u.Role)
                .Include(u => u.Activities)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                throw new Exception($"There is no user with email: {email}");
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
