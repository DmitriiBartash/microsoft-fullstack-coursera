using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.Products.Queries.GetProductById;

/// <summary>
/// Query to retrieve a single product by ID
/// </summary>
public sealed record GetProductByIdQuery(Guid Id) : IRequest<Result<ProductDto>>;
