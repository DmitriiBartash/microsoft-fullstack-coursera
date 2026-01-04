using InventoryHub.Application.Common.Models;
using InventoryHub.Application.Features.Categories;
using InventoryHub.Application.Features.Categories.Commands;
using InventoryHub.Application.Features.Categories.Queries;
using MediatR;

namespace InventoryHub.API.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/categories").WithTags("Categories");

        group.MapGet("/", GetCategories);
        group.MapGet("/{id:guid}", GetCategoryById);
        group.MapPost("/", CreateCategory);
        group.MapPut("/{id:guid}", UpdateCategory);
        group.MapDelete("/{id:guid}", DeleteCategory);
    }

    private static async Task<IResult> GetCategories(
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetCategoriesQuery(), ct);
        return result.IsSuccess
            ? Results.Ok(new { success = true, data = result.Data })
            : Results.BadRequest(new { success = false, error = result.Error });
    }

    private static async Task<IResult> GetCategoryById(
        Guid id,
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetCategoryByIdQuery(id), ct);
        return result.IsSuccess
            ? Results.Ok(new { success = true, data = result.Data })
            : Results.NotFound(new { success = false, error = result.Error });
    }

    private static async Task<IResult> CreateCategory(
        CreateCategoryCommand command,
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(command, ct);
        return result.IsSuccess
            ? Results.Created($"/api/categories/{result.Data}", new { success = true, data = result.Data })
            : Results.BadRequest(new { success = false, error = result.Error });
    }

    private static async Task<IResult> UpdateCategory(
        Guid id,
        UpdateCategoryCommand command,
        ISender sender,
        CancellationToken ct = default)
    {
        if (id != command.Id)
            return Results.BadRequest(new { success = false, error = new Error("INVALID_ID", "ID mismatch") });

        var result = await sender.Send(command, ct);
        return result.IsSuccess
            ? Results.Ok(new { success = true, data = result.Data })
            : Results.BadRequest(new { success = false, error = result.Error });
    }

    private static async Task<IResult> DeleteCategory(
        Guid id,
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new DeleteCategoryCommand(id), ct);
        return result.IsSuccess
            ? Results.Ok(new { success = true, data = result.Data })
            : Results.BadRequest(new { success = false, error = result.Error });
    }
}
