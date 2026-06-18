# Implementing JWT Authentication

**Course 8 — Security and Authentication** · Module 2 · Lesson 4: *Implementing JWT Authentication in ASP.NET Core* · `You Try It!`

> Build a small ASP.NET Core **Web API** that issues and verifies JWTs. A client logs in
> against an **in-memory user store**, receives a **signed JWT**, and then uses it as a
> `Bearer` token to reach a protected endpoint. Authentication middleware validates every
> token; the `[Authorize]` attribute guards the secured route and returns **401** without one.

---

## 🎯 Objective

Implement JWT authentication in an ASP.NET Core application using Visual Studio Code:
create an in-memory user store, configure the authentication middleware, issue signed
tokens on login, and secure API endpoints so only authenticated callers can reach them.

---

## 🗂️ What you'll build

A Web API project named **`JwtAuthDemo`** with these pieces:

| File                            | Responsibility                                                         |
| ------------------------------- | --------------------------------------------------------------------- |
| `appsettings.json`              | Define `JwtSettings` — secret key, issuer, audience, expiry           |
| `Program.cs`                    | Register JWT Bearer auth; wire `UseAuthentication` → `UseAuthorization`|
| `Models/User.cs`                | User model — `Username` and `Password`                                |
| `Services/TokenService.cs`      | Read `JwtSettings`; build & sign a token via `GenerateToken(username)` |
| `Controllers/UserController.cs` | In-memory user store; `POST /login`; protected `GET /secure-data`     |

**Flow:** `POST /login → validate(store) → TokenService.GenerateToken() → JWT → GET /secure-data [Bearer] → JwtBearer validates → 200 / 401`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code
- NuGet packages: `Microsoft.AspNetCore.Authentication.JwtBearer`, `System.IdentityModel.Tokens.Jwt`
- A REST client to test — Postman or `curl`

---

## 🛠️ Steps

### Step 1 — Prepare the application

Scaffold the Web API, move into it, and add the JWT packages.

```bash
dotnet new webapi -n JwtAuthDemo
cd JwtAuthDemo
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package System.IdentityModel.Tokens.Jwt
```

- Create a `Controllers/UserController.cs` file (filled in over the next steps).

### Step 2 — Configure JWT authentication

Tell the app how to validate tokens.

- Add a `JwtSettings` section to `appsettings.json`: **secret key**, **issuer**, **audience**, and **expiration** time.
- In `Program.cs`:
  - Register JWT Bearer authentication in the service container.
  - Configure the middleware to validate incoming tokens against the secret key (issuer, audience, lifetime, signing key).
  - Ensure the pipeline runs **authentication** then **authorization** middleware.

### Step 3 — User model and token generation

- Create `Models/User.cs` with `Username` and `Password` string properties.
- Create `Services/TokenService.cs`:
  - Read `JwtSettings` from configuration.
  - Use the `System.IdentityModel.Tokens.Jwt` namespace to build and **sign** a JWT.
  - Implement `GenerateToken(string username)` returning the signed token string.

### Step 4 — In-memory user store + login

- In `UserController`, declare a **static list** of `User` objects (hardcoded credentials).
- Implement `POST /login`:
  - Validate the supplied `Username` / `Password` against the store.
  - If valid → use `TokenService` to generate a JWT and return it.
  - If invalid → return **401 Unauthorized**.

### Step 5 — Secure the routes

- Add a `GET /secure-data` endpoint protected with the **`[Authorize]`** attribute; return a simple "you reached a secure endpoint" message.
- Test with Postman or `curl` — send the token in the `Authorization` header with the `Bearer` scheme, and confirm requests without a valid token get **401**.

```bash
# 1) log in → copy the returned token
curl -X POST http://localhost:5000/login -H "Content-Type: application/json" -d '{"username":"admin","password":"password"}'

# 2) call the protected route with the token
curl http://localhost:5000/secure-data -H "Authorization: Bearer <token>"
```

---

## ▶️ Expected result

Logging in with valid credentials returns a **signed JWT**. Calling `GET /secure-data`
with that token in the `Authorization: Bearer …` header returns the secure message;
calling it with **no token, a tampered token, or an expired one** returns **401 Unauthorized**.

---

## ☑️ Definition of done

- [ ] `JwtAuthDemo` Web API created; `…JwtBearer` and `System.IdentityModel.Tokens.Jwt` packages added
- [ ] `appsettings.json` defines `JwtSettings` (key, issuer, audience, expiry)
- [ ] `Program.cs` registers JWT Bearer auth and uses authentication → authorization middleware
- [ ] `Models/User.cs` and `Services/TokenService.cs` (`GenerateToken`) created
- [ ] `UserController` exposes `POST /login` (JWT on success, 401 on failure)
- [ ] `GET /secure-data` is `[Authorize]`-protected
- [ ] A valid Bearer token reaches `/secure-data` (200); a missing/invalid token is rejected (401)

---

## 🏗️ Implementation notes

> The reference solution in this folder is **enriched beyond the literal steps** for teaching and
> testability. The deviations below are intentional:
>
> - **Layering** — `User`, `UserStore`, `TokenService` and `JwtSettings` live in a shared
>   **`JwtAuthCore`** class library (not inside `JwtAuthDemo`), so the API and the visual Studio reuse
>   one implementation. `JwtAuthDemo` references it.
> - **Packages** — `System.IdentityModel.Tokens.Jwt` sits in `JwtAuthCore` (token creation);
>   `JwtAuthDemo` holds `Microsoft.AspNetCore.Authentication.JwtBearer` and gets the former transitively.
> - **User store** — centralised in `JwtAuthCore.UserStore` (still a `static` hardcoded list) and
>   injected into `UserController` via DI, rather than declared in the controller.
> - **`JwtAuthStudio`** — a Blazor "Auth Studio" that exercises the real `JwtBearer` guard in-process,
>   making the login → token → 200/401 handshake visible.
> - **`JwtAuthDemo.Tests`** — xUnit integration tests (`WebApplicationFactory`) for login, the protected
>   route, and missing / tampered / expired tokens — the in-memory `TestServer` stands in for the manual
>   curl/Postman step.
> - **Enrichments** — a `role` claim, a configurable-lifetime `GenerateToken` overload, and a second demo account.

---

## 🔑 Key concepts

- **Authentication vs authorization** — the JWT Bearer handler *authenticates* (is this token genuine and unexpired?); `[Authorize]` enforces *authorization* (only authenticated callers pass). Order matters: `UseAuthentication()` must run **before** `UseAuthorization()`.
- **Stateless sessions** — the signed token itself carries the identity. The server validates the signature with the shared secret and keeps no session, which is what lets JWT auth scale horizontally.
- **`TokenValidationParameters`** — issuer, audience, lifetime *and* signing key are all checked. Disabling any (e.g. `ValidateIssuer = false`) widens the attack surface — only loosen them deliberately.
- **Protect the secret** — a key in `appsettings.json` is fine for a local lab, but production keys belong in user-secrets / environment variables / a vault, never in source control. For HS256 the key must be ≥ 256 bits (32+ bytes).
- **Never store plaintext passwords** — the hardcoded in-memory store is a teaching shortcut. Real systems store a salted hash (bcrypt / Argon2) and compare hashes, never the raw password.
