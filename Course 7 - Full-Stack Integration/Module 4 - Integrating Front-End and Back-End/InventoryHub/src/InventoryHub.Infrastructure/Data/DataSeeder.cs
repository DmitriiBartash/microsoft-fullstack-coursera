using InventoryHub.Domain.Entities;
using InventoryHub.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(InventoryDbContext context)
    {
        if (await context.Categories.AnyAsync())
            return;

        var electronics = Category.Create("Electronics", "Electronic devices and accessories");
        var office = Category.Create("Office Supplies", "Office and stationery items");
        var furniture = Category.Create("Furniture", "Office and home furniture");

        var laptops = Category.Create("Laptops", "Portable computers", electronics.Id);
        var accessories = Category.Create("Accessories", "Computer accessories", electronics.Id);
        var paper = Category.Create("Paper Products", "Paper and printing supplies", office.Id);

        await context.Categories.AddRangeAsync(electronics, office, furniture, laptops, accessories, paper);

        var techCorp = Supplier.Create(
            "TechCorp",
            "John Smith",
            "orders@techcorp.com",
            "+1-555-0100",
            "123 Tech Street, San Francisco, CA");

        var officeMax = Supplier.Create(
            "OfficeMax",
            "Jane Doe",
            "sales@officemax.com",
            "+1-555-0200",
            "456 Office Blvd, New York, NY");

        var furniturePro = Supplier.Create(
            "FurniturePro",
            "Mike Johnson",
            "contact@furniturepro.com",
            "+1-555-0300",
            "789 Furniture Ave, Chicago, IL");

        await context.Suppliers.AddRangeAsync(techCorp, officeMax, furniturePro);

        var laptop = Product.Create("ELEC-001", "Laptop Pro 15", 1299.99m, 5, laptops.Id,
            "High-performance laptop with 15-inch display", techCorp.Id);

        var mouse = Product.Create("ELEC-002", "Wireless Mouse", 29.99m, 20, accessories.Id,
            "Ergonomic wireless mouse", techCorp.Id);

        var keyboard = Product.Create("ELEC-003", "Mechanical Keyboard", 89.99m, 15, accessories.Id,
            "RGB mechanical keyboard", techCorp.Id);

        var monitor = Product.Create("ELEC-004", "27\" Monitor", 349.99m, 10, electronics.Id,
            "4K UHD monitor", techCorp.Id);

        var paperA4 = Product.Create("OFF-001", "A4 Paper (500 sheets)", 12.50m, 50, paper.Id,
            "Premium white A4 paper", officeMax.Id);

        var pens = Product.Create("OFF-002", "Ballpoint Pens (12 pack)", 8.99m, 100, office.Id,
            "Blue ballpoint pens", officeMax.Id);

        var desk = Product.Create("FURN-001", "Office Desk", 299.99m, 5, furniture.Id,
            "Modern office desk with cable management", furniturePro.Id);

        var chair = Product.Create("FURN-002", "Ergonomic Chair", 449.99m, 8, furniture.Id,
            "Adjustable ergonomic office chair", furniturePro.Id);

        await context.Products.AddRangeAsync(laptop, mouse, keyboard, monitor, paperA4, pens, desk, chair);

        laptop.AdjustStock(25, MovementType.In, "INIT", "Initial stock", "System");
        mouse.AdjustStock(100, MovementType.In, "INIT", "Initial stock", "System");
        keyboard.AdjustStock(50, MovementType.In, "INIT", "Initial stock", "System");
        monitor.AdjustStock(15, MovementType.In, "INIT", "Initial stock", "System");
        paperA4.AdjustStock(200, MovementType.In, "INIT", "Initial stock", "System");
        pens.AdjustStock(500, MovementType.In, "INIT", "Initial stock", "System");
        desk.AdjustStock(10, MovementType.In, "INIT", "Initial stock", "System");
        chair.AdjustStock(20, MovementType.In, "INIT", "Initial stock", "System");

        var acmeCorp = Client.Create(
            "Acme Corporation",
            "Alice Brown",
            "orders@acme.com",
            "+1-555-1001",
            "100 Corporate Drive, Boston, MA",
            "100 Corporate Drive, Boston, MA 02101");

        var techStartup = Client.Create(
            "Tech Startup Inc",
            "Bob Wilson",
            "bob@techstartup.io",
            "+1-555-1002",
            "50 Innovation Way, Austin, TX",
            "50 Innovation Way, Suite 200, Austin, TX 78701");

        var retailStore = Client.Create(
            "Downtown Retail Store",
            "Carol Davis",
            "manager@downtownretail.com",
            "+1-555-1003",
            "25 Main Street, Seattle, WA",
            "25 Main Street, Seattle, WA 98101");

        var universityIt = Client.Create(
            "State University IT Dept",
            "David Martinez",
            "procurement@stateuniv.edu",
            "+1-555-1004",
            "1000 Campus Road, Denver, CO",
            "IT Building, Room 101, 1000 Campus Road, Denver, CO 80204");

        var homeOffice = Client.Create(
            "John's Home Office",
            "John Peterson",
            "john.peterson@email.com",
            "+1-555-1005",
            "456 Oak Lane, Portland, OR",
            "456 Oak Lane, Portland, OR 97201");

        await context.Clients.AddRangeAsync(acmeCorp, techStartup, retailStore, universityIt, homeOffice);

        var po1 = PurchaseOrder.Create("PO-2024-001", techCorp.Id,
            DateTime.UtcNow.AddDays(-30), "Quarterly laptop restock");
        po1.AddItem(laptop.Id, 10, 1100.00m);
        po1.AddItem(mouse.Id, 50, 22.00m);
        po1.SetStatus(OrderStatus.Received);

        var po2 = PurchaseOrder.Create("PO-2024-002", officeMax.Id,
            DateTime.UtcNow.AddDays(3), "Office supplies restocking");
        po2.AddItem(paperA4.Id, 100, 10.00m);
        po2.AddItem(pens.Id, 200, 7.50m);
        po2.SetStatus(OrderStatus.Shipped);

        var po3 = PurchaseOrder.Create("PO-2024-003", furniturePro.Id,
            DateTime.UtcNow.AddDays(14), "New office setup");
        po3.AddItem(desk.Id, 5, 250.00m);
        po3.AddItem(chair.Id, 10, 380.00m);
        po3.SetStatus(OrderStatus.Confirmed);

        var po4 = PurchaseOrder.Create("PO-2024-004", techCorp.Id,
            DateTime.UtcNow.AddDays(21), "Monitor upgrade project");
        po4.AddItem(monitor.Id, 20, 300.00m);
        po4.AddItem(keyboard.Id, 20, 75.00m);
        po4.SetStatus(OrderStatus.Submitted);

        var po5 = PurchaseOrder.Create("PO-2024-005", techCorp.Id,
            DateTime.UtcNow.AddDays(30), "Q2 Electronics order draft");
        po5.AddItem(laptop.Id, 5, 1100.00m);

        var po6 = PurchaseOrder.Create("PO-2024-006", officeMax.Id,
            DateTime.UtcNow.AddDays(-7), "Cancelled - duplicate order");
        po6.AddItem(pens.Id, 50, 7.50m);
        po6.SetStatus(OrderStatus.Cancelled);

        await context.PurchaseOrders.AddRangeAsync(po1, po2, po3, po4, po5, po6);

        var so1 = SalesOrder.Create("SO-2024-001", acmeCorp.Id,
            DateTime.UtcNow.AddDays(-15),
            "100 Corporate Drive, Boston, MA 02101",
            "Quarterly equipment order");
        so1.AddItem(laptop.Id, 3, 1299.99m);
        so1.AddItem(mouse.Id, 10, 29.99m);
        so1.AddItem(keyboard.Id, 5, 89.99m);
        so1.SetStatus(SalesOrderStatus.Delivered);

        laptop.AdjustStock(3, MovementType.Out, "SO-2024-001", "Sales order fulfilled", "System");
        mouse.AdjustStock(10, MovementType.Out, "SO-2024-001", "Sales order fulfilled", "System");
        keyboard.AdjustStock(5, MovementType.Out, "SO-2024-001", "Sales order fulfilled", "System");

        var so2 = SalesOrder.Create("SO-2024-002", techStartup.Id,
            DateTime.UtcNow.AddDays(2),
            "50 Innovation Way, Suite 200, Austin, TX 78701",
            "Startup office setup");
        so2.AddItem(laptop.Id, 5, 1299.99m);
        so2.AddItem(monitor.Id, 5, 349.99m);
        so2.AddItem(desk.Id, 3, 299.99m);
        so2.AddItem(chair.Id, 5, 449.99m);
        so2.SetStatus(SalesOrderStatus.Shipped);

        laptop.AdjustStock(5, MovementType.Out, "SO-2024-002", "Sales order shipped", "System");
        monitor.AdjustStock(5, MovementType.Out, "SO-2024-002", "Sales order shipped", "System");
        desk.AdjustStock(3, MovementType.Out, "SO-2024-002", "Sales order shipped", "System");
        chair.AdjustStock(5, MovementType.Out, "SO-2024-002", "Sales order shipped", "System");

        var so3 = SalesOrder.Create("SO-2024-003", universityIt.Id,
            DateTime.UtcNow.AddDays(7),
            "IT Building, Room 101, 1000 Campus Road, Denver, CO 80204",
            "Computer lab upgrade - Phase 1");
        so3.AddItem(laptop.Id, 10, 1250.00m);
        so3.AddItem(mouse.Id, 30, 29.99m);
        so3.AddItem(keyboard.Id, 20, 89.99m);
        so3.SetStatus(SalesOrderStatus.Confirmed);

        var so4 = SalesOrder.Create("SO-2024-004", retailStore.Id,
            DateTime.UtcNow.AddDays(14),
            "25 Main Street, Seattle, WA 98101",
            "Retail inventory restocking");
        so4.AddItem(paperA4.Id, 50, 12.50m);
        so4.AddItem(pens.Id, 100, 8.99m);

        var so5 = SalesOrder.Create("SO-2024-005", homeOffice.Id,
            DateTime.UtcNow.AddDays(5),
            "456 Oak Lane, Portland, OR 97201",
            "Home office setup");
        so5.AddItem(desk.Id, 1, 299.99m);
        so5.AddItem(chair.Id, 1, 449.99m);
        so5.AddItem(monitor.Id, 2, 349.99m);
        so5.SetStatus(SalesOrderStatus.Confirmed);

        var so6 = SalesOrder.Create("SO-2024-006", acmeCorp.Id,
            DateTime.UtcNow.AddDays(-5),
            "100 Corporate Drive, Boston, MA 02101",
            "Cancelled - budget constraints");
        so6.AddItem(laptop.Id, 2, 1299.99m);
        so6.SetStatus(SalesOrderStatus.Cancelled);

        await context.SalesOrders.AddRangeAsync(so1, so2, so3, so4, so5, so6);

        pens.AdjustStock(10, MovementType.Adjust, "ADJ-001", "Inventory count correction", "Admin");
        paperA4.AdjustStock(5, MovementType.Out, "DAMAGE-001", "Damaged in warehouse", "Warehouse");

        mouse.AdjustStock(15, MovementType.Out, "WALK-IN-001", "Walk-in customer purchase", "Sales");
        pens.AdjustStock(25, MovementType.Out, "WALK-IN-002", "Walk-in customer purchase", "Sales");

        await context.SaveChangesAsync();
    }
}
