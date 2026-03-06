# OAuthFlow — OAuth 2.0 Authorization Code Flow Demo

Educational project demonstrating the complete OAuth 2.0 Authorization Code Flow
with two ASP.NET applications: an Authorization Server and a Client.

## Projects

| Project | Port | Description |
|---------|------|-------------|
| `OAuthFlow.Server` | 5001 | Authorization Server (Minimal API) — issues authorization codes and JWT tokens |
| `OAuthFlow.Client` | 5002 | Razor Pages Client — initiates OAuth flow, displays dashboard with JWT inspector and flow timeline |

## Quick Start

```bash
# Terminal 1
dotnet run --project src/OAuthFlow.Server

# Terminal 2
dotnet run --project src/OAuthFlow.Client
```

Open `http://localhost:5002` and click **Login with OAuth**.

### Demo Credentials

| Username | Password |
|----------|----------|
| `demo`   | `password` |
| `alice`  | `password` |

## OAuth 2.0 Flow

```
Browser                     Client (:5002)                Server (:5001)
  |                              |                              |
  |-- Click "Login" ----------->|                              |
  |                              |-- Generate state             |
  |<-- 302 to /authorize -------|                              |
  |                                                             |
  |-- GET /authorize ----------------------------------------->|
  |<-- Consent page (HTML) ------------------------------------|
  |                                                             |
  |-- POST /authorize (credentials + approve) ---------------->|
  |                                                             |-- Validate user
  |                                                             |-- Generate code
  |<-- 302 to /Callback?code=...&state=... --------------------|
  |                                                             |
  |-- GET /Callback ----------->|                              |
  |                              |-- Validate state             |
  |                              |-- POST /token (code) ------>|
  |                              |<-- { access_token } --------|
  |                              |-- GET /userinfo (Bearer) -->|
  |                              |<-- { name, email } ---------|
  |                              |                              |
  |<-- 302 to /Dashboard -------|                              |
```

## Server Endpoints

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| GET | `/authorize` | No | Shows consent page |
| POST | `/authorize` | No | Processes login and consent |
| POST | `/token` | No | Exchanges authorization code for JWT |
| GET | `/userinfo` | Bearer | Returns authenticated user info |
| GET | `/swagger` | No | API documentation |

## Dashboard Features

- **User Profile** — display name, email, avatar
- **JWT Token Inspector** — decoded header (red), payload (purple), signature (blue)
- **Flow Timeline** — 6-step vertical stepper with expandable request/response details

## Tech Stack

- .NET 10, C# 13
- Minimal API (Server), Razor Pages (Client)
- JWT via `System.IdentityModel.Tokens.Jwt`
- Pure CSS (no frameworks)
- In-memory stores (no database)
