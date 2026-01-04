using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.Clients.Commands;

/// <summary>
/// Command to create a new client.
/// </summary>
public record CreateClientCommand : IRequest<Result<Guid>>
{
    public string Name { get; init; } = null!;
    public string? ContactPerson { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Address { get; init; }
    public string? ShippingAddress { get; init; }
}
