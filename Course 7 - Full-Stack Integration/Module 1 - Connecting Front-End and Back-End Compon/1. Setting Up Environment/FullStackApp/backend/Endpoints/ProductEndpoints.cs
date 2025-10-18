using backend.Models;
using backend.Services;
using System.Text.Json;

namespace backend.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products");

        group.MapGet("/", (ProductService service) =>
            Results.Ok(service.GetAll()));

        group.MapGet("/{id:int}", (int id, ProductService service) =>
        {
            var product = service.GetById(id);
            return product is null ? Results.NotFound() : Results.Ok(product);
        });

        group.MapGet("/search", (string? keyword, string? category, ProductService service) =>
        {
            var result = service.Search(keyword, category);
            return result.Any() ? Results.Ok(result) : Results.NotFound();
        });

        group.MapPost("/", (Product newProduct, ProductService service) =>
        {
            var added = service.Add(newProduct);
            return Results.Created($"/api/products/{added.Id}", added);
        });

        group.MapPut("/{id:int}", (int id, Product updated, ProductService service) =>
        {
            var result = service.Update(id, updated);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapPatch("/{id:int}", (int id, JsonElement patchDoc, ProductService service) =>
        {
            var result = service.Patch(id, patchDoc);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapDelete("/{id:int}", (int id, ProductService service) =>
        {
            var removed = service.Delete(id);
            return removed ? Results.NoContent() : Results.NotFound();
        });
    }
}
