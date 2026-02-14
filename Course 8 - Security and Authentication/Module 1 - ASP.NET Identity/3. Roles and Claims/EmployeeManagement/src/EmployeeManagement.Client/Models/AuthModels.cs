namespace EmployeeManagement.Client.Models;

public record LoginRequest(string Email, string Password);

public record RegisterRequest(string Email, string Password, string FirstName, string LastName);

public record RefreshTokenRequest(string AccessToken, string RefreshToken);

public record AuthResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt);
