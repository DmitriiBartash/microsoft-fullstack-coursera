using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.SalesOrders.Commands;

/// <summary>
/// Command to confirm a sales order (Draft -> Confirmed).
/// </summary>
public sealed record ConfirmSalesOrderCommand(Guid OrderId) : IRequest<Result<Unit>>;
