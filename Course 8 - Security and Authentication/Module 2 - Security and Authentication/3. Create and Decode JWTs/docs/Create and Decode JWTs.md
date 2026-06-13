# Create and Decode JWTs

**Course 8 — Security and Authentication** · Module 2 · Lesson 3: *Create and Decode JWTs* · `You Try It!`

> Build a small .NET console app that **generates** a signed JWT and then **decodes &
> verifies** it. The point of the lab is *modularization*: token creation and decoding
> each live in their own single-responsibility class (`JwtCreator`, `JwtDecoder`),
> and `Program.cs` just wires them together.

---

## 🎯 Objective

Learn how to create and decode JWTs in a .NET (ASP.NET Core) console application by
breaking the JWT functionality into **separate classes**, making each piece easier to
understand, test, and reuse.

---

## 🗂️ What you'll build

A console project named **`JwtDemo`** made of three files:

| File            | Responsibility                                          |
| --------------- | ------------------------------------------------------- |
| `JwtCreator.cs` | Generate (sign) a JWT                                   |
| `JwtDecoder.cs` | Decode a JWT **and validate its signature**             |
| `Program.cs`    | Orchestrate: create a token, then hand it to the decoder|

**Flow:** `Program  →  JwtCreator.Create()  →  token  →  JwtDecoder.Decode(token)  →  claims`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code
- NuGet package: `System.IdentityModel.Tokens.Jwt`

---

## 🛠️ Steps

### Step 1 — Prepare the application

Scaffold the console app, move into it, and add the JWT package.

```bash
dotnet new console -n JwtDemo
cd JwtDemo
dotnet add package System.IdentityModel.Tokens.Jwt
```

- Open `Program.cs` and **clear any existing content**.

### Step 2 — Create a `JwtCreator` class

This class handles JWT **generation**.

- Create a new file named **`JwtCreator.cs`**.
- Define a class with a method that **generates a JWT** — build the claims, sign them with
  a secret key (`SigningCredentials`), and return the serialized token string.

### Step 3 — Create a `JwtDecoder` class

This class handles JWT **decoding**.

- Create a new file named **`JwtDecoder.cs`**.
- Define a class with a method that **decodes a JWT and validates its signature** — configure
  `TokenValidationParameters`, validate the token, then read its claims.

### Step 4 — Use the classes in `Main`

In the `Main` method, use `JwtCreator` and `JwtDecoder` together.

- Call the JWT **creation** method from `JwtCreator`.
- Pass the generated token to `JwtDecoder` for **decoding and validation**.
- Run the application:

```bash
dotnet run
```

---

## ▶️ Expected result

The app prints the **generated token**, then the **decoded & validated claims**
(e.g. subject, role, expiry) — proving the signature checks out end-to-end.

---

## ☑️ Definition of done

- [ ] `JwtDemo` console project created and `System.IdentityModel.Tokens.Jwt` added
- [ ] `JwtCreator.cs` generates a **signed** token
- [ ] `JwtDecoder.cs` **validates the signature** and reads the claims
- [ ] `Program.cs` wires creation → decoding in `Main`
- [ ] `dotnet run` outputs a token and its decoded claims

---

## 🔑 Key concepts

- **Separation of concerns** — creation and decoding are independent classes, so each has a
  single reason to change and can be tested in isolation.
- **Symmetric signing (HS256)** — the *same* secret key both signs and validates; keep it out
  of source control (use configuration / environment variables, not a hard-coded literal).
- **Validation ≠ decoding** — anyone can Base64URL-decode the payload; only signature
  *validation* proves the token is authentic and untampered.
- **`exp` matters** — always validate the expiration claim server-side and reject stale tokens.
