using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.Suppliers.Queries;

/// <summary>
/// Query to retrieve a list of suppliers with optional search filter.
/// </summary>
public record GetSuppliersQuery : IRequest<Result<List<SupplierDto>>>
{
    public string? SearchTerm { get; init; }
    public bool? IsActive { get; init; }
}
