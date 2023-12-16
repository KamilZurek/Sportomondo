using Sportomondo.Api.Models;
using Sportomondo.Api.Requests;
using Sportomondo.Api.Responses;

namespace Sportomondo.Api.Services
{
    public interface IUserService
    {
        Task RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default);
        Task<LoginUserResponse> LoginAsync(LoginUserRequest request, CancellationToken cancellationToken = default);
        Task ChangePasswordAsync(ChangeUserPasswordRequest request, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task ChangeRoleAsync(int id, string newRoleName, CancellationToken cancellationToken = default);
    }
}
