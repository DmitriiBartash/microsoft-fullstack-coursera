using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using InventoryHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.Orders.Commands;

/// <summary>
/// Handler for creating new purchase order.
/// </summary>
public sealed class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateOrderHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(
        CreateOrderCommand request,
        CancellationToken ct)
    {
        var supplierExists = await _context.Suppliers
            .AnyAsync(s => s.Id == request.SupplierId, ct);

        if (!supplierExists)
        {
            return Result<Guid>.Failure(
                "SUPPLIER_NOT_FOUND",
                $"Supplier with ID {request.SupplierId} was not found");
        }

        if (request.Items.Count == 0)
        {
            return Result<Guid>.Failure(
                "EMPTY_ORDER",
                "Cannot create an order without items");
        }

        var productIds = request.Items.Select(i => i.ProductId).ToList();
        var products = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, ct);

        var missingProducts = productIds.Except(products.Keys).ToList();
        if (missingProducts.Any())
        {
            return Result<Guid>.Failure(
                "PRODUCTS_NOT_FOUND",
                $"Some products were not found: {string.Join(", ", missingProducts)}");
        }

        foreach (var item in request.Items)
        {
            if (item.Quantity <= 0)
            {
                return Result<Guid>.Failure(
                    "INVALID_QUANTITY",
                    $"Quantity for product {products[item.ProductId].Name} must be greater than zero");
            }

            if (item.UnitPrice < 0)
            {
                return Result<Guid>.Failure(
                    "INVALID_PRICE",
                    $"Unit price for product {products[item.ProductId].Name} cannot be negative");
            }
        }

        var orderNumber = await GenerateOrderNumberAsync(ct);

        var order = PurchaseOrder.Create(
            orderNumber,
            request.SupplierId,
            request.ExpectedDeliveryDate,
            request.Notes);

        foreach (var item in request.Items)
        {
            order.AddItem(item.ProductId, item.Quantity, item.UnitPrice);
        }

        _context.PurchaseOrders.Add(order);
        await _context.SaveChangesAsync(ct);

        return Result<Guid>.Success(order.Id);
    }

    private async Task<string> GenerateOrderNumberAsync(CancellationToken ct)
    {
        var currentYear = DateTime.UtcNow.Year;
        var yearPrefix = $"PO-{currentYear}-";

        var lastOrder = await _context.PurchaseOrders
            .Where(o => o.OrderNumber.StartsWith(yearPrefix))
            .OrderByDescending(o => o.OrderNumber)
            .Select(o => o.OrderNumber)
            .FirstOrDefaultAsync(ct);

        int sequential = 1;
        if (lastOrder != null)
        {
            var lastSequential = lastOrder.Replace(yearPrefix, string.Empty);
            if (int.TryParse(lastSequential, out var parsed))
            {
                sequential = parsed + 1;
            }
        }

        return $"{yearPrefix}{sequential:D4}";
    }
}
