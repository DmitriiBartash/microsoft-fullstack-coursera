using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.Orders.Commands;

/// <summary>
/// Command to delete draft purchase order.
/// </summary>
public sealed record DeleteOrderCommand(Guid OrderId) : IRequest<Result<Unit>>;
