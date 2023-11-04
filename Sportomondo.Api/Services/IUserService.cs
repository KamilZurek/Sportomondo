using Sportomondo.Api.Models;
using Sportomondo.Api.Requests;
using Sportomondo.Api.Responses;

namespace Sportomondo.Api.Services
{
    public interface IUserService
    {
        Task RegisterAsync(RegisterUserRequest request);
        Task<LoginUserResponse> LoginAsync(LoginUserRequest request);
        Task ChangePasswordAsync(ChangeUserPasswordRequest request);
        Task<IEnumerable<User>> GetAllAsync();
        Task DeleteAsync(int id);
        Task ChangeRoleAsync(int id, string newRoleName);
    }
}
