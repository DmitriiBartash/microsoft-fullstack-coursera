using System.Net.Http.Json;
using EmployeeManagement.Client.Models;

namespace EmployeeManagement.Client.Services;

public class EmployeeService
{
    private readonly HttpClient _http;

    public EmployeeService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<EmployeeResponse>> GetEmployeesAsync()
    {
        var response = await _http.GetAsync("api/employees");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<EmployeeResponse>>() ?? [];
    }

    public async Task<EmployeeResponse?> GetProfileAsync()
    {
        var response = await _http.GetAsync("api/employees/me");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<EmployeeResponse>();
    }
}
