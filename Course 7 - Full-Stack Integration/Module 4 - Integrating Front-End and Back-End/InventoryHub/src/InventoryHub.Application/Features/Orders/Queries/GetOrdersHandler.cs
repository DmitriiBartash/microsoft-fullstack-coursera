using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using InventoryHub.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.Orders.Queries;

/// <summary>
/// Handler for getting paginated list of purchase orders.
/// </summary>
public sealed class GetOrdersHandler : IRequestHandler<GetOrdersQuery, Result<PaginatedList<PurchaseOrderDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetOrdersHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<PurchaseOrderDto>>> Handle(
        GetOrdersQuery request,
        CancellationToken ct)
    {
        var query = _context.PurchaseOrders
            .Include(o => o.Supplier)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (Enum.TryParse<OrderStatus>(request.Status, ignoreCase: true, out var status))
            {
                query = query.Where(o => o.Status == status);
            }
            else
            {
                return Result<PaginatedList<PurchaseOrderDto>>.Failure(
                    "INVALID_STATUS",
                    $"Invalid order status: {request.Status}");
            }
        }

        if (request.SupplierId.HasValue)
        {
            query = query.Where(o => o.SupplierId == request.SupplierId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(o =>
                o.OrderNumber.ToLower().Contains(search) ||
                o.Supplier.Name.ToLower().Contains(search) ||
                (o.Notes != null && o.Notes.ToLower().Contains(search)));
        }

        query = query.OrderByDescending(o => o.OrderDate);

        var orders = await PaginatedList<PurchaseOrderDto>.CreateAsync(
            query.Select(o => new PurchaseOrderDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                SupplierId = o.SupplierId,
                SupplierName = o.Supplier.Name,
                Status = o.Status.ToString(),
                OrderDate = o.OrderDate,
                ExpectedDeliveryDate = o.ExpectedDeliveryDate,
                TotalAmount = o.TotalAmount,
                Notes = o.Notes,
                Items = o.Items.Select(i => new OrderItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    ProductSKU = i.Product.SKU,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    LineTotal = i.LineTotal,
                    ReceivedQuantity = i.ReceivedQuantity
                }).ToList()
            }),
            request.Page,
            request.PageSize,
            ct);

        return Result<PaginatedList<PurchaseOrderDto>>.Success(orders);
    }
}
