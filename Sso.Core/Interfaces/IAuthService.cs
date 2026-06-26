using Sso.Core.DTOs;

namespace Sso.Core.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<LoginResponse> RefreshAsync(RefreshRequest request);
    Task LogoutAsync(Guid userId, string refreshToken);
}
