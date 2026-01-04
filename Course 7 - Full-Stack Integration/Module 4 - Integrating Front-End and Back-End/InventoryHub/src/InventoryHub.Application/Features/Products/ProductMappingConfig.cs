using InventoryHub.Domain.Entities;
using Mapster;

namespace InventoryHub.Application.Features.Products;

/// <summary>
/// Mapster configuration for Product entity to ProductDto mapping
/// </summary>
public sealed class ProductMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Product, ProductDto>()
            .Map(dest => dest.CategoryName, src => src.Category.Name)
            .Map(dest => dest.SupplierName, src => src.Supplier != null ? src.Supplier.Name : null)
            .Map(dest => dest.IsLowStock, src => src.CurrentStock <= src.MinimumStock)
            .Map(dest => dest.IsOutOfStock, src => src.CurrentStock == 0);
    }
}
