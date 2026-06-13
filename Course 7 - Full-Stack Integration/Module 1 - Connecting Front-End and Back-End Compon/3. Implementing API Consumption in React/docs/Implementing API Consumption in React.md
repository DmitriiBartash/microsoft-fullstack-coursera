# Implementing API Consumption in React

**Course 7 — Full-Stack Integration** · Module 1 · Lesson 3 · `You Try It!`

> Build a small **Blazor WebAssembly** app that consumes a public RESTful API and renders
> the results in the browser. You'll register `HttpClient` for dependency injection, fetch
> JSON from `https://jsonplaceholder.typicode.com/posts` with `GetFromJsonAsync`, bind it to
> a strongly-typed `Post` model, display it in a table, and add graceful loading/error states.

---

## 🎯 Objective

Learn how to consume a RESTful API from a client-side app by wiring up `HttpClient`, calling
the endpoint asynchronously inside a component's lifecycle, **deserializing JSON into a typed
model**, rendering the data in a user-friendly table, and handling failures with `try/catch`
and conditional UI.

---

## 🗂️ What you'll build

A Blazor WebAssembly project named **`ApiDemo`** with a data component and a model:

| File              | Responsibility                                                      |
| ----------------- | ------------------------------------------------------------------ |
| `Program.cs`      | Register `HttpClient` for dependency injection                     |
| `Post.cs`         | Strongly-typed model (`Id`, `Title`, `Body`) for the API response  |
| `FetchData.razor` | Inject `HttpClient`, call the API, and render the data in a table  |

**Flow:** `OnInitializedAsync → HttpClient.GetFromJsonAsync(url) → Post[] → foreach → <table> rows`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code (with the C# / C# Dev Kit extension)
- A network connection to reach `https://jsonplaceholder.typicode.com`

---

## 🛠️ Steps

### Step 1 — Prepare the application

Create a new Blazor WebAssembly app, move into it, and confirm the default app runs.

```bash
dotnet new blazorwasm -n ApiDemo
cd ApiDemo
dotnet run
```

- Open the served URL in your browser and ensure the default app loads successfully.
- Stop the app with `Ctrl+C` once you've confirmed it runs.

### Step 2 — Set up API consumption with `HttpClient`

Ensure `HttpClient` is registered for dependency injection. In a Blazor WebAssembly project
the template already does this in `Program.cs` — confirm the registration is present:

```csharp
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ApiDemo;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Register HttpClient for dependency injection
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

await builder.Build().RunAsync();
```

Next, define the data model so the JSON can be deserialized into typed objects. Create a new
file **`Post.cs`** with properties that match the API structure (`Id`, `Title`, `Body`):

```csharp
namespace ApiDemo;

public class Post
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Body { get; set; }
}
```

### Step 3 — Make the API call

Create a new Razor component **`Pages/FetchData.razor`**. Inject `HttpClient` with the
`@inject` directive, declare a `Posts` variable to hold the response, and fetch the data in
`OnInitializedAsync` using `GetFromJsonAsync`:

```razor
@page "/fetchdata"
@inject HttpClient Http

<h3>Posts from API</h3>

@code {
    private Post[]? Posts;

    protected override async Task OnInitializedAsync()
    {
        Posts = await Http.GetFromJsonAsync<Post[]>(
            "https://jsonplaceholder.typicode.com/posts");
    }
}
```

- `GetFromJsonAsync<Post[]>` sends the GET request **and** deserializes the JSON response
  directly into the `Post[]` array declared in Step 2.

### Step 4 — Display data in a user-friendly way

Render the retrieved data inside `FetchData.razor` as an HTML `<table>`. Use a `foreach`
loop to populate one row per `Post`, with headers for **Post ID**, **Title**, and **Body**:

```razor
@page "/fetchdata"
@inject HttpClient Http

<h3>Posts from API</h3>

<table class="table">
    <thead>
        <tr>
            <th>Post ID</th>
            <th>Title</th>
            <th>Body</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var post in Posts!)
        {
            <tr>
                <td>@post.Id</td>
                <td>@post.Title</td>
                <td>@post.Body</td>
            </tr>
        }
    </tbody>
</table>
```

### Step 5 — Implement basic error handling

Make the component resilient. Wrap the API call in a `try/catch`, log any exception, set an
error flag, and use a conditional to render **"Loading..."** or **"Error fetching data"**
based on the call's status:

```razor
@page "/fetchdata"
@inject HttpClient Http

<h3>Posts from API</h3>

@if (hasError)
{
    <p><em>Error fetching data. Please try again later.</em></p>
}
else if (Posts is null)
{
    <p><em>Loading...</em></p>
}
else
{
    <table class="table">
        <thead>
            <tr>
                <th>Post ID</th>
                <th>Title</th>
                <th>Body</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var post in Posts)
            {
                <tr>
                    <td>@post.Id</td>
                    <td>@post.Title</td>
                    <td>@post.Body</td>
                </tr>
            }
        </tbody>
    </table>
}

@code {
    private Post[]? Posts;
    private bool hasError;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            Posts = await Http.GetFromJsonAsync<Post[]>(
                "https://jsonplaceholder.typicode.com/posts");
        }
        catch (Exception ex)
        {
            hasError = true;
            Console.Error.WriteLine($"API request failed: {ex.Message}");
        }
    }
}
```

Run the app again with `dotnet run` and navigate to `/fetchdata` to see it in action.

---

## ▶️ Expected result

Navigating to `/fetchdata` briefly shows **"Loading..."**, then renders a table of posts
fetched from `https://jsonplaceholder.typicode.com/posts` — each row showing the post's
**ID**, **Title**, and **Body**. If the API is unreachable, the page shows
**"Error fetching data"** instead of crashing.

---

## ☑️ Definition of done

- [ ] `ApiDemo` Blazor WebAssembly project created and the default app runs in the browser
- [ ] `HttpClient` is registered for dependency injection in `Program.cs`
- [ ] `Post.cs` model defined with `Id`, `Title`, and `Body`
- [ ] `FetchData.razor` injects `HttpClient` and calls the API in `OnInitializedAsync`
- [ ] Data is fetched with `GetFromJsonAsync` and rendered in an HTML `<table>` via `foreach`
- [ ] API call is wrapped in `try/catch`, with `Loading...` / `Error fetching data` states

---

## 🔑 Key concepts

- **Dependency injection for `HttpClient`** — registering the client once and injecting it
  with `@inject` keeps components decoupled from how the client is constructed and configured.
- **Typed deserialization** — `GetFromJsonAsync<Post[]>` fetches and maps JSON to your `Post`
  model in one call, so the rest of the component works with strongly-typed objects.
- **Async lifecycle methods** — `OnInitializedAsync` is the right hook to load data when a
  component first renders; `await` keeps the UI responsive while the request is in flight.
- **Resilient UI states** — wrapping the call in `try/catch` and branching the markup on
  loading / error / success turns transient network failures into a graceful user experience.
