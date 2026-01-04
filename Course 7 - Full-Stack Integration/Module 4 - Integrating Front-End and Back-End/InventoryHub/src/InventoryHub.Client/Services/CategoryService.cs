using InventoryHub.Client.Models;
using System.Net.Http.Json;

namespace InventoryHub.Client.Services;

/// <summary>
/// Service for managing categories
/// </summary>
public class CategoryService
{
    private readonly HttpClient _http;

    public CategoryService(HttpClient http)
    {
        _http = http ?? throw new ArgumentNullException(nameof(http));
    }

    /// <summary>
    /// Retrieves a paginated list of categories (client-side pagination)
    /// </summary>
    public async Task<ApiResponse<PaginatedList<CategoryDto>>?> GetCategoriesAsync(
        int page = 1,
        int pageSize = 10,
        string? search = null)
    {
        var response = await _http.GetFromJsonAsync<ApiResponse<List<CategoryDto>>>("api/categories");

        if (response?.Success != true || response.Data == null)
        {
            return new ApiResponse<PaginatedList<CategoryDto>>
            {
                Success = false,
                Error = response?.Error ?? new ApiError { Code = "ERROR", Message = "Failed to load categories" }
            };
        }

        var filtered = response.Data.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchProductCount = int.TryParse(search.Trim(), out var count) ? count : (int?)null;

            filtered = filtered.Where(c =>
                c.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                (c.Description?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (searchProductCount.HasValue && c.ProductCount == searchProductCount.Value));
        }

        var totalCount = filtered.Count();
        var items = filtered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new ApiResponse<PaginatedList<CategoryDto>>
        {
            Success = true,
            Data = new PaginatedList<CategoryDto>
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
    /// Retrieves all categories without pagination
    /// </summary>
    public async Task<ApiResponse<List<CategoryDto>>?> GetAllCategoriesAsync()
    {
        return await _http.GetFromJsonAsync<ApiResponse<List<CategoryDto>>>("api/categories");
    }

    /// <summary>
    /// Retrieves a single category by ID
    /// </summary>
    public async Task<ApiResponse<CategoryDto>?> GetCategoryAsync(Guid id)
    {
        return await _http.GetFromJsonAsync<ApiResponse<CategoryDto>>($"api/categories/{id}");
    }

    /// <summary>
    /// Creates a new category
    /// </summary>
    public async Task<ApiResponse<Guid>?> CreateCategoryAsync(CreateCategoryDto request)
    {
        var response = await _http.PostAsJsonAsync("api/categories", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ApiResponse<Guid>>();
    }

    /// <summary>
    /// Updates an existing category
    /// </summary>
    public async Task<ApiResponse<bool>?> UpdateCategoryAsync(UpdateCategoryRequest request)
    {
        var response = await _http.PutAsJsonAsync($"api/categories/{request.Id}", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
    }

    /// <summary>
    /// Deletes a category
    /// </summary>
    public async Task<ApiResponse<bool>?> DeleteCategoryAsync(Guid id)
    {
        var response = await _http.DeleteAsync($"api/categories/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
    }
}
