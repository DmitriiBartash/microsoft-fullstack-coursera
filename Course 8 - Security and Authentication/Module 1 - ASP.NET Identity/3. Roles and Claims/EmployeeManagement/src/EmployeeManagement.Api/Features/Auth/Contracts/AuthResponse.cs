namespace EmployeeManagement.Api.Features.Auth.Contracts;

public record AuthResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt);
