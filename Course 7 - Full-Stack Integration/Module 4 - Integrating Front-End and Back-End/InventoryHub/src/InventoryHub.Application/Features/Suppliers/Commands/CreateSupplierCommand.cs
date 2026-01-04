using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.Suppliers.Commands;

/// <summary>
/// Command to create a new supplier.
/// </summary>
public record CreateSupplierCommand : IRequest<Result<Guid>>
{
    public string Name { get; init; } = null!;
    public string? ContactPerson { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Address { get; init; }
}
