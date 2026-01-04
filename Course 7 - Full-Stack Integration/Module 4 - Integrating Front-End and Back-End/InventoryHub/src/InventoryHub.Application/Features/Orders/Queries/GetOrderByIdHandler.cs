using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.Orders.Queries;

/// <summary>
/// Handler for getting purchase order by ID.
/// </summary>
public sealed class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, Result<PurchaseOrderDto>>
{
    private readonly IApplicationDbContext _context;

    public GetOrderByIdHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PurchaseOrderDto>> Handle(
        GetOrderByIdQuery request,
        CancellationToken ct)
    {
        var order = await _context.PurchaseOrders
            .Include(o => o.Supplier)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .AsNoTracking()
            .Where(o => o.Id == request.Id)
            .Select(o => new PurchaseOrderDto
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
            })
            .FirstOrDefaultAsync(ct);

        if (order is null)
        {
            return Result<PurchaseOrderDto>.Failure(
                "ORDER_NOT_FOUND",
                $"Purchase order with ID {request.Id} was not found");
        }

        return Result<PurchaseOrderDto>.Success(order);
    }
}
