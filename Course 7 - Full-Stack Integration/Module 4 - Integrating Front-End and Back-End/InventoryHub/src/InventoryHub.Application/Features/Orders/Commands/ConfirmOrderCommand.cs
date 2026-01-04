using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.Orders.Commands;

/// <summary>
/// Command to confirm purchase order.
/// </summary>
public sealed record ConfirmOrderCommand(Guid OrderId) : IRequest<Result<Unit>>;
