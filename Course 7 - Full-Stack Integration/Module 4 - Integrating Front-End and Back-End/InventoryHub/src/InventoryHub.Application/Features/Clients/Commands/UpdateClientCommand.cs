using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.Clients.Commands;

/// <summary>
/// Command to update an existing client.
/// </summary>
public record UpdateClientCommand : IRequest<Result<Unit>>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? ContactPerson { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Address { get; init; }
    public string? ShippingAddress { get; init; }
    public bool IsActive { get; init; }
}
