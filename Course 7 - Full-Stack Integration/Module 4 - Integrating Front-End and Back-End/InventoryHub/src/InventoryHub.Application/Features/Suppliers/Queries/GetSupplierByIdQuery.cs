using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.Suppliers.Queries;

/// <summary>
/// Query to retrieve a single supplier by ID.
/// </summary>
public record GetSupplierByIdQuery(Guid Id) : IRequest<Result<SupplierDto>>;
