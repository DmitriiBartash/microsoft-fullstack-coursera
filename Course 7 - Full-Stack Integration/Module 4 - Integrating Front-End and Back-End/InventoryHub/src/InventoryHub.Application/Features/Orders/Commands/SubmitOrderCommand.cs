using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.Orders.Commands;

/// <summary>
/// Command to submit purchase order.
/// </summary>
public sealed record SubmitOrderCommand(Guid OrderId) : IRequest<Result<Unit>>;
