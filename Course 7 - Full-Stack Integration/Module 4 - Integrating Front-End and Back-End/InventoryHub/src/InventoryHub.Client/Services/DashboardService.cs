using InventoryHub.Client.Models;
using System.Net.Http.Json;

namespace InventoryHub.Client.Services;

/// <summary>
/// Service for retrieving dashboard statistics and insights
/// </summary>
public class DashboardService
{
    private readonly HttpClient _http;

    public DashboardService(HttpClient http)
    {
        _http = http ?? throw new ArgumentNullException(nameof(http));
    }

    /// <summary>
    /// Retrieves comprehensive dashboard statistics
    /// </summary>
    public async Task<ApiResponse<DashboardStatsDto>?> GetStatsAsync()
    {
        return await _http.GetFromJsonAsync<ApiResponse<DashboardStatsDto>>("api/dashboard/stats");
    }

    /// <summary>
    /// Retrieves products with low stock levels
    /// </summary>
    public async Task<ApiResponse<List<LowStockProductDto>>?> GetLowStockProductsAsync(int count = 10)
    {
        return await _http.GetFromJsonAsync<ApiResponse<List<LowStockProductDto>>>(
            $"api/dashboard/low-stock?count={count}");
    }

    /// <summary>
    /// Retrieves recent inventory movements
    /// </summary>
    public async Task<ApiResponse<List<RecentMovementDto>>?> GetRecentMovementsAsync(int count = 10)
    {
        return await _http.GetFromJsonAsync<ApiResponse<List<RecentMovementDto>>>(
            $"api/dashboard/recent-movements?limit={count}");
    }
}
