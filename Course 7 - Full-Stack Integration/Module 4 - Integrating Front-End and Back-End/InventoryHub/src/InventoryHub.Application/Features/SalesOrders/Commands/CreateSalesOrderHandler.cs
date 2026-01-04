using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using InventoryHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.SalesOrders.Commands;

/// <summary>
/// Handler for creating new sales order.
/// </summary>
public sealed class CreateSalesOrderHandler : IRequestHandler<CreateSalesOrderCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateSalesOrderHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(
        CreateSalesOrderCommand request,
        CancellationToken ct)
    {
        var clientExists = await _context.Clients
            .AnyAsync(c => c.Id == request.ClientId && c.IsActive, ct);

        if (!clientExists)
        {
            return Result<Guid>.Failure(
                "CLIENT_NOT_FOUND",
                $"Active client with ID {request.ClientId} was not found");
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

        var order = SalesOrder.Create(
            orderNumber,
            request.ClientId,
            request.RequiredDate,
            request.ShippingAddress,
            request.Notes);

        foreach (var item in request.Items)
        {
            order.AddItem(item.ProductId, item.Quantity, item.UnitPrice);
        }

        _context.SalesOrders.Add(order);
        await _context.SaveChangesAsync(ct);

        return Result<Guid>.Success(order.Id);
    }

    private async Task<string> GenerateOrderNumberAsync(CancellationToken ct)
    {
        var currentYear = DateTime.UtcNow.Year;
        var yearPrefix = $"SO-{currentYear}-";

        var lastOrder = await _context.SalesOrders
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
