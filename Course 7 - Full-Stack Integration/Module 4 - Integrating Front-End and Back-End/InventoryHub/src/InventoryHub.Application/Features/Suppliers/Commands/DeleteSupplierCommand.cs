using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.Suppliers.Commands;

/// <summary>
/// Command to soft delete (deactivate) a supplier.
/// </summary>
public record DeleteSupplierCommand(Guid Id) : IRequest<Result<Unit>>;
