using FluentValidation;
using InventoryHub.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.Products.Commands.CreateProduct;

/// <summary>
/// Validator for CreateProductCommand.
/// </summary>
public sealed class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateProductValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.SKU)
            .NotEmpty().WithMessage("SKU is required.")
            .MaximumLength(50).WithMessage("SKU must not exceed 50 characters.")
            .MustAsync(BeUniqueSKU).WithMessage("Product with this SKU already exists.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(200).WithMessage("Product name must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");

        RuleFor(x => x.MinimumStock)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum stock must be greater than or equal to 0.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category is required.")
            .MustAsync(CategoryExists).WithMessage("Selected category does not exist.");

        RuleFor(x => x.SupplierId)
            .MustAsync(SupplierExists).WithMessage("Selected supplier does not exist.")
            .When(x => x.SupplierId.HasValue);
    }

    /// <summary>
    /// Validates that the SKU is unique.
    /// </summary>
    private async Task<bool> BeUniqueSKU(string sku, CancellationToken ct)
    {
        return !await _context.Products.AnyAsync(p => p.SKU == sku, ct);
    }

    /// <summary>
    /// Validates that the category exists.
    /// </summary>
    private async Task<bool> CategoryExists(Guid categoryId, CancellationToken ct)
    {
        return await _context.Categories.AnyAsync(c => c.Id == categoryId, ct);
    }

    /// <summary>
    /// Validates that the supplier exists.
    /// </summary>
    private async Task<bool> SupplierExists(Guid? supplierId, CancellationToken ct)
    {
        if (!supplierId.HasValue)
            return true;

        return await _context.Suppliers.AnyAsync(s => s.Id == supplierId.Value, ct);
    }
}
