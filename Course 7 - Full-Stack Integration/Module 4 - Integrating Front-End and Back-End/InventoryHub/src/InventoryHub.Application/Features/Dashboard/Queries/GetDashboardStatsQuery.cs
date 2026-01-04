using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.Dashboard.Queries;

/// <summary>
/// Query to retrieve dashboard statistics
/// </summary>
public sealed record GetDashboardStatsQuery : IRequest<Result<DashboardStatsDto>>;
