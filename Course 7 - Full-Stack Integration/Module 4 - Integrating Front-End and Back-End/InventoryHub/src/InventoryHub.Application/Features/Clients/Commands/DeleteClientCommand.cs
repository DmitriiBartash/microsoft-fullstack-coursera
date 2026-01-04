using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.Clients.Commands;

/// <summary>
/// Command to delete a client.
/// </summary>
public record DeleteClientCommand(Guid Id) : IRequest<Result<Unit>>;
