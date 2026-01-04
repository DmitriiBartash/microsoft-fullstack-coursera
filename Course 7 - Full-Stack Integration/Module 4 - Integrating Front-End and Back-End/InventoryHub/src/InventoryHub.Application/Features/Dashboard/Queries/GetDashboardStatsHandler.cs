using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using InventoryHub.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.Dashboard.Queries;

/// <summary>
/// Handler for retrieving dashboard statistics
/// </summary>
public sealed class GetDashboardStatsHandler : IRequestHandler<GetDashboardStatsQuery, Result<DashboardStatsDto>>
{
    private readonly IApplicationDbContext _context;

    public GetDashboardStatsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<DashboardStatsDto>> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var totalProducts = await _context.Products.CountAsync(cancellationToken);
        var activeProducts = await _context.Products.CountAsync(p => p.IsActive, cancellationToken);
        var lowStockCount = await _context.Products.CountAsync(p => p.CurrentStock <= p.MinimumStock, cancellationToken);
        var outOfStockCount = await _context.Products.CountAsync(p => p.CurrentStock == 0, cancellationToken);
        var totalCategories = await _context.Categories.CountAsync(cancellationToken);
        var totalSuppliers = await _context.Suppliers.CountAsync(cancellationToken);

        var pendingOrders = await _context.PurchaseOrders
            .CountAsync(o => o.Status == OrderStatus.Draft || o.Status == OrderStatus.Submitted, cancellationToken);

        var totalInventoryValue = await _context.Products
            .Where(p => p.IsActive)
            .SumAsync(p => p.CurrentStock * p.Price, cancellationToken);

        var stats = new DashboardStatsDto
        {
            TotalProducts = totalProducts,
            ActiveProducts = activeProducts,
            LowStockCount = lowStockCount,
            OutOfStockCount = outOfStockCount,
            TotalCategories = totalCategories,
            TotalSuppliers = totalSuppliers,
            PendingOrders = pendingOrders,
            TotalInventoryValue = totalInventoryValue
        };

        return Result<DashboardStatsDto>.Success(stats);
    }
}
