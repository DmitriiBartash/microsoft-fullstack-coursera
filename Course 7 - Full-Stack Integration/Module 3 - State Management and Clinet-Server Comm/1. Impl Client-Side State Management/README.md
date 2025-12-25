# Blazor State App

Demo Blazor WebAssembly application for learning client-side state management using various browser storage APIs.

## Technologies

| Technology | Version | Purpose |
|------------|---------|---------|
| .NET | 10.0 | Platform |
| Blazor WebAssembly | - | Frontend framework |
| Fluxor | 6.9.0 | Redux-like state management |
| IndexedDB | TG.Blazor.IndexedDB 1.5.0 | Persistent storage |
| Blazored.LocalStorage | 4.5.0 | LocalStorage API |
| Blazored.SessionStorage | 2.4.0 | SessionStorage API |
| Bootstrap | 5.3.2 | UI framework |

## State Management (Fluxor)

### Redux Pattern

```
Action → Reducer → State → UI
           ↓
        Effect → Side Effects (IndexedDB, DOM)
```

### Cart Feature

| Component | Description |
|-----------|-------------|
| `CartState` | Immutable cart state (Items, IsLoading, LastSyncedAt) |
| `CartActions` | Load, Add, Remove, Update, Clear |
| `CartReducers` | Pure functions for state updates |
| `CartEffects` | Side effects for IndexedDB operations |

### Theme Feature

| Component | Description |
|-----------|-------------|
| `ThemeState` | Current theme (light/dark), IsLoading |
| `ThemeActions` | Load, Set |
| `ThemeReducers` | Theme state updates |
| `ThemeEffects` | Save to IndexedDB, apply to DOM |

## Data Storage Strategy

| Data | Storage | Reason |
|------|---------|--------|
| Cart | IndexedDB | Large volumes, structured data |
| Wishlist | LocalStorage | Simple data, persists between sessions |
| Compare | SessionStorage | Temporary data, clears on browser close |
| Theme | IndexedDB | User preferences |

## Getting Started

```bash
cd src/BlazorStateApp.Client

# Restore packages
dotnet restore

# Run
dotnet run

# Or with hot reload
dotnet watch run
```

Application will be available at: `http://localhost:5096`

## Features

### Home Page (/)
- Product catalog with 9 items
- Add to cart (IndexedDB)
- Add to wishlist (LocalStorage)
- Add to compare (SessionStorage, max 4)
- View and edit cart
- Checkout order

### Compare Page (/cart)
- Product comparison table
- Price analysis (Best Deal, ranking)
- Data clears on browser close

### Wishlist Page (/wishlist)
- Saved products list
- Quick add to cart
- Data persists between sessions

### Theme
- Light/Dark mode toggle
- User preference saved
- Bootstrap `data-bs-theme` applied

## Key Patterns

- **Fluxor/Redux** — Predictable state management
- **Repository Pattern** — `IStorageService` abstraction
- **Immutable State** — Records for state
- **Separation of Concerns** — Reducers (pure) vs Effects (side effects)
- **DRY** — Centralized `ProductCatalog`
