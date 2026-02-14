using System.Net.Http.Json;
using Blazored.LocalStorage;
using EmployeeManagement.Client.Auth;
using EmployeeManagement.Client.Models;

namespace EmployeeManagement.Client.Services;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;
    private readonly JwtAuthenticationStateProvider _authStateProvider;

    public AuthService(
        HttpClient http,
        ILocalStorageService localStorage,
        JwtAuthenticationStateProvider authStateProvider)
    {
        _http = http;
        _localStorage = localStorage;
        _authStateProvider = authStateProvider;
    }

    public async Task<(bool Success, string? Error)> LoginAsync(LoginRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiError>();
            return (false, error?.Detail ?? "Login failed");
        }

        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        if (result is null) return (false, "Invalid response");

        await StoreTokens(result);
        _authStateProvider.MarkUserAsAuthenticated(result.AccessToken);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> RegisterAsync(RegisterRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/auth/register", request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiError>();
            return (false, error?.Detail ?? "Registration failed");
        }

        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        if (result is null) return (false, "Invalid response");

        await StoreTokens(result);
        _authStateProvider.MarkUserAsAuthenticated(result.AccessToken);
        return (true, null);
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync("accessToken");
        await _localStorage.RemoveItemAsync("refreshToken");
        _authStateProvider.MarkUserAsLoggedOut();
    }

    private async Task StoreTokens(AuthResponse auth)
    {
        await _localStorage.SetItemAsStringAsync("accessToken", auth.AccessToken);
        await _localStorage.SetItemAsStringAsync("refreshToken", auth.RefreshToken);
    }
}
