using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using InventoryHub.Domain.Entities;
using InventoryHub.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.Products.Commands.CreateProduct;

/// <summary>
/// Handler for creating a new product.
/// </summary>
public sealed class CreateProductHandler : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateProductHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the product creation request.
    /// </summary>
    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var skuExists = await _context.Products
            .AnyAsync(p => p.SKU == request.SKU, ct);

        if (skuExists)
        {
            return Result<Guid>.Failure("Product.DuplicateSKU", $"Product with SKU '{request.SKU}' already exists.");
        }

        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == request.CategoryId, ct);

        if (!categoryExists)
        {
            return Result<Guid>.Failure("Product.CategoryNotFound", $"Category with ID '{request.CategoryId}' does not exist.");
        }

        if (request.SupplierId.HasValue)
        {
            var supplierExists = await _context.Suppliers
                .AnyAsync(s => s.Id == request.SupplierId.Value, ct);

            if (!supplierExists)
            {
                return Result<Guid>.Failure("Product.SupplierNotFound", $"Supplier with ID '{request.SupplierId}' does not exist.");
            }
        }

        var product = Product.Create(
            request.SKU,
            request.Name,
            request.Price,
            request.MinimumStock,
            request.CategoryId,
            request.Description,
            request.SupplierId);

        _context.Products.Add(product);

        if (request.InitialStock > 0)
        {
            var stockMovement = product.AdjustStock(
                request.InitialStock,
                MovementType.In,
                reference: "INIT",
                notes: "Initial stock on product creation",
                createdBy: "System");

            _context.StockMovements.Add(stockMovement);
        }

        await _context.SaveChangesAsync(ct);

        return Result<Guid>.Success(product.Id);
    }
}
