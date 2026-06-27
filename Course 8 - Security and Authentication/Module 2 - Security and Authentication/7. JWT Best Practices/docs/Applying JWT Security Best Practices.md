# Applying JWT Security Best Practices

**Course 8 — Security and Authentication** · Module 2 · Lesson 7: *JWT Best Practices* · `You Try It!`

> Build an ASP.NET Core **Web API** that authenticates with JWTs the way production systems do:
> a **short-lived access token** carried as a `Bearer` header, plus a **long-lived refresh token**
> kept in an **HttpOnly cookie**. When the access token expires the client silently calls
> `/auth/refresh`, the server **rotates** the refresh token (single-use, with reuse detection),
> and the session continues — no re-login, minimal blast radius if a token leaks.

---

## 🎯 Objective

Apply JWT security best practices: keep access tokens short-lived, renew them with rotating refresh
tokens stored in HttpOnly cookies, sign with a strong key sourced from configuration, and protect
endpoints with role-based `[Authorize]`. By the end you can log in, ride out access-token expiry via
silent refresh, and see a stolen/replayed refresh token get rejected.

---

## 🗂️ What you'll build

A Web API project named **`JwtAuthExample`** with these pieces:

| File                                | Responsibility                                                                       |
| ----------------------------------- | ------------------------------------------------------------------------------------ |
| `appsettings.json`                  | `JwtSettings` — `Key`, `Issuer`, `Audience`, `AccessTokenMinutes`, `RefreshTokenDays` |
| `Program.cs`                        | Register JWT Bearer auth; set `TokenValidationParameters`; refuse a weak key outside Development; wire `UseAuthentication` → `UseAuthorization` |
| `Models/User.cs`                    | User model — `Username`, `Password`, one or more `Roles`                              |
| `Services/TokenService.cs`          | Build & sign a short-lived access token with **username + role** claims              |
| `Services/RefreshTokenStore.cs`     | In-memory refresh tokens with **rotation + reuse detection**                          |
| `Controllers/AuthController.cs`     | `POST /auth/login`, `POST /auth/refresh`, `POST /auth/logout` (refresh token in an HttpOnly cookie) |
| `Controllers/SecureController.cs`   | `GET /secure` (`[Authorize]`) and `GET /secure/admin` (`[Authorize(Roles="Admin")]`) |

**Flow:**
`POST /auth/login → access token (body) + refresh cookie (HttpOnly) → GET /secure [Bearer] → 200 …
access expires → 401 → POST /auth/refresh [cookie] → rotate → new access token → 200`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code
- NuGet packages: `Microsoft.AspNetCore.Authentication.JwtBearer`, `System.IdentityModel.Tokens.Jwt`, `Microsoft.IdentityModel.Tokens`
- A REST client — **Postman** or `curl` (a cookie jar helps for the refresh step)

---

## 🛠️ Steps  ·  `0 of 6 completed`

### Step 1 — Prepare the application

```bash
dotnet new webapi -o JwtAuthExample
cd JwtAuthExample
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package System.IdentityModel.Tokens.Jwt
dotnet add package Microsoft.IdentityModel.Tokens
```

- Remove the default **WeatherForecast** files.
- Open `Program.cs` and `appsettings.json`.

> **Pin `JwtBearer` to your SDK's major version** (e.g. `--version 8.0.*` on .NET 8) so it matches the runtime.

### Step 2 — Configure JWT authentication

1. Add a `JwtSettings` section to `appsettings.json` — note the **split lifetimes**:

```json
"JwtSettings": {
  "Key": "SuperSecretKeyForJwtTokenAuthorization123456789",
  "Issuer": "JwtAuthExample",
  "Audience": "JwtAuthExampleUsers",
  "AccessTokenMinutes": 15,
  "RefreshTokenDays": 7
}
```

2. In `Program.cs`:
   - Register JWT Bearer auth; set `TokenValidationParameters` to validate **issuer, audience, lifetime, signing key** with `ClockSkew = TimeSpan.Zero`.
   - **Map the role claim** (`RoleClaimType`) so `[Authorize(Roles = …)]` can read roles.
   - The `Key` in `appsettings.json` is a **development default only** — supply the real one from an environment variable (`JwtSettings__Key`) or a secret store. Refuse to start outside Development with a weak/default key.

### Step 3 — Implement user authentication

- Create `TokenService` reading `JwtSettings`; issue a **short-lived** access token with **username + role** claims, signed HS256.
- Add `POST /auth/login`: validate credentials against the user store; on success return the access token (and its lifetime); on failure return **401**.

### Step 4 — Add refresh-token functionality

- Store refresh tokens in a simple in-memory structure (`RefreshTokenStore`): an opaque high-entropy string bound to a user, with an expiry.
- `POST /auth/refresh`: read the refresh token **from the cookie**, validate it, and issue a **new access token**.
- Set the refresh token as a cookie with `HttpOnly`, `Secure`, `SameSite=Strict`, and a path scoped to `/auth`.
- **Best practice — rotation:** every refresh **revokes** the presented token and issues a **new** one (single-use). Re-presenting a used token (the signature of a replayed/stolen token) is **reuse**, and revokes the whole token family.

### Step 5 — Protect API endpoints

- Add `SecureController`:
  - `GET /secure` → `[Authorize]` (any authenticated caller).
  - `GET /secure/admin` → `[Authorize(Roles = "Admin")]` (Admin only).
- Confirm: valid token → **200**, no/expired/tampered token → **401**, valid token but wrong role → **403**.

### Step 6 — Test and debug

```bash
# 1) log in → access token in body, refresh_token set as an HttpOnly cookie (save the cookie jar)
curl -i -c jar.txt -X POST http://localhost:5000/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"password"}'

# 2) call a protected route with the access token → 200
curl http://localhost:5000/secure -H "Authorization: Bearer <token>"

# 3) renew with the cookie → new access token, rotated refresh cookie
curl -i -b jar.txt -c jar.txt -X POST http://localhost:5000/auth/refresh

# 4) no token → 401 · valid User token on /secure/admin → 403
curl -i http://localhost:5000/secure
```

---

## ▶️ Expected result

- Login returns a **signed, short-lived access token** and sets an **HttpOnly refresh cookie**.
- A valid access token reaches `/secure` with **200**; once it expires, `/secure` returns **401**.
- `POST /auth/refresh` with the cookie returns a **fresh access token** and **rotates** the refresh token.
- Replaying a **rotated-out** refresh token returns **401** (reuse detection); a **wrong-role** token gets **403**.

---

## ☑️ Definition of done

- [ ] `JwtAuthExample` Web API created; JWT packages added; WeatherForecast removed
- [ ] `appsettings.json` defines `JwtSettings` with split `AccessTokenMinutes` / `RefreshTokenDays`
- [ ] `Program.cs` registers JWT Bearer auth, validates the token, **maps the role claim**, and guards against a weak key outside Development
- [ ] `TokenService` issues a short-lived signed token with **username + role** claims
- [ ] `RefreshTokenStore` stores refresh tokens and **rotates** them with **reuse detection**
- [ ] `AuthController` exposes `login` / `refresh` / `logout`; the refresh token rides in an **HttpOnly, Secure, SameSite=Strict** cookie
- [ ] `SecureController` endpoints are protected with `[Authorize]` / `[Authorize(Roles = …)]`
- [ ] Verified: access expiry → 401 → refresh → 200; rotated-token replay → 401; wrong role → 403

---

## 🏗️ Implementation notes

> The reference solution is **enriched beyond the literal steps** for teaching and testability. The
> deviations below are intentional:
>
> - **Layering** — `User`, `UserStore`, `JwtSettings`, `TokenService` and `RefreshTokenStore` live in a
>   shared **`JwtSecurityCore`** class library, so the API, the tests and the Studio reuse one implementation.
> - **`JwtSecurityStudio`** — a Blazor "JWT Security Studio" that drives the genuine `JwtBearer` handler and
>   the real refresh/cookie endpoints in-process, making the **access/refresh lifecycle** visible: a live
>   dual countdown, an auto-pilot that hits 401 on expiry and silently refreshes, the rotation ledger, the
>   captured `Set-Cookie`, and attack replays (rotated-token reuse, tampered token, wrong role).
> - **Refresh-token rotation + reuse detection** — beyond the literal "issue a new access token", the store
>   makes refresh tokens single-use and revokes the family on reuse, the current OWASP guidance.
> - **`JwtAuthExample.Tests`** — xUnit integration tests (`WebApplicationFactory`) covering login + cookie,
>   access expiry, refresh, rotation-replay rejection, logout revocation, tampered tokens and RBAC — the
>   in-memory `TestServer` stands in for the manual Postman step.

---

## 🔑 Key concepts

- **Short access + long refresh** — a short access-token lifetime limits the damage window if it leaks;
  the refresh token renews it without forcing re-login. Two tokens, two jobs.
- **HttpOnly cookies** — keeping the refresh token in an `HttpOnly` cookie means page scripts (and thus XSS
  payloads) can't read it via `document.cookie`. `Secure` keeps it to HTTPS; `SameSite=Strict` blocks
  cross-site sends; scoping the `Path` to `/auth` means it's only ever sent to the refresh endpoint.
- **Refresh-token rotation** — each refresh consumes the old token and issues a new one. If an old token is
  ever presented again, that's a sign it was stolen and replayed — revoke the whole family and force a login.
- **401 vs 403** — a missing/expired/tampered token fails *authentication* → **401**; a valid token without
  the required role fails *authorization* → **403**. The 401 is the client's cue to refresh.
- **Protect the secret** — the signing key is a secret: load it from an environment variable / user-secrets /
  a vault, never source control. HS256 needs a key ≥ 256 bits (32+ bytes); real passwords are stored as
  salted hashes (bcrypt / Argon2), never plaintext.
- **Middleware order** — `UseAuthentication()` must run **before** `UseAuthorization()`, or the identity
  (and roles) isn't established when the authorization check runs.
