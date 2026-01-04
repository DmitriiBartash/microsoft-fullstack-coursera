using InventoryHub.Client.Models;
using System.Net.Http.Json;

namespace InventoryHub.Client.Services;

/// <summary>
/// Service for managing clients
/// </summary>
public class ClientService
{
    private readonly HttpClient _http;

    public ClientService(HttpClient http)
    {
        _http = http ?? throw new ArgumentNullException(nameof(http));
    }

    /// <summary>
    /// Retrieves a paginated list of clients (client-side pagination)
    /// </summary>
    public async Task<ApiResponse<PaginatedList<ClientDto>>?> GetClientsAsync(
        int page = 1,
        int pageSize = 10,
        string? search = null,
        bool? activeOnly = null)
    {
        var response = await _http.GetFromJsonAsync<ApiResponse<List<ClientDto>>>("api/clients");

        if (response?.Success != true || response.Data == null)
        {
            return new ApiResponse<PaginatedList<ClientDto>>
            {
                Success = false,
                Error = response?.Error ?? new ApiError { Code = "ERROR", Message = "Failed to load clients" }
            };
        }

        var filtered = response.Data.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            filtered = filtered.Where(c =>
                c.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                (c.ContactPerson?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (c.Email?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (c.Phone?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (c.Address?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        if (activeOnly == true)
        {
            filtered = filtered.Where(c => c.IsActive);
        }

        var totalCount = filtered.Count();
        var items = filtered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new ApiResponse<PaginatedList<ClientDto>>
        {
            Success = true,
            Data = new PaginatedList<ClientDto>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                HasPreviousPage = page > 1,
                HasNextPage = page * pageSize < totalCount
            }
        };
    }

    /// <summary>
    /// Retrieves all active clients without pagination
    /// </summary>
    public async Task<ApiResponse<List<ClientDto>>?> GetAllClientsAsync()
    {
        return await _http.GetFromJsonAsync<ApiResponse<List<ClientDto>>>("api/clients?isActive=true");
    }

    /// <summary>
    /// Retrieves a single client by ID
    /// </summary>
    public async Task<ApiResponse<ClientDto>?> GetClientAsync(Guid id)
    {
        return await _http.GetFromJsonAsync<ApiResponse<ClientDto>>($"api/clients/{id}");
    }

    /// <summary>
    /// Creates a new client
    /// </summary>
    public async Task<ApiResponse<Guid>?> CreateClientAsync(CreateClientRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/clients", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ApiResponse<Guid>>();
    }

    /// <summary>
    /// Updates an existing client
    /// </summary>
    public async Task<ApiResponse<bool>?> UpdateClientAsync(UpdateClientRequest request)
    {
        var response = await _http.PutAsJsonAsync($"api/clients/{request.Id}", request);

        if (!response.IsSuccessStatusCode)
        {
            return new ApiResponse<bool> { Success = false, Error = new ApiError { Code = "ERROR", Message = "Failed to update client" } };
        }

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        return new ApiResponse<bool> { Success = result?.Success ?? false, Data = result?.Success ?? false, Error = result?.Error };
    }

    /// <summary>
    /// Deletes a client
    /// </summary>
    public async Task<ApiResponse<bool>?> DeleteClientAsync(Guid id)
    {
        var response = await _http.DeleteAsync($"api/clients/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return new ApiResponse<bool> { Success = false, Error = new ApiError { Code = "ERROR", Message = "Failed to delete client" } };
        }

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        return new ApiResponse<bool> { Success = result?.Success ?? false, Data = result?.Success ?? false, Error = result?.Error };
    }
}
