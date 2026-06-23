# Securing API Endpoints with JWTs

**Course 8 — Security and Authentication** · Module 2 · Lesson 6: *Securing API Endpoints* · `You Try It!`

> Build an ASP.NET Core **Web API** that authenticates with JWTs **and enforces role-based
> access control**. A client logs in, receives a **signed JWT carrying role claims**, and uses
> it as a `Bearer` token. The JWT Bearer handler validates every token; `[Authorize(Roles = …)]`
> then decides *who may do what* — returning **401** without a token and **403** when the token
> is valid but the role is wrong.

---

## 🎯 Objective

Secure API endpoints using JWT authentication and **role-based access control (RBAC)**: issue
tokens that carry the caller's roles, configure the middleware to read those roles, and protect
endpoints so only authorized roles can reach them. By the end, only authenticated users with the
right role can access each secured route.

---

## 🗂️ What you'll build

A Web API project named **`SecureApiWithJwt`** with these pieces:

| File                              | Responsibility                                                                  |
| --------------------------------- | ------------------------------------------------------------------------------- |
| `appsettings.json`                | Define `JwtSettings` — `Key`, `Issuer`, `Audience`, `DurationInMinutes`          |
| `Program.cs`                      | Register JWT Bearer auth; set `TokenValidationParameters` (incl. **role claim mapping**); wire `UseAuthentication` → `UseAuthorization` |
| `Models/User.cs`                  | User model — `Username`, `Password`, and one or more `Roles`                     |
| `Services/TokenService.cs`        | Read `JwtSettings`; build & sign a token with **username + role claims**         |
| `Controllers/AuthController.cs`   | `POST /auth/login` — validate credentials, return a JWT (401 on failure)         |
| `Controllers/ValuesController.cs` | `GET` endpoints guarded with `[Authorize(Roles = "Admin")]` / `"User"`           |

**Flow:**
`POST /auth/login → validate(store) → TokenService.GenerateToken(user, roles) → JWT → GET /values [Bearer] → JwtBearer validates → role check → 200 / 401 / 403`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code
- NuGet packages: `Microsoft.AspNetCore.Authentication.JwtBearer`, `System.IdentityModel.Tokens.Jwt`, `Microsoft.IdentityModel.Tokens`
- A REST client to test — **Postman** or `curl`

---

## 🛠️ Steps  ·  `0 of 6 completed`

### Step 1 — Prepare the application

Scaffold the Web API, move into it, and add the JWT packages.

```bash
dotnet new webapi -o SecureApiWithJwt
cd SecureApiWithJwt
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package System.IdentityModel.Tokens.Jwt --version 7.2.0
dotnet add package Microsoft.IdentityModel.Tokens --version 7.2.0
```

- Remove the default **WeatherForecast** files (`WeatherForecast.cs`, `Controllers/WeatherForecastController.cs`) to clean up the project.
- Open `Program.cs` and `appsettings.json` for configuration.

> **Pin `JwtBearer` to your SDK's major version** (e.g. `--version 8.0.*` on .NET 8) so it matches the runtime.

### Step 2 — Add configuration for JWT authentication

1. Add a `JwtSettings` section to `appsettings.json`:

```json
"JwtSettings": {
  "Key": "SuperSecretKeyForJwtTokenAuthorization123456789",
  "Issuer": "SecureApi",
  "Audience": "SecureApiUsers",
  "DurationInMinutes": 60
}
```

2. Configure JWT authentication in `Program.cs`:
   - Register JWT Bearer authentication in the service container.
   - Set up `TokenValidationParameters` to validate **issuer, audience, lifetime, and signing key**.
   - **Map the roles claim** (`RoleClaimType`) so `[Authorize(Roles = …)]` can read roles from the token.
   - Ensure the pipeline runs **authentication** then **authorization** middleware.

### Step 3 — Implement token generation

- Create a `TokenService` class to handle JWT creation, reading the values from `JwtSettings`.
- When generating a token, add **claims for the username and the user's roles** (one role claim per role).
- Expose a method such as `GenerateToken(User user)` returning the signed token string.

### Step 4 — Add a login endpoint

- Create an `AuthController` in the `Controllers` folder.
- Implement a `POST /auth/login` endpoint:
  - Validate the supplied credentials against the user store.
  - If valid → call `TokenService` and return the JWT.
  - If invalid → return **401 Unauthorized**.

### Step 5 — Secure API endpoints

- Create a `ValuesController` with `GET` endpoints.
- Restrict access by role using the `[Authorize]` attribute:
  - `[Authorize(Roles = "Admin")]` — admin-only data.
  - `[Authorize(Roles = "Admin,User")]` — data for either role.
- Confirm an unprotected/wrong-role request is rejected.

### Step 6 — Test the application

Use **Postman** (or `curl`) to:

1. Generate a token by sending a `POST` request to `/auth/login` with valid credentials.
2. Call the secured endpoints with `Authorization: Bearer <token>` and verify role-based restrictions:

```bash
# 1) log in as an Admin → copy the returned token
curl -X POST http://localhost:5000/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"password"}'

# 2) Admin token reaching an Admin-only route → 200
curl http://localhost:5000/values/admin -H "Authorization: Bearer <token>"

# 3) no token → 401  ·  valid token but wrong role → 403
curl -i http://localhost:5000/values/admin
```

---

## ▶️ Expected result

Logging in with valid credentials returns a **signed JWT carrying role claims**. With that token:

- A route allowed for the caller's role returns **200 OK**.
- A request with **no/invalid/expired token** returns **401 Unauthorized**.
- A request with a **valid token but the wrong role** returns **403 Forbidden**.

---

## ☑️ Definition of done

- [ ] `SecureApiWithJwt` Web API created; `JwtBearer`, `System.IdentityModel.Tokens.Jwt`, `Microsoft.IdentityModel.Tokens` added
- [ ] Default WeatherForecast files removed
- [ ] `appsettings.json` defines `JwtSettings` (key, issuer, audience, duration)
- [ ] `Program.cs` registers JWT Bearer auth, validates the token, **maps the role claim**, and runs authentication → authorization
- [ ] `TokenService` issues a signed token with **username + role** claims
- [ ] `AuthController` exposes `POST /auth/login` (JWT on success, 401 on failure)
- [ ] `ValuesController` endpoints are protected with `[Authorize(Roles = …)]`
- [ ] Verified: right role → **200**, no token → **401**, wrong role → **403**

---

## 🏗️ Implementation notes

> The reference solution in this folder is **enriched beyond the literal steps** for teaching and
> testability. The deviations below are intentional:
>
> - **Layering** — `User`, `UserStore`, `TokenService`, and `JwtSettings` live in a shared
>   **`SecureApiCore`** class library, so the API and the visual studio reuse one implementation.
>   `SecureApiWithJwt` references it.
> - **Packages** — `System.IdentityModel.Tokens.Jwt` + `Microsoft.IdentityModel.Tokens` sit in
>   `SecureApiCore` (token creation); the API holds `Microsoft.AspNetCore.Authentication.JwtBearer`
>   and gets the rest transitively.
> - **User store** — centralised in `SecureApiCore.UserStore` (a `static` hardcoded list with roles)
>   and injected via DI, rather than declared in a controller.
> - **`SecureApiStudio`** — a Blazor "Security Studio" that drives the real `JwtBearer` + role guard
>   in-process, making the **login → token → 200/401/403** handshake visible per role.
> - **`SecureApiWithJwt.Tests`** — xUnit integration tests (`WebApplicationFactory`) covering login,
>   admin-only vs shared routes, and missing / wrong-role / tampered / expired tokens — the in-memory
>   `TestServer` stands in for the manual Postman step.

---

## 🔑 Key concepts

- **401 vs 403** — the JWT Bearer handler returns **401 Unauthorized** when a token is missing,
  malformed, or expired (*authentication* failed); `[Authorize(Roles = …)]` returns **403 Forbidden**
  when the token is valid but lacks the required role (*authorization* failed). Different failures,
  different codes.
- **Role claim mapping** — `[Authorize(Roles = …)]` reads roles from whichever claim `RoleClaimType`
  points at. If the token writes roles under `"role"` but the validator expects
  `ClaimTypes.Role`, every role check fails — map them deliberately so they agree.
- **Roles travel in the token** — RBAC is stateless: the signed JWT carries the role claims, so the
  API authorizes from the token alone with no per-request database lookup.
- **Middleware order** — `UseAuthentication()` must run **before** `UseAuthorization()`; otherwise the
  user's identity (and roles) isn't established when the authorization check runs.
- **Protect the secret & passwords** — the `appsettings.json` key is fine for a local lab, but
  production keys belong in user-secrets / environment variables / a vault, and real credentials are
  stored as salted hashes (bcrypt / Argon2), never plaintext. HS256 needs a key ≥ 256 bits (32+ bytes).
