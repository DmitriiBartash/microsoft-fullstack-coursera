using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using InventoryHub.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.SalesOrders.Queries;

/// <summary>
/// Handler for getting paginated list of sales orders.
/// </summary>
public sealed class GetSalesOrdersHandler : IRequestHandler<GetSalesOrdersQuery, Result<PaginatedList<SalesOrderDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetSalesOrdersHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<SalesOrderDto>>> Handle(
        GetSalesOrdersQuery request,
        CancellationToken ct)
    {
        var query = _context.SalesOrders
            .Include(o => o.Client)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (Enum.TryParse<SalesOrderStatus>(request.Status, ignoreCase: true, out var status))
            {
                query = query.Where(o => o.Status == status);
            }
            else
            {
                return Result<PaginatedList<SalesOrderDto>>.Failure(
                    "INVALID_STATUS",
                    $"Invalid order status: {request.Status}");
            }
        }

        if (request.ClientId.HasValue)
        {
            query = query.Where(o => o.ClientId == request.ClientId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(o =>
                o.OrderNumber.ToLower().Contains(search) ||
                o.Client.Name.ToLower().Contains(search) ||
                (o.ShippingAddress != null && o.ShippingAddress.ToLower().Contains(search)) ||
                (o.Notes != null && o.Notes.ToLower().Contains(search)));
        }

        query = query.OrderByDescending(o => o.OrderDate);

        var orders = await PaginatedList<SalesOrderDto>.CreateAsync(
            query.Select(o => new SalesOrderDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                ClientId = o.ClientId,
                ClientName = o.Client.Name,
                Status = o.Status.ToString(),
                OrderDate = o.OrderDate,
                RequiredDate = o.RequiredDate,
                ShippingAddress = o.ShippingAddress,
                TotalAmount = o.TotalAmount,
                Notes = o.Notes,
                Items = o.Items.Select(i => new SalesOrderItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    ProductSKU = i.Product.SKU,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    LineTotal = i.LineTotal,
                    CurrentStock = i.Product.CurrentStock
                }).ToList()
            }),
            request.Page,
            request.PageSize,
            ct);

        return Result<PaginatedList<SalesOrderDto>>.Success(orders);
    }
}
