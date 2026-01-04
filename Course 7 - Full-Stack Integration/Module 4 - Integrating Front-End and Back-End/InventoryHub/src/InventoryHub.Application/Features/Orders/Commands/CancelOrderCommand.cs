using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.Orders.Commands;

/// <summary>
/// Command to cancel purchase order.
/// </summary>
public sealed record CancelOrderCommand(Guid OrderId) : IRequest<Result<Unit>>;
