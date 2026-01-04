using InventoryHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }
    DbSet<Category> Categories { get; }
    DbSet<Supplier> Suppliers { get; }
    DbSet<StockMovement> StockMovements { get; }
    DbSet<PurchaseOrder> PurchaseOrders { get; }
    DbSet<OrderItem> OrderItems { get; }
    DbSet<Client> Clients { get; }
    DbSet<SalesOrder> SalesOrders { get; }
    DbSet<SalesOrderItem> SalesOrderItems { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
