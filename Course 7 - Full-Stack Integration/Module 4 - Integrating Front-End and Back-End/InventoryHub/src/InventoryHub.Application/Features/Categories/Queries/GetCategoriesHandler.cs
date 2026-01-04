using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.Categories.Queries;

/// <summary>
/// Handler for retrieving all categories with hierarchy
/// </summary>
public sealed class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, Result<List<CategoryDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetCategoriesHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Categories.AsQueryable();

        if (request.IncludeChildren)
        {
            query = query.Include(c => c.Children);
        }

        if (request.IncludeProductCount)
        {
            query = query.Include(c => c.Products);
        }

        query = query.Include(c => c.Parent);

        var categories = await query.ToListAsync(cancellationToken);

        var rootCategories = categories
            .Where(c => c.ParentId == null)
            .Select(c => MapToDto(c, categories, request.IncludeChildren))
            .OrderBy(c => c.Name)
            .ToList();

        return Result<List<CategoryDto>>.Success(rootCategories);
    }

    private static CategoryDto MapToDto(
        Domain.Entities.Category category,
        List<Domain.Entities.Category> allCategories,
        bool includeChildren)
    {
        var dto = new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ParentId = category.ParentId,
            ParentName = category.Parent?.Name,
            ProductCount = category.Products.Count,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };

        if (includeChildren)
        {
            var children = allCategories
                .Where(c => c.ParentId == category.Id)
                .Select(c => MapToDto(c, allCategories, true))
                .OrderBy(c => c.Name)
                .ToList();

            return dto with { Children = children };
        }

        return dto;
    }
}
