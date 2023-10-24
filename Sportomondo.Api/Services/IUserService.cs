using Sportomondo.Api.Models;
using Sportomondo.Api.Requests;

namespace Sportomondo.Api.Services
{
    public interface IUserService
    {
        Task RegisterAsync(RegisterUserRequest request);
        Task<string> LoginAsync(LoginUserRequest request);
        Task ChangePasswordAsync(ChangeUserPasswordRequest request);
        Task<IEnumerable<User>> GetAllAsync();
        Task DeleteAsync(int id);
        Task ChangeRoleAsync(int id, string newRoleName);
    }
}
