using InventoryHub.Client.Models;
using System.Net.Http.Json;

namespace InventoryHub.Client.Services;

/// <summary>
/// Service for managing orders
/// </summary>
public class OrderService
{
    private readonly HttpClient _http;

    public OrderService(HttpClient http)
    {
        _http = http ?? throw new ArgumentNullException(nameof(http));
    }

    /// <summary>
    /// Retrieves a paginated list of orders
    /// </summary>
    public async Task<ApiResponse<PaginatedList<OrderDto>>?> GetOrdersAsync(
        int page = 1,
        int pageSize = 10,
        string? status = null,
        Guid? supplierId = null,
        string? search = null)
    {
        var query = $"api/orders?page={page}&pageSize={pageSize}";

        if (!string.IsNullOrEmpty(status))
            query += $"&status={status}";

        if (supplierId.HasValue)
            query += $"&supplierId={supplierId.Value}";

        if (!string.IsNullOrEmpty(search))
            query += $"&search={Uri.EscapeDataString(search)}";

        return await _http.GetFromJsonAsync<ApiResponse<PaginatedList<OrderDto>>>(query);
    }

    /// <summary>
    /// Retrieves a single order by ID
    /// </summary>
    public async Task<ApiResponse<OrderDto>?> GetOrderAsync(Guid id)
    {
        return await _http.GetFromJsonAsync<ApiResponse<OrderDto>>($"api/orders/{id}");
    }

    /// <summary>
    /// Creates a new order in draft status
    /// </summary>
    public async Task<ApiResponse<Guid>?> CreateOrderAsync(CreateOrderRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/orders", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ApiResponse<Guid>>();
    }

    /// <summary>
    /// Submits a draft order to supplier
    /// </summary>
    public async Task<ApiResponse<bool>?> SubmitOrderAsync(Guid id)
    {
        var response = await _http.PostAsync($"api/orders/{id}/submit", null);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
    }

    /// <summary>
    /// Confirms an order from supplier
    /// </summary>
    public async Task<ApiResponse<bool>?> ConfirmOrderAsync(Guid id)
    {
        var response = await _http.PostAsync($"api/orders/{id}/confirm", null);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
    }

    /// <summary>
    /// Marks an order as shipped
    /// </summary>
    public async Task<ApiResponse<bool>?> ShipOrderAsync(Guid id)
    {
        var response = await _http.PostAsync($"api/orders/{id}/ship", null);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
    }

    /// <summary>
    /// Marks an order as received and updates inventory
    /// </summary>
    public async Task<ApiResponse<bool>?> ReceiveOrderAsync(Guid id)
    {
        var response = await _http.PostAsync($"api/orders/{id}/receive", null);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
    }

    /// <summary>
    /// Cancels an order
    /// </summary>
    public async Task<ApiResponse<bool>?> CancelOrderAsync(Guid id)
    {
        var response = await _http.PostAsync($"api/orders/{id}/cancel", null);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
    }

    /// <summary>
    /// Deletes a draft order
    /// </summary>
    public async Task<ApiResponse<bool>?> DeleteOrderAsync(Guid id)
    {
        var response = await _http.DeleteAsync($"api/orders/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
    }

    /// <summary>
    /// Changes order status directly
    /// </summary>
    public async Task<ApiResponse<bool>?> ChangeStatusAsync(Guid id, string newStatus)
    {
        var response = await _http.PutAsJsonAsync($"api/orders/{id}/status", new { Status = newStatus });
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
    }
}
