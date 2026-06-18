namespace JwtAuthCore.Models;

public record LoginRequest(string Username, string Password);

public record TokenResponse(string Token);

public record MessageResponse(string Message);
