using InventoryHub.Client.Models;
using System.Net.Http.Json;

namespace InventoryHub.Client.Services;

/// <summary>
/// Service for managing products
/// </summary>
public class ProductService
{
    private readonly HttpClient _http;

    public ProductService(HttpClient http)
    {
        _http = http ?? throw new ArgumentNullException(nameof(http));
    }

    /// <summary>
    /// Retrieves a paginated list of products with optional filtering and sorting
    /// </summary>
    public async Task<ApiResponse<PaginatedList<ProductDto>>?> GetProductsAsync(
        int page = 1,
        int pageSize = 10,
        string? search = null,
        Guid? categoryId = null,
        Guid? supplierId = null,
        bool? lowStock = null,
        string? sortBy = null,
        string? sortDirection = null)
    {
        var query = $"api/products?page={page}&pageSize={pageSize}";

        if (!string.IsNullOrWhiteSpace(search))
            query += $"&search={Uri.EscapeDataString(search)}";

        if (categoryId.HasValue)
            query += $"&categoryId={categoryId.Value}";

        if (supplierId.HasValue)
            query += $"&supplierId={supplierId.Value}";

        if (lowStock.HasValue)
            query += $"&lowStock={lowStock.Value}";

        if (!string.IsNullOrWhiteSpace(sortBy))
            query += $"&sortBy={Uri.EscapeDataString(sortBy)}";

        if (!string.IsNullOrWhiteSpace(sortDirection))
            query += $"&sortDirection={Uri.EscapeDataString(sortDirection)}";

        return await _http.GetFromJsonAsync<ApiResponse<PaginatedList<ProductDto>>>(query);
    }

    /// <summary>
    /// Retrieves a single product by ID
    /// </summary>
    public async Task<ApiResponse<ProductDto>?> GetProductAsync(Guid id)
    {
        return await _http.GetFromJsonAsync<ApiResponse<ProductDto>>($"api/products/{id}");
    }

    /// <summary>
    /// Creates a new product
    /// </summary>
    public async Task<ApiResponse<Guid>?> CreateProductAsync(CreateProductDto request)
    {
        var response = await _http.PostAsJsonAsync("api/products", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ApiResponse<Guid>>();
    }

    /// <summary>
    /// Updates an existing product
    /// </summary>
    public async Task<ApiResponse<bool>?> UpdateProductAsync(Guid id, UpdateProductDto request)
    {
        var response = await _http.PutAsJsonAsync($"api/products/{id}", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
    }

    /// <summary>
    /// Deletes a product
    /// </summary>
    public async Task<ApiResponse<bool>?> DeleteProductAsync(Guid id)
    {
        var response = await _http.DeleteAsync($"api/products/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
    }
}
