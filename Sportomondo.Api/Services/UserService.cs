using Sportomondo.Api.Context;
using Sportomondo.Api.Models;
using Sportomondo.Api.Requests;

namespace Sportomondo.Api.Services
{
    public class UserService : IUserService
    {
        private readonly SportomondoDbContext _dbContext;

        public UserService(SportomondoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task RegisterAsync(RegisterUserRequest request)
        {
            
        }

        public async Task<string> LoginAsync(LoginUserRequest request)
        {
            
        }

        public async Task ChangePasswordAsync(ChangeUserPasswordRequest request)
        {
            
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {

        }

        public async Task DeleteAsync(int id)
        {

        }

        public async Task ChangeRoleAsync(int id, string newRoleName)
        {
            
        }
    }
}
