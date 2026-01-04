using InventoryHub.Application.Features.SalesOrders.Commands;
using InventoryHub.Application.Features.SalesOrders.Queries;
using InventoryHub.Domain.Enums;
using MediatR;

namespace InventoryHub.API.Endpoints;

public static class SalesOrderEndpoints
{
    public static void MapSalesOrderEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/sales-orders").WithTags("Sales Orders");

        group.MapGet("/", GetSalesOrders);
        group.MapGet("/{id:guid}", GetSalesOrderById);
        group.MapPost("/", CreateSalesOrder);
        group.MapPost("/{id:guid}/confirm", ConfirmSalesOrder);
        group.MapPost("/{id:guid}/ship", ShipSalesOrder);
        group.MapPost("/{id:guid}/deliver", DeliverSalesOrder);
        group.MapPost("/{id:guid}/cancel", CancelSalesOrder);
        group.MapPost("/{id:guid}/status", ChangeSalesOrderStatus);
        group.MapDelete("/{id:guid}", DeleteSalesOrder);
    }

    private static async Task<IResult> GetSalesOrders(
        ISender sender,
        int page = 1,
        int pageSize = 10,
        SalesOrderStatus? status = null,
        Guid? clientId = null,
        string? search = null,
        CancellationToken ct = default)
    {
        var query = new GetSalesOrdersQuery
        {
            Page = page,
            PageSize = pageSize,
            Status = status?.ToString(),
            ClientId = clientId,
            Search = search
        };

        var result = await sender.Send(query, ct);
        return result.IsSuccess
            ? Results.Ok(new { success = true, data = result.Data })
            : Results.BadRequest(new { success = false, error = result.Error });
    }

    private static async Task<IResult> GetSalesOrderById(
        Guid id,
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetSalesOrderByIdQuery(id), ct);
        return result.IsSuccess
            ? Results.Ok(new { success = true, data = result.Data })
            : Results.NotFound(new { success = false, error = result.Error });
    }

    private static async Task<IResult> CreateSalesOrder(
        CreateSalesOrderCommand command,
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(command, ct);
        return result.IsSuccess
            ? Results.Created($"/api/sales-orders/{result.Data}", new { success = true, data = result.Data })
            : Results.BadRequest(new { success = false, error = result.Error });
    }

    private static async Task<IResult> ConfirmSalesOrder(
        Guid id,
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new ConfirmSalesOrderCommand(id), ct);
        return result.IsSuccess
            ? Results.Ok(new { success = true, data = true })
            : Results.BadRequest(new { success = false, error = result.Error });
    }

    private static async Task<IResult> ShipSalesOrder(
        Guid id,
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new ShipSalesOrderCommand(id), ct);
        return result.IsSuccess
            ? Results.Ok(new { success = true, data = true })
            : Results.BadRequest(new { success = false, error = result.Error });
    }

    private static async Task<IResult> DeliverSalesOrder(
        Guid id,
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new DeliverSalesOrderCommand(id), ct);
        return result.IsSuccess
            ? Results.Ok(new { success = true, data = true })
            : Results.BadRequest(new { success = false, error = result.Error });
    }

    private static async Task<IResult> CancelSalesOrder(
        Guid id,
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new CancelSalesOrderCommand(id), ct);
        return result.IsSuccess
            ? Results.Ok(new { success = true, data = true })
            : Results.BadRequest(new { success = false, error = result.Error });
    }

    private static async Task<IResult> DeleteSalesOrder(
        Guid id,
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new DeleteSalesOrderCommand(id), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new { success = false, error = result.Error });
    }

    private static async Task<IResult> ChangeSalesOrderStatus(
        Guid id,
        ChangeSalesOrderStatusRequest request,
        ISender sender,
        CancellationToken ct = default)
    {
        if (!Enum.TryParse<SalesOrderStatus>(request.Status, true, out var status))
        {
            return Results.BadRequest(new { success = false, error = new { code = "INVALID_STATUS", message = $"Invalid status: {request.Status}" } });
        }

        var result = await sender.Send(new ChangeSalesOrderStatusCommand(id, status), ct);
        return result.IsSuccess
            ? Results.Ok(new { success = true, data = true })
            : Results.BadRequest(new { success = false, error = result.Error });
    }
}

public record ChangeSalesOrderStatusRequest(string Status);
