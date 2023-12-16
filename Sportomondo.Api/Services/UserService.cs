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
        private readonly IHttpContextService _contextService;

        public UserService(SportomondoDbContext dbContext, IPasswordHasher<User> passwordHasher, IConfiguration configuration, IHttpContextService contextService)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
            _contextService = contextService;
        }

        /// <summary>
        /// Register user with basic role - "Member".
        /// Validation is executed by FluentValidation
        /// </summary>
        public async Task RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default)
        {
            var memberRole = await _dbContext.Roles
               .FirstOrDefaultAsync(x => x.Name == Role.MemberRoleName, cancellationToken);

            if (memberRole == null)
            {
                throw new NotFoundException($"Missing role '{Role.MemberRoleName}' in database - cannot register user");
            }

            var user = new User()
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                Weight = request.Weight,
                RoleId = memberRole.Id
            };
            
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Sign in user with provided login and password.
        /// If valid, returns JWT Token
        /// </summary>
        public async Task<LoginUserResponse> LoginAsync(LoginUserRequest request, CancellationToken cancellationToken = default)
        {
            var user = await GetUserFromDbAsync(request.Email, cancellationToken);

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

        /// <summary>
        /// Change password for current user
        /// </summary>
        public async Task ChangePasswordAsync(ChangeUserPasswordRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext.Users
                .FirstAsync(u => u.Id == _contextService.UserId, cancellationToken);

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

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Get all users
        /// </summary>
        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var users = await _dbContext.Users
                .Include(u => u.Role)
                .Include(u => u.Activities)
                .Include(u => u.Achievements)
                .ToListAsync(cancellationToken);

            return users;
        }

        /// <summary>
        /// Delete user by Id. Admin-only
        /// </summary>
        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var user = await GetUserFromDbAsync(id, cancellationToken);

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Change user's role. Admin-only
        /// </summary>
        public async Task ChangeRoleAsync(int userId, string newRoleName, CancellationToken cancellationToken = default)
        {
            var user = await GetUserFromDbAsync(userId, cancellationToken);

            var newRole = await GetRoleFromDbAsync(newRoleName, cancellationToken);

            user.RoleId = newRole.Id;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task<User> GetUserFromDbAsync(int id, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext.Users
                .Include(u => u.Role)
                .Include(u => u.Activities)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

            if (user == null)
            {
                throw new NotFoundException($"There is no user with Id: {id}");
            }

            return user;
        }

        private async Task<User> GetUserFromDbAsync(string email, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext.Users
                .Include(u => u.Role)
                .Include(u => u.Activities)
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

            if (user == null)
            {
                throw new NotFoundException($"There is no user with email: {email}");
            }

            return user;
        }

        private async Task<Role> GetRoleFromDbAsync(string roleName, CancellationToken cancellationToken = default)
        {
            var role = await _dbContext.Roles
                    .FirstOrDefaultAsync(x => x.Name == roleName, cancellationToken);

            if (role == null)
            {
                throw new NotFoundException($"There is no role wit Name: {roleName}");
            }

            return role;
        }
    }
}
