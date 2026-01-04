using InventoryHub.Client.Models;
using System.Net.Http.Json;

namespace InventoryHub.Client.Services;

/// <summary>
/// Service for managing inventory movements
/// </summary>
public class InventoryService
{
    private readonly HttpClient _http;

    public InventoryService(HttpClient http)
    {
        _http = http ?? throw new ArgumentNullException(nameof(http));
    }

    /// <summary>
    /// Retrieves a paginated list of inventory movements
    /// </summary>
    public async Task<ApiResponse<PaginatedList<InventoryMovementDto>>?> GetMovementsAsync(
        int page = 1,
        int pageSize = 10,
        Guid? productId = null,
        MovementType? type = null,
        DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        var query = $"api/inventory/movements?page={page}&pageSize={pageSize}";

        if (productId.HasValue)
            query += $"&productId={productId.Value}";

        if (type.HasValue)
            query += $"&type={type.Value}";

        if (fromDate.HasValue)
            query += $"&fromDate={fromDate.Value:yyyy-MM-dd}";

        if (toDate.HasValue)
            query += $"&toDate={toDate.Value:yyyy-MM-dd}";

        return await _http.GetFromJsonAsync<ApiResponse<PaginatedList<InventoryMovementDto>>>(query);
    }

    /// <summary>
    /// Retrieves inventory movements for a specific product
    /// </summary>
    public async Task<ApiResponse<PaginatedList<InventoryMovementDto>>?> GetProductMovementsAsync(
        Guid productId,
        int page = 1,
        int pageSize = 10)
    {
        return await GetMovementsAsync(page, pageSize, productId);
    }

    /// <summary>
    /// Records a new inventory movement
    /// </summary>
    public async Task<ApiResponse<Guid>?> RecordMovementAsync(RecordMovementRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/inventory/movements", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ApiResponse<Guid>>();
    }
}
