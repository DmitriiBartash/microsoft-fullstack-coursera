# SecureNotes — JWT Token-Based Authentication

A single-page application demonstrating JWT token-based authentication with ASP.NET Identity. The focus is on **token lifecycle mechanics**: generation, validation, refresh token rotation, revocation, and real-time JWT inspection on the frontend.

## Key Features

- **Short-lived Access Tokens (5 min)** — intentionally short to demonstrate the refresh flow in real time
- **Refresh Token Rotation** — old token is revoked on each refresh; new one is issued
- **JWT Inspector** — client-side base64url decode showing header, payload (claims table), signature, and a live expiry countdown
- **Auto-Refresh** — triggers 30 seconds before expiry; retries on 401 with transparent token renewal
- **Owner Isolation** — notes are filtered by `userId` from JWT claims; no cross-user access
- **No Roles** — pure token-focused auth (roles & claims are covered in the previous lab)

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend | ASP.NET (.NET 10), Minimal API, ASP.NET Identity |
| Auth | JWT Bearer (access + refresh tokens) |
| Database | EF Core InMemory |
| Frontend | Vanilla JS (ES Modules), Tailwind CSS (CDN) |
| Docs | Swagger / OpenAPI |

## Getting Started

```bash
cd SecureNotes/src/SecureNotes.Api
dotnet run
```

Open http://localhost:5150 in your browser.
Swagger UI is available at http://localhost:5150/swagger.

## Demo Accounts

| Email | Password | Notes |
|-------|----------|-------|
| `demo@notes.com` | `Demo123` | 3 sample notes (Welcome, Token Lifecycle, Security Best Practices) |
| `alice@notes.com` | `Alice123` | 2 sample notes (demonstrates owner isolation) |

## Project Structure

```
SecureNotes/
  src/SecureNotes.Api/
    Program.cs
    appsettings.json                         # JWT settings (key, issuer, audience, expiration)
    Domain/Entities/                         # ApplicationUser, Note, RefreshToken
    Infrastructure/
      Data/ApplicationDbContext.cs            # IdentityDbContext + owned RefreshTokens
      Seed/SeedDataService.cs                # Demo users and sample notes
    Services/
      TokenService.cs                        # JWT generation, refresh token, validate expired
      AuthService.cs                         # Register, Login, Refresh (rotation), Logout
      NoteService.cs                         # CRUD with owner isolation
    Contracts/                               # Request/Response DTOs
    Endpoints/
      AuthEndpoints.cs                       # POST register, login, refresh, logout
      NoteEndpoints.cs                       # GET/POST/PUT/DELETE (RequireAuthorization)
    Middleware/ExceptionHandlingMiddleware.cs # ProblemDetails responses
    Extensions/                              # DI wiring, JWT config, Swagger setup
    wwwroot/                                 # SPA frontend
      js/auth.js                             # Token storage, decode, countdown, auto-refresh
      js/api.js                              # Fetch wrapper with 401 retry
      js/router.js                           # Hash-based SPA router with auth guard
      js/components/jwt-inspector.js         # Real-time JWT decode and inspection
```

## API Endpoints

### Auth (`/api/auth`)

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| POST | `/register` | No | Create account, returns access + refresh tokens |
| POST | `/login` | No | Authenticate, returns access + refresh tokens |
| POST | `/refresh` | No | Exchange expired access token + valid refresh token for new pair |
| POST | `/logout` | Yes | Revoke all refresh tokens for the user |

### Notes (`/api/notes`)

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| GET | `/` | Yes | List all notes for the authenticated user |
| GET | `/{id}` | Yes | Get a specific note (owner only) |
| POST | `/` | Yes | Create a new note |
| PUT | `/{id}` | Yes | Update a note (owner only) |
| DELETE | `/{id}` | Yes | Delete a note (owner only) |

## Token Configuration

```json
{
  "Jwt": {
    "AccessTokenExpirationMinutes": 5,
    "RefreshTokenExpirationDays": 7
  }
}
```

- `ClockSkew` is set to `TimeSpan.Zero` for precise expiration behavior
- Refresh tokens are rotated on every use (old one is revoked)
- Logout revokes all active refresh tokens
