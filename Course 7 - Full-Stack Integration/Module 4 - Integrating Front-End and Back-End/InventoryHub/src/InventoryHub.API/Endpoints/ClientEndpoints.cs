using InventoryHub.Application.Common.Models;
using InventoryHub.Application.Features.Clients;
using InventoryHub.Application.Features.Clients.Commands;
using InventoryHub.Application.Features.Clients.Queries;
using MediatR;

namespace InventoryHub.API.Endpoints;

public static class ClientEndpoints
{
    public static void MapClientEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/clients").WithTags("Clients");

        group.MapGet("/", GetClients);
        group.MapGet("/{id:guid}", GetClientById);
        group.MapPost("/", CreateClient);
        group.MapPut("/{id:guid}", UpdateClient);
        group.MapDelete("/{id:guid}", DeleteClient);
    }

    private static async Task<IResult> GetClients(
        ISender sender,
        string? search = null,
        bool? isActive = null,
        CancellationToken ct = default)
    {
        var query = new GetClientsQuery { SearchTerm = search, IsActive = isActive };
        var result = await sender.Send(query, ct);
        return result.IsSuccess
            ? Results.Ok(new { success = true, data = result.Data })
            : Results.BadRequest(new { success = false, error = result.Error });
    }

    private static async Task<IResult> GetClientById(
        Guid id,
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetClientByIdQuery(id), ct);
        return result.IsSuccess
            ? Results.Ok(new { success = true, data = result.Data })
            : Results.NotFound(new { success = false, error = result.Error });
    }

    private static async Task<IResult> CreateClient(
        CreateClientCommand command,
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(command, ct);
        return result.IsSuccess
            ? Results.Created($"/api/clients/{result.Data}", new { success = true, data = result.Data })
            : Results.BadRequest(new { success = false, error = result.Error });
    }

    private static async Task<IResult> UpdateClient(
        Guid id,
        UpdateClientCommand command,
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

    private static async Task<IResult> DeleteClient(
        Guid id,
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new DeleteClientCommand(id), ct);
        return result.IsSuccess
            ? Results.Ok(new { success = true, data = result.Data })
            : Results.NotFound(new { success = false, error = result.Error });
    }
}
