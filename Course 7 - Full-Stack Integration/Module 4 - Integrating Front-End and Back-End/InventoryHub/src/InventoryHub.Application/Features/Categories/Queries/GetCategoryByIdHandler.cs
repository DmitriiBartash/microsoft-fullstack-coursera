using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.Categories.Queries;

/// <summary>
/// Handler for retrieving a category by identifier
/// </summary>
public sealed class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCategoryByIdHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Categories
            .Include(c => c.Parent)
            .Include(c => c.Products)
            .AsQueryable();

        if (request.IncludeChildren)
        {
            query = query.Include(c => c.Children);
        }

        var category = await query.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (category is null)
        {
            return Result<CategoryDto>.Failure("Category.NotFound", $"Category with ID '{request.Id}' was not found.");
        }

        var dto = new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ParentId = category.ParentId,
            ParentName = category.Parent?.Name,
            ProductCount = category.Products.Count,
            Children = request.IncludeChildren
                ? category.Children.Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ParentId = c.ParentId,
                    ParentName = category.Name,
                    ProductCount = 0,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                }).OrderBy(c => c.Name).ToList()
                : [],
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };

        return Result<CategoryDto>.Success(dto);
    }
}
