using InventoryHub.Application.Common.Models;
using InventoryHub.Application.Features.Suppliers;
using InventoryHub.Application.Features.Suppliers.Commands;
using InventoryHub.Application.Features.Suppliers.Queries;
using MediatR;

namespace InventoryHub.API.Endpoints;

public static class SupplierEndpoints
{
    public static void MapSupplierEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/suppliers").WithTags("Suppliers");

        group.MapGet("/", GetSuppliers);
        group.MapGet("/{id:guid}", GetSupplierById);
        group.MapPost("/", CreateSupplier);
        group.MapPut("/{id:guid}", UpdateSupplier);
        group.MapDelete("/{id:guid}", DeleteSupplier);
    }

    private static async Task<IResult> GetSuppliers(
        ISender sender,
        string? search = null,
        bool? isActive = null,
        CancellationToken ct = default)
    {
        var query = new GetSuppliersQuery { SearchTerm = search, IsActive = isActive };
        var result = await sender.Send(query, ct);
        return result.IsSuccess
            ? Results.Ok(new { success = true, data = result.Data })
            : Results.BadRequest(new { success = false, error = result.Error });
    }

    private static async Task<IResult> GetSupplierById(
        Guid id,
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetSupplierByIdQuery(id), ct);
        return result.IsSuccess
            ? Results.Ok(new { success = true, data = result.Data })
            : Results.NotFound(new { success = false, error = result.Error });
    }

    private static async Task<IResult> CreateSupplier(
        CreateSupplierCommand command,
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(command, ct);
        return result.IsSuccess
            ? Results.Created($"/api/suppliers/{result.Data}", new { success = true, data = result.Data })
            : Results.BadRequest(new { success = false, error = result.Error });
    }

    private static async Task<IResult> UpdateSupplier(
        Guid id,
        UpdateSupplierCommand command,
        ISender sender,
        CancellationToken ct = default)
    {
        if (id != command.Id)
            return Results.BadRequest(new { success = false, error = new Error("INVALID_ID", "ID mismatch") });

        var result = await sender.Send(command, ct);
        return result.IsSuccess
            ? Results.Ok(new { success = true, data = result.Data })
            : Results.BadRequest(new { success = false, error = result.Error });
    }

    private static async Task<IResult> DeleteSupplier(
        Guid id,
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new DeleteSupplierCommand(id), ct);
        return result.IsSuccess
            ? Results.Ok(new { success = true, data = result.Data })
            : Results.NotFound(new { success = false, error = result.Error });
    }
}
