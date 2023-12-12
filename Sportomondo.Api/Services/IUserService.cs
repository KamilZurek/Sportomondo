using Sportomondo.Api.Models;
using Sportomondo.Api.Requests;
using Sportomondo.Api.Responses;

namespace Sportomondo.Api.Services
{
    public interface IUserService
    {
        Task RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken);
        Task<LoginUserResponse> LoginAsync(LoginUserRequest request, CancellationToken cancellationToken);
        Task ChangePasswordAsync(ChangeUserPasswordRequest request, CancellationToken cancellationToken);
        Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
        Task ChangeRoleAsync(int id, string newRoleName, CancellationToken cancellationToken);
    }
}
