using InventoryHub.Application.Common.Models;
using InventoryHub.Application.Features.Orders.Commands;
using InventoryHub.Application.Features.Orders.Queries;
using InventoryHub.Domain.Enums;
using MediatR;

namespace InventoryHub.API.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/orders").WithTags("Orders");

        group.MapGet("/", GetOrders);
        group.MapGet("/{id:guid}", GetOrderById);
        group.MapPost("/", CreateOrder);
        group.MapPost("/{id:guid}/submit", SubmitOrder);
        group.MapPost("/{id:guid}/confirm", ConfirmOrder);
        group.MapPost("/{id:guid}/ship", ShipOrder);
        group.MapPost("/{id:guid}/receive", ReceiveOrder);
        group.MapPost("/{id:guid}/cancel", CancelOrder);
        group.MapPut("/{id:guid}/status", ChangeOrderStatus);
        group.MapDelete("/{id:guid}", DeleteOrder);
    }

    private static async Task<IResult> GetOrders(
        ISender sender,
        int page = 1,
        int pageSize = 10,
        OrderStatus? status = null,
        Guid? supplierId = null,
        string? search = null,
        CancellationToken ct = default)
    {
        var query = new GetOrdersQuery(
            Page: page,
            PageSize: pageSize,
            Status: status?.ToString(),
            SupplierId: supplierId,
            Search: search);

        var result = await sender.Send(query, ct);
        return result.IsSuccess
            ? Results.Ok(new { success = true, data = result.Data })
            : Results.BadRequest(new { success = false, error = result.Error });
    }

    private static async Task<IResult> GetOrderById(
        Guid id,
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetOrderByIdQuery(id), ct);
        return result.IsSuccess
            ? Results.Ok(new { success = true, data = result.Data })
            : Results.NotFound(new { success = false, error = result.Error });
    }

    private static async Task<IResult> CreateOrder(
        CreateOrderCommand command,
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(command, ct);
        return result.IsSuccess
            ? Results.Created($"/api/orders/{result.Data}", new { success = true, data = result.Data })
            : Results.BadRequest(new { success = false, error = result.Error });
    }

    private static async Task<IResult> SubmitOrder(
        Guid id,
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new SubmitOrderCommand(id), ct);
        return result.IsSuccess
            ? Results.Ok(new { success = true, data = true })
            : Results.BadRequest(new { success = false, error = result.Error });
    }

    private static async Task<IResult> ConfirmOrder(
        Guid id,
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new ConfirmOrderCommand(id), ct);
        return result.IsSuccess
            ? Results.Ok(new { success = true, data = true })
            : Results.BadRequest(new { success = false, error = result.Error });
    }

    private static async Task<IResult> ShipOrder(
        Guid id,
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new ShipOrderCommand(id), ct);
        return result.IsSuccess
            ? Results.Ok(new { success = true, data = true })
            : Results.BadRequest(new { success = false, error = result.Error });
    }

    private static async Task<IResult> ReceiveOrder(
        Guid id,
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new ReceiveOrderCommand(id, []), ct);
        return result.IsSuccess
            ? Results.Ok(new { success = true, data = true })
            : Results.BadRequest(new { success = false, error = result.Error });
    }

    private static async Task<IResult> CancelOrder(
        Guid id,
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new CancelOrderCommand(id), ct);
        return result.IsSuccess
            ? Results.Ok(new { success = true, data = true })
            : Results.BadRequest(new { success = false, error = result.Error });
    }

    private static async Task<IResult> DeleteOrder(
        Guid id,
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new DeleteOrderCommand(id), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new { success = false, error = result.Error });
    }

    private static async Task<IResult> ChangeOrderStatus(
        Guid id,
        ChangeStatusRequest request,
        ISender sender,
        CancellationToken ct = default)
    {
        if (!Enum.TryParse<OrderStatus>(request.Status, true, out var status))
        {
            return Results.BadRequest(new { success = false, error = new { code = "INVALID_STATUS", message = $"Invalid status: {request.Status}" } });
        }

        var result = await sender.Send(new ChangeOrderStatusCommand(id, status), ct);
        return result.IsSuccess
            ? Results.Ok(new { success = true, data = true })
            : Results.BadRequest(new { success = false, error = result.Error });
    }
}

public record ChangeStatusRequest(string Status);
