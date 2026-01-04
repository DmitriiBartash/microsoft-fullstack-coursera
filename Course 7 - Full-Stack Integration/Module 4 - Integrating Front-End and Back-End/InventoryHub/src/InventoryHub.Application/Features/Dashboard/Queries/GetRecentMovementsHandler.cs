using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.Dashboard.Queries;

/// <summary>
/// Handler for retrieving recent stock movements
/// </summary>
public sealed class GetRecentMovementsHandler : IRequestHandler<GetRecentMovementsQuery, Result<List<RecentMovementDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetRecentMovementsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<RecentMovementDto>>> Handle(GetRecentMovementsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.StockMovements
            .Include(m => m.Product)
            .AsQueryable();

        if (request.Type.HasValue)
        {
            query = query.Where(m => m.Type == request.Type.Value);
        }

        if (request.ProductId.HasValue)
        {
            query = query.Where(m => m.ProductId == request.ProductId.Value);
        }

        var movements = await query
            .OrderByDescending(m => m.CreatedAt)
            .Take(request.Limit)
            .Select(m => new RecentMovementDto
            {
                Id = m.Id,
                ProductId = m.ProductId,
                ProductSKU = m.Product.SKU,
                ProductName = m.Product.Name,
                Type = m.Type,
                Quantity = m.Quantity,
                PreviousStock = m.PreviousStock,
                NewStock = m.NewStock,
                Reference = m.Reference,
                Notes = m.Notes,
                CreatedBy = m.CreatedBy,
                CreatedAt = m.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return Result<List<RecentMovementDto>>.Success(movements);
    }
}
