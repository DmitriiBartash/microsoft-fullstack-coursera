using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.Categories.Commands;

/// <summary>
/// Handler for updating an existing category
/// </summary>
public sealed class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public UpdateCategoryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Result<bool>.Failure("Category.InvalidName", "Category name cannot be empty.");
        }

        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (category is null)
        {
            return Result<bool>.Failure("Category.NotFound", $"Category with ID '{request.Id}' was not found.");
        }

        var duplicateName = await _context.Categories
            .AnyAsync(c => c.Name == request.Name && c.Id != request.Id, cancellationToken);

        if (duplicateName)
        {
            return Result<bool>.Failure("Category.DuplicateName", $"Category with name '{request.Name}' already exists.");
        }

        if (request.ParentId.HasValue)
        {
            if (request.ParentId.Value == request.Id)
            {
                return Result<bool>.Failure("Category.CircularReference", "Category cannot be its own parent.");
            }

            var parentExists = await _context.Categories
                .AnyAsync(c => c.Id == request.ParentId.Value, cancellationToken);

            if (!parentExists)
            {
                return Result<bool>.Failure("Category.ParentNotFound", $"Parent category with ID '{request.ParentId}' was not found.");
            }

            var wouldCreateCircularReference = await WouldCreateCircularReference(
                request.Id,
                request.ParentId.Value,
                cancellationToken);

            if (wouldCreateCircularReference)
            {
                return Result<bool>.Failure("Category.CircularReference", "Cannot set parent category as it would create a circular reference.");
            }
        }

        category.Update(request.Name, request.Description, request.ParentId);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    private async Task<bool> WouldCreateCircularReference(Guid categoryId, Guid parentId, CancellationToken cancellationToken)
    {
        Guid? currentParentId = parentId;

        while (currentParentId.HasValue)
        {
            if (currentParentId.Value == categoryId)
            {
                return true;
            }

            var parent = await _context.Categories
                .Where(c => c.Id == currentParentId.Value)
                .Select(c => new { c.ParentId })
                .FirstOrDefaultAsync(cancellationToken);

            currentParentId = parent?.ParentId;
        }

        return false;
    }
}
