using System.Net.Http.Json;
using EmployeeManagement.Client.Models;

namespace EmployeeManagement.Client.Services;

public class UserManagementService
{
    private readonly HttpClient _http;

    public UserManagementService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<UserResponse>> GetAllUsersAsync()
    {
        var response = await _http.GetAsync("api/users");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<UserResponse>>() ?? [];
    }

    public async Task<UserResponse?> AssignRoleAsync(string userId, string role)
    {
        var response = await _http.PostAsJsonAsync($"api/users/{userId}/roles", new AssignRoleRequest(role));
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserResponse>();
    }

    public async Task<UserResponse?> RemoveRoleAsync(string userId, string role)
    {
        var response = await _http.DeleteAsync($"api/users/{userId}/roles/{role}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserResponse>();
    }

    public async Task<UserResponse?> AddClaimAsync(string userId, string claimType, string claimValue)
    {
        var response = await _http.PostAsJsonAsync($"api/users/{userId}/claims",
            new AddClaimRequest(claimType, claimValue));
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserResponse>();
    }

    public async Task<UserResponse?> RemoveClaimAsync(string userId, string claimType)
    {
        var response = await _http.DeleteAsync($"api/users/{userId}/claims/{claimType}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserResponse>();
    }
}
