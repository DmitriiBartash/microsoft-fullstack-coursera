using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.Orders.Commands;

/// <summary>
/// Command to mark purchase order as shipped.
/// </summary>
public sealed record ShipOrderCommand(Guid OrderId) : IRequest<Result<Unit>>;
