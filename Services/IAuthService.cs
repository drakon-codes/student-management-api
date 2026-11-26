using StudentManagementAPI.DTOs;

namespace StudentManagementAPI.Services
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    }
}
