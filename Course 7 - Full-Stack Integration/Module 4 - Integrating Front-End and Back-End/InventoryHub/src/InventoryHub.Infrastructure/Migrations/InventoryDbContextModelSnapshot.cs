using System;
using InventoryHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace InventoryHub.Infrastructure.Migrations
{
    [DbContext(typeof(InventoryDbContext))]
    partial class InventoryDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "10.0.1");

            modelBuilder.Entity("InventoryHub.Domain.Entities.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Name");

                    b.HasIndex("ParentId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("InventoryHub.Domain.Entities.Client", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Address")
                        .HasMaxLength(500)
                        .HasColumnType("TEXT");

                    b.Property<string>("ContactPerson")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<string>("Phone")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("ShippingAddress")
                        .HasMaxLength(500)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("IsActive");

                    b.HasIndex("Name");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("InventoryHub.Domain.Entities.OrderItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("TEXT");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ReceivedQuantity")
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("UnitPrice")
                        .HasPrecision(18, 2)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.HasIndex("ProductId");

                    b.ToTable("OrderItems");
                });

            modelBuilder.Entity("InventoryHub.Domain.Entities.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("CurrentStock")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MinimumStock")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Price")
                        .HasPrecision(18, 2)
                        .HasColumnType("TEXT");

                    b.Property<string>("SKU")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("SupplierId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("IsActive");

                    b.HasIndex("Name");

                    b.HasIndex("SKU")
                        .IsUnique();

                    b.HasIndex("SupplierId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("InventoryHub.Domain.Entities.PurchaseOrder", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ExpectedDeliveryDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Notes")
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("OrderNumber")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("SupplierId")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("TotalAmount")
                        .HasPrecision(18, 2)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("OrderDate");

                    b.HasIndex("OrderNumber")
                        .IsUnique();

                    b.HasIndex("Status");

                    b.HasIndex("SupplierId");

                    b.ToTable("PurchaseOrders");
                });

            modelBuilder.Entity("InventoryHub.Domain.Entities.SalesOrder", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ClientId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Notes")
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("OrderNumber")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("RequiredDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("ShippingAddress")
                        .HasMaxLength(500)
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("TotalAmount")
                        .HasPrecision(18, 2)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("OrderDate");

                    b.HasIndex("OrderNumber")
                        .IsUnique();

                    b.HasIndex("Status");

                    b.ToTable("SalesOrders");
                });

            modelBuilder.Entity("InventoryHub.Domain.Entities.SalesOrderItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("TEXT");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("SalesOrderId")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("UnitPrice")
                        .HasPrecision(18, 2)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("SalesOrderId");

                    b.ToTable("SalesOrderItems");
                });

            modelBuilder.Entity("InventoryHub.Domain.Entities.StockMovement", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<int>("NewStock")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Notes")
                        .HasMaxLength(500)
                        .HasColumnType("TEXT");

                    b.Property<int>("PreviousStock")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("TEXT");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Reference")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("ProductId");

                    b.HasIndex("Type");

                    b.ToTable("StockMovements");
                });

            modelBuilder.Entity("InventoryHub.Domain.Entities.Supplier", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Address")
                        .HasMaxLength(500)
                        .HasColumnType("TEXT");

                    b.Property<string>("ContactPerson")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<string>("Phone")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("IsActive");

                    b.HasIndex("Name");

                    b.ToTable("Suppliers");
                });

            modelBuilder.Entity("InventoryHub.Domain.Entities.Category", b =>
                {
                    b.HasOne("InventoryHub.Domain.Entities.Category", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("InventoryHub.Domain.Entities.OrderItem", b =>
                {
                    b.HasOne("InventoryHub.Domain.Entities.PurchaseOrder", "Order")
                        .WithMany("Items")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("InventoryHub.Domain.Entities.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Order");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("InventoryHub.Domain.Entities.Product", b =>
                {
                    b.HasOne("InventoryHub.Domain.Entities.Category", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("InventoryHub.Domain.Entities.Supplier", "Supplier")
                        .WithMany("Products")
                        .HasForeignKey("SupplierId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Category");

                    b.Navigation("Supplier");
                });

            modelBuilder.Entity("InventoryHub.Domain.Entities.PurchaseOrder", b =>
                {
                    b.HasOne("InventoryHub.Domain.Entities.Supplier", "Supplier")
                        .WithMany("Orders")
                        .HasForeignKey("SupplierId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Supplier");
                });

            modelBuilder.Entity("InventoryHub.Domain.Entities.SalesOrder", b =>
                {
                    b.HasOne("InventoryHub.Domain.Entities.Client", "Client")
                        .WithMany("Orders")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Client");
                });

            modelBuilder.Entity("InventoryHub.Domain.Entities.SalesOrderItem", b =>
                {
                    b.HasOne("InventoryHub.Domain.Entities.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("InventoryHub.Domain.Entities.SalesOrder", "SalesOrder")
                        .WithMany("Items")
                        .HasForeignKey("SalesOrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("SalesOrder");
                });

            modelBuilder.Entity("InventoryHub.Domain.Entities.StockMovement", b =>
                {
                    b.HasOne("InventoryHub.Domain.Entities.Product", "Product")
                        .WithMany("Movements")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("InventoryHub.Domain.Entities.Category", b =>
                {
                    b.Navigation("Children");

                    b.Navigation("Products");
                });

            modelBuilder.Entity("InventoryHub.Domain.Entities.Client", b =>
                {
                    b.Navigation("Orders");
                });

            modelBuilder.Entity("InventoryHub.Domain.Entities.Product", b =>
                {
                    b.Navigation("Movements");
                });

            modelBuilder.Entity("InventoryHub.Domain.Entities.PurchaseOrder", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("InventoryHub.Domain.Entities.SalesOrder", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("InventoryHub.Domain.Entities.Supplier", b =>
                {
                    b.Navigation("Orders");

                    b.Navigation("Products");
                });
#pragma warning restore 612, 618
        }
    }
}
