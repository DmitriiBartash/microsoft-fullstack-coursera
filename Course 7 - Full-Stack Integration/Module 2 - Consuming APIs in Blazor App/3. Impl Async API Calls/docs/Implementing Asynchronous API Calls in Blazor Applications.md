# Implementing Asynchronous API Calls in Blazor Applications

**Course 7 — Full-Stack Integration** · Module 2 · Lesson 3 · `You Try It!`

> Build a Blazor app that **fetches users from a public REST API** without blocking the
> UI. You'll wire up `HttpClient`, model the JSON, call the API with `async`/`await`,
> show a **loading indicator** while data is in flight, **handle errors** gracefully,
> and render the results in a live-updating table.

---

## 🎯 Objective

By the end of this activity, you will be able to manage **asynchronous API calls** in a
Blazor application — fetch data with `Task`/`await`, display **loading states**, **handle
errors** gracefully, and render the fetched data in a table that updates on demand.

---

## 🗂️ What you'll build

A Blazor project named **`AsyncApi`** that calls the public
[JSONPlaceholder](https://jsonplaceholder.typicode.com/users) API:

| File                 | Responsibility                                                        |
| -------------------- | -------------------------------------------------------------------- |
| `Program.cs`         | Register `HttpClient` in DI so components can request it             |
| `Models/User.cs`     | Strongly-typed shape of the JSON returned by the API                |
| `Pages/FetchUsers.razor` | The page: button, loading indicator, error message, results table |

**Flow:** `Click → FetchUsers() → HttpClient.GetFromJsonAsync → List<User> → table re-renders`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code
- Internet access (the lab calls a public API at `https://jsonplaceholder.typicode.com`)

---

## 🛠️ Steps

### Step 1 — Configure a Blazor application in VS Code

Set up the Blazor application as the foundation for this activity.

- Open Visual Studio Code and navigate to the folder where you keep your projects.
- Open the terminal in VS Code with `Ctrl + ~`.
- Create the front-end project:

```bash
dotnet new blazor -n AsyncApi
```

This creates a Blazor project in a folder named `AsyncApi`.

- Open the project in VS Code:

```bash
code AsyncApi
```

- Open `Program.cs` and **register `HttpClient`** so components can inject it:

```csharp
builder.Services.AddHttpClient();
```

- Confirm the app runs:

```bash
dotnet run
```

This serves the app and opens it in a browser — verify the default Blazor app loads.

- Create a new Razor component named **`FetchUsers.razor`** in the `Pages` folder (you'll
  fill it in over the next steps).

### Step 2 — Define the `User` class

Create a class that defines the structure of the user data fetched from the API. The
JSONPlaceholder payload uses lower-case JSON names and a nested `address` object, so map
those with `[JsonPropertyName]`.

- Add a folder named **`Models`** in the root of the project.
- Create a file named **`User.cs`** inside it:

```csharp
// Models/User.cs
using System.Text.Json.Serialization;

namespace AsyncApi.Models;

public class User
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("address")]
    public Address? Address { get; set; }
}

public class Address
{
    [JsonPropertyName("street")]
    public string Street { get; set; } = string.Empty;

    [JsonPropertyName("city")]
    public string City { get; set; } = string.Empty;
}
```

### Step 3 — Implement asynchronous API calls

Fetch user data using asynchronous methods. In `FetchUsers.razor`, inject the
`HttpClient` service and create a method that retrieves data from
`https://jsonplaceholder.typicode.com/users` with `Task` and `await`, storing the result
in a `List<User>`.

```razor
@page "/fetchusers"
@using AsyncApi.Models
@using System.Net.Http.Json
@inject HttpClient Http

<h3>Users</h3>

@code {
    private List<User>? users;

    private async Task FetchUsersAsync()
    {
        users = await Http.GetFromJsonAsync<List<User>>(
            "https://jsonplaceholder.typicode.com/users");
    }
}
```

> `GetFromJsonAsync<T>` issues the GET request, awaits the response, and deserializes the
> JSON body straight into your `List<User>` — all without blocking the UI thread.

### Step 4 — Add loading and error states

Handle the loading state and errors gracefully during the API call. Add a `bool` to track
loading and a `string?` to hold any error message, then wrap the call in `try/catch/finally`.

```razor
@code {
    private List<User>? users;
    private bool isLoading = false;
    private string? errorMessage;

    private async Task FetchUsersAsync()
    {
        isLoading = true;
        errorMessage = null;

        try
        {
            users = await Http.GetFromJsonAsync<List<User>>(
                "https://jsonplaceholder.typicode.com/users");
        }
        catch (Exception ex)
        {
            errorMessage = $"Failed to load users: {ex.Message}";
        }
        finally
        {
            isLoading = false;
        }
    }
}
```

- Setting `isLoading = true` before the `await` lets the markup show a spinner/message.
- `catch` captures any network or deserialization failure into `errorMessage`.
- `finally` guarantees the loading flag is cleared whether the call succeeds or fails.

### Step 5 — Display retrieved data in a table

Add a button labeled **Fetch New Users** to trigger the call, then render the data in a
table that updates dynamically. Conditionally show the loading indicator, the error
message, or the results.

```razor
@page "/fetchusers"
@using AsyncApi.Models
@using System.Net.Http.Json
@inject HttpClient Http

<h3>Users</h3>

<button class="btn btn-primary" @onclick="FetchUsersAsync" disabled="@isLoading">
    Fetch New Users
</button>

@if (isLoading)
{
    <p><em>Loading users…</em></p>
}
else if (errorMessage is not null)
{
    <p class="text-danger">@errorMessage</p>
}
else if (users is not null)
{
    <table class="table">
        <thead>
            <tr>
                <th>Id</th>
                <th>Name</th>
                <th>Email</th>
                <th>City</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var user in users)
            {
                <tr>
                    <td>@user.Id</td>
                    <td>@user.Name</td>
                    <td>@user.Email</td>
                    <td>@user.Address?.City</td>
                </tr>
            }
        </tbody>
    </table>
}

@code {
    private List<User>? users;
    private bool isLoading = false;
    private string? errorMessage;

    private async Task FetchUsersAsync()
    {
        isLoading = true;
        errorMessage = null;

        try
        {
            users = await Http.GetFromJsonAsync<List<User>>(
                "https://jsonplaceholder.typicode.com/users");
        }
        catch (Exception ex)
        {
            errorMessage = $"Failed to load users: {ex.Message}";
        }
        finally
        {
            isLoading = false;
        }
    }
}
```

> Because Blazor re-renders after each `await` completes inside an event handler, the
> table refreshes automatically every time you click the button — no manual
> `StateHasChanged()` needed here.

### Step 6 — Test your application

Verify the application works as expected for all scenarios.

```bash
dotnet run
```

- Navigate to `http://localhost:5000/fetchusers` in your browser.
- Click the **Fetch New Users** button:
  - While fetching, a **loading message** is displayed.
  - On success, a **table** of user data is displayed.
  - If an error occurs, an **error message** is displayed.

---

## ▶️ Expected result

Clicking **Fetch New Users** briefly shows "Loading users…", then renders a table of users
(Id, Name, Email, City) pulled live from JSONPlaceholder. If the network is unavailable,
a red error message appears instead — and the button is never permanently stuck disabled.

---

## ☑️ Definition of done

- [ ] `AsyncApi` Blazor project created and `HttpClient` registered in `Program.cs`
- [ ] `Models/User.cs` maps the API JSON (`Id`, `Name`, `Email`, `Address`)
- [ ] `FetchUsers.razor` fetches data with `async`/`await` via `GetFromJsonAsync`
- [ ] A `bool` loading flag and a `string?` error message drive the UI
- [ ] A **Fetch New Users** button triggers the call and the table updates dynamically
- [ ] Loading, success (table), and error states all render correctly at `/fetchusers`

---

## 🔑 Key concepts

- **Async over blocking** — `Task` + `await` keep the UI responsive while the HTTP request
  is in flight; the thread is never blocked waiting on the network.
- **Typed deserialization** — `GetFromJsonAsync<List<User>>` maps JSON straight onto C#
  models, and `[JsonPropertyName]` bridges the API's lower-case names to your properties.
- **DI for `HttpClient`** — registering it once in `Program.cs` and injecting it keeps
  components free of manual lifetime management.
- **Loading & error UX** — a loading flag and an error string (set in `try/catch/finally`)
  let the same component render three distinct states cleanly.
- **Automatic re-render** — Blazor calls `StateHasChanged` after an awaited event handler,
  so the table reflects new data without extra plumbing.
