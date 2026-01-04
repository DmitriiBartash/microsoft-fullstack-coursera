using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.Inventory.Queries.GetMovements;

/// <summary>
/// Handler for retrieving stock movements with filters
/// </summary>
public sealed class GetMovementsHandler : IRequestHandler<GetMovementsQuery, Result<PaginatedList<StockMovementDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetMovementsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the stock movements retrieval request
    /// </summary>
    public async Task<Result<PaginatedList<StockMovementDto>>> Handle(GetMovementsQuery request, CancellationToken ct)
    {
        var query = _context.StockMovements
            .Include(m => m.Product)
            .AsQueryable();

        if (request.ProductId.HasValue)
        {
            query = query.Where(m => m.ProductId == request.ProductId.Value);
        }

        if (request.Type.HasValue)
        {
            query = query.Where(m => m.Type == request.Type.Value);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(m => m.CreatedAt >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(m => m.CreatedAt <= request.ToDate.Value);
        }

        var movements = query
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => new StockMovementDto
            {
                Id = m.Id,
                ProductId = m.ProductId,
                ProductName = m.Product.Name,
                ProductSKU = m.Product.SKU,
                Type = m.Type.ToString(),
                Quantity = m.Quantity,
                PreviousStock = m.PreviousStock,
                NewStock = m.NewStock,
                Reference = m.Reference,
                Notes = m.Notes,
                CreatedAt = m.CreatedAt,
                CreatedBy = m.CreatedBy
            });

        var paginatedList = await PaginatedList<StockMovementDto>.CreateAsync(
            movements,
            request.Page,
            request.PageSize,
            ct);

        return Result<PaginatedList<StockMovementDto>>.Success(paginatedList);
    }
}
