using InventoryHub.Application.Features.Dashboard.Queries;
using MediatR;

namespace InventoryHub.API.Endpoints;

public static class DashboardEndpoints
{
    public static void MapDashboardEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/dashboard").WithTags("Dashboard");

        group.MapGet("/stats", GetStats);
        group.MapGet("/low-stock", GetLowStockProducts);
        group.MapGet("/recent-movements", GetRecentMovements);
    }

    private static async Task<IResult> GetStats(
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetDashboardStatsQuery(), ct);
        return result.IsSuccess
            ? Results.Ok(new { success = true, data = result.Data })
            : Results.BadRequest(new { success = false, error = result.Error });
    }

    private static async Task<IResult> GetLowStockProducts(
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetLowStockProductsQuery(), ct);
        return result.IsSuccess
            ? Results.Ok(new { success = true, data = result.Data })
            : Results.BadRequest(new { success = false, error = result.Error });
    }

    private static async Task<IResult> GetRecentMovements(
        ISender sender,
        int limit = 10,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetRecentMovementsQuery { Limit = limit }, ct);
        return result.IsSuccess
            ? Results.Ok(new { success = true, data = result.Data })
            : Results.BadRequest(new { success = false, error = result.Error });
    }
}
