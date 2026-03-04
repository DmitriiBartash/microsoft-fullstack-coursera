using SecureNotes.Api.Contracts.Auth;

namespace SecureNotes.Api.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RefreshTokenAsync(RefreshRequest request);
    Task LogoutAsync(string userId);
}
