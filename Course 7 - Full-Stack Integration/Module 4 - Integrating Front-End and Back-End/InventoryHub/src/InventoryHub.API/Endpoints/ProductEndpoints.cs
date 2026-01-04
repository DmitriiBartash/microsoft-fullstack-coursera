using InventoryHub.Application.Common.Models;
using InventoryHub.Application.Features.Products;
using InventoryHub.Application.Features.Products.Commands.CreateProduct;
using InventoryHub.Application.Features.Products.Commands.UpdateProduct;
using InventoryHub.Application.Features.Products.Commands.DeleteProduct;
using InventoryHub.Application.Features.Products.Queries.GetProducts;
using InventoryHub.Application.Features.Products.Queries.GetProductById;
using MediatR;

namespace InventoryHub.API.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/products").WithTags("Products");

        group.MapGet("/", GetProducts);
        group.MapGet("/{id:guid}", GetProductById);
        group.MapPost("/", CreateProduct);
        group.MapPut("/{id:guid}", UpdateProduct);
        group.MapDelete("/{id:guid}", DeleteProduct);
    }

    private static async Task<IResult> GetProducts(
        ISender sender,
        int page = 1,
        int pageSize = 10,
        string? search = null,
        Guid? categoryId = null,
        Guid? supplierId = null,
        bool? lowStock = null,
        string? sortBy = null,
        string? sortDirection = null,
        CancellationToken ct = default)
    {
        var query = new GetProductsQuery
        {
            Page = page,
            PageSize = pageSize,
            Search = search,
            CategoryId = categoryId,
            SupplierId = supplierId,
            LowStock = lowStock,
            SortBy = sortBy,
            SortDirection = sortDirection
        };

        var result = await sender.Send(query, ct);
        return result.IsSuccess
            ? Results.Ok(new { success = true, data = result.Data })
            : Results.BadRequest(new { success = false, error = result.Error });
    }

    private static async Task<IResult> GetProductById(
        Guid id,
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetProductByIdQuery(id), ct);
        return result.IsSuccess
            ? Results.Ok(new { success = true, data = result.Data })
            : Results.NotFound(new { success = false, error = result.Error });
    }

    private static async Task<IResult> CreateProduct(
        CreateProductCommand command,
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(command, ct);
        return result.IsSuccess
            ? Results.Created($"/api/products/{result.Data}", new { success = true, data = result.Data })
            : Results.BadRequest(new { success = false, error = result.Error });
    }

    private static async Task<IResult> UpdateProduct(
        Guid id,
        UpdateProductRequest request,
        ISender sender,
        CancellationToken ct = default)
    {
        var command = new UpdateProductCommand
        {
            Id = id,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            MinimumStock = request.MinimumStock,
            CategoryId = request.CategoryId,
            SupplierId = request.SupplierId
        };

        var result = await sender.Send(command, ct);
        return result.IsSuccess
            ? Results.Ok(new { success = true, data = result.Data })
            : Results.BadRequest(new { success = false, error = result.Error });
    }

    public record UpdateProductRequest(
        string Name,
        string? Description,
        decimal Price,
        int MinimumStock,
        Guid CategoryId,
        Guid? SupplierId = null);

    private static async Task<IResult> DeleteProduct(
        Guid id,
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new DeleteProductCommand { Id = id }, ct);
        return result.IsSuccess
            ? Results.Ok(new { success = true, data = result.Data })
            : Results.NotFound(new { success = false, error = result.Error });
    }
}
