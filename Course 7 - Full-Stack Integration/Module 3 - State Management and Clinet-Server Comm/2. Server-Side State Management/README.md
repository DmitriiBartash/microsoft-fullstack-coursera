# Currency Exchange Dashboard

Blazor Server application demonstrating server-side state management techniques.

## Features

- Real-time exchange rates from [Frankfurter API](https://www.frankfurter.app/)
- Currency converter with historical charts
- Virtual wallet for buying/selling currencies
- User preferences (theme, default currency)

## State Management

| Layer | Technology | Scope |
|-------|------------|-------|
| Server cache | `IMemoryCache` | Application-wide |
| User state | Session Storage | Per-user session |

## Project Structure

```
CurrencyExchangeDashboard/
├── Models/           # Data models (records)
├── Services/         # Business logic & API
│   └── Interfaces/   # Service contracts
└── Components/       # Blazor UI
    ├── Pages/        # Routable pages
    ├── Shared/       # Reusable components
    └── Layout/       # App layout
```

## Run

```bash
cd CurrencyExchangeDashboard
dotnet run
```

## Test

```bash
dotnet test
```

## Tech Stack

- .NET 10 / Blazor Server
- MudBlazor (UI components)
- Blazor-ApexCharts (charts)
- Blazored.SessionStorage
- xUnit + Moq (testing)
