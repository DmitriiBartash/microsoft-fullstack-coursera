using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.SalesOrders.Queries;

/// <summary>
/// Handler for getting sales order by ID.
/// </summary>
public sealed class GetSalesOrderByIdHandler : IRequestHandler<GetSalesOrderByIdQuery, Result<SalesOrderDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSalesOrderByIdHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<SalesOrderDto>> Handle(
        GetSalesOrderByIdQuery request,
        CancellationToken ct)
    {
        var order = await _context.SalesOrders
            .Include(o => o.Client)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .AsNoTracking()
            .Where(o => o.Id == request.Id)
            .Select(o => new SalesOrderDto
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
            })
            .FirstOrDefaultAsync(ct);

        if (order is null)
        {
            return Result<SalesOrderDto>.Failure(
                "SALES_ORDER_NOT_FOUND",
                $"Sales order with ID {request.Id} was not found");
        }

        return Result<SalesOrderDto>.Success(order);
    }
}
