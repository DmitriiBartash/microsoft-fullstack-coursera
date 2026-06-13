# Implementing Token-Based Authentication in an ASP.NET Application

**Course 8 — Security and Authentication** · Module 1 · Lesson 4 · `You Try It!`

> Build a small .NET console app that implements a simple **token-based
> authentication** flow. The point of the lab is *modularization*: user data, token
> generation, registration/login, and secure-content access each live in their own
> single-responsibility class, and `Program.cs` just wires them together.

---

## 🎯 Objective

By the end of this lab you will implement a simple token-based authentication system by
breaking the application into **distinct classes**. Each class keeps minimal functionality
so the code stays simple yet **modular** — easier to understand, test, and reuse.

---

## 🗂️ What you will build

A console project named **`SimpleTokenAuthApp`** made of five files:

| File                      | Responsibility                                                  |
| ------------------------- | --------------------------------------------------------------- |
| `User.cs`                 | Hold user data: `Username`, `Password`, `Token`                 |
| `TokenManager.cs`         | Generate a Base64-encoded token via `GenerateToken`             |
| `AuthManager.cs`          | `Register` and `Login` users, assigning tokens on login         |
| `SecureContentManager.cs` | `ValidateToken` and grant or deny access to protected content   |
| `Program.cs`              | Orchestrate: register → login → access secure content           |

**Flow:** `Register  →  Login  →  GenerateToken  →  token  →  ValidateToken  →  secure content`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code
- Basic C# familiarity (classes, properties, methods)

---

## 🛠️ Steps

### Step 1 — Prepare the application

You will create a console application to implement a basic token-based authentication
system, with separate classes for user management, token generation, and secure content
access. Scaffold the project and move into it:

```bash
dotnet new console -o SimpleTokenAuthApp
cd SimpleTokenAuthApp
```

- Open `Program.cs` and **clear any existing code** — all main logic will be moved into
  separate files.

### Step 2 — Create a `User` class

Define a simple `User` class to store user data.

- Add a new file **`User.cs`**.
- Give it three properties: `Username` (string), `Password` (string), and `Token`
  (string, for simplicity).

```csharp
namespace SimpleTokenAuthApp;

// Holds a single user's credentials and their currently issued token.
public class User
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}
```

### Step 3 — Implement token management

Create a class to handle token generation.

- Add a new file **`TokenManager.cs`**.
- Implement a `GenerateToken` method that creates a **Base64-encoded string** as a token.

```csharp
using System.Text;

namespace SimpleTokenAuthApp;

// Produces a simple Base64-encoded token for a username.
// (For learning only — a real system would issue a signed JWT.)
public class TokenManager
{
    public string GenerateToken(string username)
    {
        // Combine the username with a unique value so each token is distinct.
        string raw = $"{username}:{Guid.NewGuid()}";
        byte[] bytes = Encoding.UTF8.GetBytes(raw);
        return Convert.ToBase64String(bytes);
    }
}
```

### Step 4 — Manage user registration and login

Create a class to handle user registration and login.

- Add a new file **`AuthManager.cs`**.
- Implement `Register` and `Login` methods to manage users and assign tokens.

```csharp
namespace SimpleTokenAuthApp;

// In-memory user store handling registration and login.
public class AuthManager
{
    private readonly List<User> _users = new();
    private readonly TokenManager _tokenManager = new();

    // Registers a new user; rejects duplicate usernames.
    public bool Register(string username, string password)
    {
        if (_users.Any(u => u.Username == username))
        {
            Console.WriteLine($"Registration failed: '{username}' already exists.");
            return false;
        }

        _users.Add(new User { Username = username, Password = password });
        Console.WriteLine($"User '{username}' registered successfully.");
        return true;
    }

    // Validates credentials and, on success, issues and returns a token.
    public string? Login(string username, string password)
    {
        User? user = _users.FirstOrDefault(u => u.Username == username && u.Password == password);
        if (user is null)
        {
            Console.WriteLine("Login failed: invalid username or password.");
            return null;
        }

        user.Token = _tokenManager.GenerateToken(username);
        Console.WriteLine($"User '{username}' logged in. Token issued.");
        return user.Token;
    }
}
```

### Step 5 — Access secure content

Create a class to handle secure content access.

- Add a new file **`SecureContentManager.cs`**.
- Implement a `ValidateToken` method to check tokens and **allow or deny** access to content.

```csharp
namespace SimpleTokenAuthApp;

// Guards protected content behind a valid, non-empty token.
public class SecureContentManager
{
    public void ValidateToken(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            Console.WriteLine("Access denied: no valid token provided.");
            return;
        }

        Console.WriteLine("Access granted. Secure content: Welcome to the protected area!");
    }
}
```

### Step 6 — Run the application

Write the main program in `Program.cs` to test the application end to end:
register a user, log in to obtain a token, then use that token to access secure content.

```csharp
using SimpleTokenAuthApp;

var auth = new AuthManager();
var secureContent = new SecureContentManager();

// 1. Register a user.
auth.Register("alice", "p@ssw0rd");

// 2. Log in to obtain a token.
string? token = auth.Login("alice", "p@ssw0rd");

// 3. Use the token to access protected content.
secureContent.ValidateToken(token);

// 4. Demonstrate that a bad/empty token is rejected.
secureContent.ValidateToken(null);
```

Run it:

```bash
dotnet run
```

---

## ▶️ Expected result

The console reports each stage — registration succeeds, login issues a token, the valid
token **grants** access to the secure content, and the empty token is **denied**:

```text
User 'alice' registered successfully.
User 'alice' logged in. Token issued.
Access granted. Secure content: Welcome to the protected area!
Access denied: no valid token provided.
```

---

## ☑️ Definition of done

- [ ] `SimpleTokenAuthApp` console project created and `Program.cs` cleared
- [ ] `User.cs` defines `Username`, `Password`, and `Token` properties
- [ ] `TokenManager.cs` generates a **Base64-encoded** token in `GenerateToken`
- [ ] `AuthManager.cs` implements `Register` and `Login`, assigning a token on login
- [ ] `SecureContentManager.cs` implements `ValidateToken` to allow or deny access
- [ ] `Program.cs` runs register → login → access, and `dotnet run` prints the expected output

---

## 🔑 Key concepts

- **Separation of concerns** — user data, token generation, authentication, and content
  access each live in their own class, so each has a single reason to change and can be
  tested in isolation.
- **Token-based flow** — a successful login *issues* a token; later requests present that
  token instead of credentials, and the server decides access based on the token alone.
- **Validate before granting access** — `ValidateToken` is the gatekeeper; protected
  content is served only when a valid, non-empty token is presented (fail closed).
- **From toy to production** — this lab uses a Base64 string for clarity, but the same
  shape (issue on login → validate on access) scales up to **signed JWTs** and ASP.NET
  Core Identity in a real application.
