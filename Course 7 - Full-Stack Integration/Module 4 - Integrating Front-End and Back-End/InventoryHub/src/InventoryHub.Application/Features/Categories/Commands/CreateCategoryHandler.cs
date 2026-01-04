using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using InventoryHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.Categories.Commands;

/// <summary>
/// Handler for creating a new category
/// </summary>
public sealed class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateCategoryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Result<Guid>.Failure("Category.InvalidName", "Category name cannot be empty.");
        }

        var nameExists = await _context.Categories
            .AnyAsync(c => c.Name == request.Name, cancellationToken);

        if (nameExists)
        {
            return Result<Guid>.Failure("Category.DuplicateName", $"Category with name '{request.Name}' already exists.");
        }

        if (request.ParentId.HasValue)
        {
            var parentExists = await _context.Categories
                .AnyAsync(c => c.Id == request.ParentId.Value, cancellationToken);

            if (!parentExists)
            {
                return Result<Guid>.Failure("Category.ParentNotFound", $"Parent category with ID '{request.ParentId}' was not found.");
            }
        }

        var category = Category.Create(request.Name, request.Description, request.ParentId);

        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(category.Id);
    }
}
