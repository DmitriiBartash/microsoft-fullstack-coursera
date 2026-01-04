using InventoryHub.Client.Models;
using System.Net.Http.Json;

namespace InventoryHub.Client.Services;

/// <summary>
/// Service for managing suppliers
/// </summary>
public class SupplierService
{
    private readonly HttpClient _http;

    public SupplierService(HttpClient http)
    {
        _http = http ?? throw new ArgumentNullException(nameof(http));
    }

    /// <summary>
    /// Retrieves a paginated list of suppliers (client-side pagination)
    /// </summary>
    public async Task<ApiResponse<PaginatedList<SupplierDto>>?> GetSuppliersAsync(
        int page = 1,
        int pageSize = 10,
        string? search = null,
        bool? activeOnly = null)
    {
        var response = await _http.GetFromJsonAsync<ApiResponse<List<SupplierDto>>>("api/suppliers");

        if (response?.Success != true || response.Data == null)
        {
            return new ApiResponse<PaginatedList<SupplierDto>>
            {
                Success = false,
                Error = response?.Error ?? new ApiError { Code = "ERROR", Message = "Failed to load suppliers" }
            };
        }

        var filtered = response.Data.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLowerInvariant();
            filtered = filtered.Where(s =>
                s.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                (s.ContactPerson?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (s.Email?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (s.Phone?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (s.Address?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        if (activeOnly == true)
        {
            filtered = filtered.Where(s => s.IsActive);
        }

        var totalCount = filtered.Count();
        var items = filtered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new ApiResponse<PaginatedList<SupplierDto>>
        {
            Success = true,
            Data = new PaginatedList<SupplierDto>
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
    /// Retrieves all active suppliers without pagination
    /// </summary>
    public async Task<ApiResponse<List<SupplierDto>>?> GetAllSuppliersAsync()
    {
        return await _http.GetFromJsonAsync<ApiResponse<List<SupplierDto>>>("api/suppliers");
    }

    /// <summary>
    /// Retrieves a single supplier by ID
    /// </summary>
    public async Task<ApiResponse<SupplierDto>?> GetSupplierAsync(Guid id)
    {
        return await _http.GetFromJsonAsync<ApiResponse<SupplierDto>>($"api/suppliers/{id}");
    }

    /// <summary>
    /// Creates a new supplier
    /// </summary>
    public async Task<ApiResponse<Guid>?> CreateSupplierAsync(CreateSupplierRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/suppliers", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ApiResponse<Guid>>();
    }

    /// <summary>
    /// Updates an existing supplier
    /// </summary>
    public async Task<ApiResponse<bool>?> UpdateSupplierAsync(UpdateSupplierRequest request)
    {
        var response = await _http.PutAsJsonAsync($"api/suppliers/{request.Id}", request);

        if (!response.IsSuccessStatusCode)
        {
            return new ApiResponse<bool> { Success = false, Error = new ApiError { Code = "ERROR", Message = "Failed to update supplier" } };
        }

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        return new ApiResponse<bool> { Success = result?.Success ?? false, Data = result?.Success ?? false, Error = result?.Error };
    }

    /// <summary>
    /// Deletes a supplier
    /// </summary>
    public async Task<ApiResponse<bool>?> DeleteSupplierAsync(Guid id)
    {
        var response = await _http.DeleteAsync($"api/suppliers/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return new ApiResponse<bool> { Success = false, Error = new ApiError { Code = "ERROR", Message = "Failed to delete supplier" } };
        }

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        return new ApiResponse<bool> { Success = result?.Success ?? false, Data = result?.Success ?? false, Error = result?.Error };
    }
}
