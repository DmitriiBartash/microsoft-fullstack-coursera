using InventoryHub.Application.Features.Inventory.Commands.RecordMovement;
using InventoryHub.Application.Features.Inventory.Queries.GetMovements;
using InventoryHub.Domain.Enums;
using MediatR;

namespace InventoryHub.API.Endpoints;

public static class InventoryEndpoints
{
    public static void MapInventoryEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/inventory").WithTags("Inventory");

        group.MapGet("/movements", GetMovements);
        group.MapGet("/products/{productId:guid}/movements", GetProductMovements);
        group.MapPost("/movements", RecordMovement);
    }

    private static async Task<IResult> GetMovements(
        ISender sender,
        int page = 1,
        int pageSize = 20,
        Guid? productId = null,
        MovementType? type = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken ct = default)
    {
        var query = new GetMovementsQuery
        {
            Page = page,
            PageSize = pageSize,
            ProductId = productId,
            Type = type,
            FromDate = fromDate,
            ToDate = toDate
        };

        var result = await sender.Send(query, ct);
        return result.IsSuccess
            ? Results.Ok(new { success = true, data = result.Data })
            : Results.BadRequest(new { success = false, error = result.Error });
    }

    private static async Task<IResult> GetProductMovements(
        Guid productId,
        ISender sender,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = new GetMovementsQuery
        {
            Page = page,
            PageSize = pageSize,
            ProductId = productId
        };

        var result = await sender.Send(query, ct);
        return result.IsSuccess
            ? Results.Ok(new { success = true, data = result.Data })
            : Results.BadRequest(new { success = false, error = result.Error });
    }

    private static async Task<IResult> RecordMovement(
        RecordMovementCommand command,
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(command, ct);
        return result.IsSuccess
            ? Results.Created($"/api/inventory/movements/{result.Data}", new { success = true, data = result.Data })
            : Results.BadRequest(new { success = false, error = result.Error });
    }
}
