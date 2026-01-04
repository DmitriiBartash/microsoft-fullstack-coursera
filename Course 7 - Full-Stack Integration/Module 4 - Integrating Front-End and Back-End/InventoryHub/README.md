# InventoryHub Pro

Full-stack inventory management system for small business.

## Tech Stack

| Layer | Technology |
|-------|------------|
| Backend | ASP.NET Core 10 Minimal API |
| Frontend | Blazor WebAssembly |
| UI Components | MudBlazor |
| Database | SQLite + Entity Framework Core |
| Architecture | Clean Architecture + CQRS |

## Project Structure

```
InventoryHub/
├── src/
│   ├── InventoryHub.API/           # Minimal API endpoints
│   ├── InventoryHub.Application/   # Commands, Queries, Handlers, DTOs
│   ├── InventoryHub.Domain/        # Entities, Enums
│   ├── InventoryHub.Infrastructure/# DbContext, EF configurations
│   └── InventoryHub.Client/        # Blazor WebAssembly frontend
└── InventoryHub.sln
```

## Features

- **Products** — CRUD, stock tracking, low stock alerts
- **Categories** — Hierarchical product categorization
- **Suppliers** — Supplier management, product linking
- **Clients** — Customer database for sales orders
- **Purchase Orders** — Order workflow (Draft → Submitted → Received)
- **Sales Orders** — Sales workflow (Draft → Confirmed → Shipped → Delivered)
- **Inventory** — Stock movements (In/Out/Adjust) with full history
- **Dashboard** — Statistics, low stock alerts, recent movements

## Quick Start

### Prerequisites

- .NET 10 SDK

### Applications

| Application | URL | Description |
|-------------|-----|-------------|
| **API** | http://localhost:5000 | Backend REST API |
| **Client** | http://localhost:5001 | Frontend Blazor WebAssembly |

### Run the Application

```bash
# 1. Restore packages
dotnet restore InventoryHub/InventoryHub.sln

# 2. Apply database migrations
dotnet ef database update -p InventoryHub/src/InventoryHub.Infrastructure -s InventoryHub/src/InventoryHub.API

# 3. Run API (Terminal 1)
dotnet run --project InventoryHub/src/InventoryHub.API

# 4. Run Client (Terminal 2)
dotnet run --project InventoryHub/src/InventoryHub.Client
```

Open http://localhost:5001 in browser.

### Build

```bash
dotnet build InventoryHub/InventoryHub.sln
```

## API Endpoints

| Resource | Endpoints |
|----------|-----------|
| Products | `GET/POST /api/products`, `GET/PUT/DELETE /api/products/{id}` |
| Categories | `GET/POST /api/categories`, `GET/PUT/DELETE /api/categories/{id}` |
| Suppliers | `GET/POST /api/suppliers`, `GET/PUT/DELETE /api/suppliers/{id}` |
| Clients | `GET/POST /api/clients`, `GET/PUT/DELETE /api/clients/{id}` |
| Orders | `GET/POST /api/orders`, `POST /api/orders/{id}/submit`, `POST /api/orders/{id}/receive` |
| Sales Orders | `GET/POST /api/sales-orders`, workflow actions |
| Inventory | `GET/POST /api/inventory/movements` |
| Dashboard | `GET /api/dashboard/stats`, `GET /api/dashboard/low-stock` |

## Architecture

```
┌─────────────────────────────────────────────────────────┐
│                    Presentation                         │
│              (API Endpoints, Client)                    │
└─────────────────────────┬───────────────────────────────┘
                          │
┌─────────────────────────▼───────────────────────────────┐
│                    Application                          │
│         (Commands, Queries, Handlers, DTOs)             │
│                  MediatR + FluentValidation             │
└─────────────────────────┬───────────────────────────────┘
                          │
┌─────────────────────────▼───────────────────────────────┐
│                      Domain                             │
│              (Entities, Enums, Interfaces)              │
│                   No dependencies                       │
└─────────────────────────┬───────────────────────────────┘
                          │
┌─────────────────────────▼───────────────────────────────┐
│                   Infrastructure                        │
│            (DbContext, EF Configurations)               │
└─────────────────────────────────────────────────────────┘
```

## Domain Entities

| Entity | Description |
|--------|-------------|
| `Product` | SKU, Name, Price, CurrentStock, MinimumStock |
| `Category` | Hierarchical (self-referencing ParentId) |
| `Supplier` | Contact info, linked to Products |
| `Client` | Customer for sales orders |
| `PurchaseOrder` | Orders from suppliers (Stock In on receive) |
| `SalesOrder` | Orders to clients (Stock Out on ship) |
| `StockMovement` | In/Out/Adjust with quantity tracking |

## Database

SQLite database located at `InventoryHub/src/InventoryHub.API/inventory.db`

### Add Migration

```bash
dotnet ef migrations add <MigrationName> \
  -p InventoryHub/src/InventoryHub.Infrastructure \
  -s InventoryHub/src/InventoryHub.API
```

### Apply Migration

```bash
dotnet ef database update \
  -p InventoryHub/src/InventoryHub.Infrastructure \
  -s InventoryHub/src/InventoryHub.API
```

## Configuration

### API (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=inventory.db"
  }
}
```

### Client (`wwwroot/appsettings.json`)

```json
{
  "ApiBaseUrl": "http://localhost:5000"
}
```

### CORS

API configured to allow requests from `http://localhost:5001` (Blazor Client).

## Key Patterns

| Pattern | Usage |
|---------|-------|
| CQRS | Commands/Queries via MediatR |
| Result Pattern | `Result<T>.Success()` / `Result<T>.Failure()` |
| Validation | FluentValidation in Application layer |
| Mapping | Mapster for Entity ↔ DTO |

## License

This project is licensed under the [MIT License](LICENSE).
