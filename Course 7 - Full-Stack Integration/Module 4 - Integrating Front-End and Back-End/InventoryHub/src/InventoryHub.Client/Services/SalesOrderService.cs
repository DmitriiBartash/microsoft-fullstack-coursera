using InventoryHub.Client.Models;
using System.Net.Http.Json;

namespace InventoryHub.Client.Services;

/// <summary>
/// Service for managing sales orders
/// </summary>
public class SalesOrderService
{
    private readonly HttpClient _http;

    public SalesOrderService(HttpClient http)
    {
        _http = http ?? throw new ArgumentNullException(nameof(http));
    }

    /// <summary>
    /// Retrieves a paginated list of sales orders
    /// </summary>
    public async Task<ApiResponse<PaginatedList<SalesOrderDto>>?> GetSalesOrdersAsync(
        int page = 1,
        int pageSize = 10,
        string? status = null,
        Guid? clientId = null,
        string? search = null)
    {
        var query = $"api/sales-orders?page={page}&pageSize={pageSize}";

        if (!string.IsNullOrEmpty(status))
            query += $"&status={status}";

        if (clientId.HasValue)
            query += $"&clientId={clientId.Value}";

        if (!string.IsNullOrEmpty(search))
            query += $"&search={Uri.EscapeDataString(search)}";

        return await _http.GetFromJsonAsync<ApiResponse<PaginatedList<SalesOrderDto>>>(query);
    }

    /// <summary>
    /// Retrieves a single sales order by ID
    /// </summary>
    public async Task<ApiResponse<SalesOrderDto>?> GetSalesOrderAsync(Guid id)
    {
        return await _http.GetFromJsonAsync<ApiResponse<SalesOrderDto>>($"api/sales-orders/{id}");
    }

    /// <summary>
    /// Creates a new sales order in draft status
    /// </summary>
    public async Task<ApiResponse<Guid>?> CreateSalesOrderAsync(CreateSalesOrderRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/sales-orders", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ApiResponse<Guid>>();
    }

    /// <summary>
    /// Confirms a draft sales order
    /// </summary>
    public async Task<ApiResponse<bool>?> ConfirmSalesOrderAsync(Guid id)
    {
        var response = await _http.PostAsync($"api/sales-orders/{id}/confirm", null);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
    }

    /// <summary>
    /// Ships a confirmed sales order (triggers Stock Out)
    /// </summary>
    public async Task<ApiResponse<bool>?> ShipSalesOrderAsync(Guid id)
    {
        var response = await _http.PostAsync($"api/sales-orders/{id}/ship", null);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
    }

    /// <summary>
    /// Marks a shipped sales order as delivered
    /// </summary>
    public async Task<ApiResponse<bool>?> DeliverSalesOrderAsync(Guid id)
    {
        var response = await _http.PostAsync($"api/sales-orders/{id}/deliver", null);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
    }

    /// <summary>
    /// Cancels a sales order (not shipped or delivered)
    /// </summary>
    public async Task<ApiResponse<bool>?> CancelSalesOrderAsync(Guid id)
    {
        var response = await _http.PostAsync($"api/sales-orders/{id}/cancel", null);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
    }

    /// <summary>
    /// Deletes a draft sales order
    /// </summary>
    public async Task<ApiResponse<bool>?> DeleteSalesOrderAsync(Guid id)
    {
        var response = await _http.DeleteAsync($"api/sales-orders/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
    }

    /// <summary>
    /// Changes sales order status directly (admin override)
    /// </summary>
    public async Task<ApiResponse<bool>?> ChangeStatusAsync(Guid id, string status)
    {
        var response = await _http.PostAsJsonAsync($"api/sales-orders/{id}/status", new { Status = status });
        return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
    }
}
