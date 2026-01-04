using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.Categories.Commands;

/// <summary>
/// Handler for deleting a category
/// </summary>
public sealed class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public DeleteCategoryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .Include(c => c.Products)
            .Include(c => c.Children)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (category is null)
        {
            return Result<bool>.Failure("Category.NotFound", $"Category with ID '{request.Id}' was not found.");
        }

        if (category.Products.Any())
        {
            return Result<bool>.Failure(
                "Category.HasProducts",
                $"Cannot delete category '{category.Name}' because it contains {category.Products.Count} product(s). Please reassign or delete the products first.");
        }

        if (category.Children.Any())
        {
            var childWord = category.Children.Count == 1 ? "subcategory" : "subcategories";
            return Result<bool>.Failure(
                "Category.HasChildren",
                $"Cannot delete category '{category.Name}' because it has {category.Children.Count} {childWord}. Please reassign or delete them first.");
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
