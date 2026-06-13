# Building a RESTful API

**Course 7 — Full-Stack Integration** · Module 1 · Lesson 2 · `You Try It!`

> Build a small **RESTful API** with **.NET Minimal API** in Visual Studio Code that lets
> users manage a list of tasks. You will wire up the four CRUD operations — **Create, Read,
> Update, Delete** — over HTTP, return correct status codes, and verify every endpoint with
> **Postman**.

---

## 🎯 Objective

By the end of this lab, you will be able to build a RESTful API using **.NET Minimal API**
in a back-end environment: scaffold the project, expose `GET`/`POST`/`PUT`/`DELETE` endpoints
backed by an in-memory task list, return proper HTTP status codes, and test the API with Postman.

---

## 🗂️ What you will build

A single-file web project named **`MinimalApiDemo`** whose `Program.cs` exposes a `/tasks`
resource with full CRUD:

| Method   | Route          | Purpose                              | Success status        |
| -------- | -------------- | ------------------------------------ | --------------------- |
| `GET`    | `/tasks`       | Retrieve all tasks                   | `200 OK`              |
| `POST`   | `/tasks`       | Create a new task                    | `201 Created`         |
| `PUT`    | `/tasks/{id}`  | Update an existing task              | `200 OK` / `404`      |
| `DELETE` | `/tasks/{id}`  | Remove a task                        | `204 No Content` / `404` |

**Flow:** `HTTP request  →  Minimal API endpoint  →  in-memory List<TaskItem>  →  JSON response`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code
- [Postman](https://www.postman.com/downloads/) to exercise the endpoints

---

## 🛠️ Steps

### Step 1 — Prepare the application

Open Visual Studio Code and a new terminal, then scaffold a .NET web application and move into it.

```bash
dotnet new web -o MinimalApiDemo
cd MinimalApiDemo
```

- Open `Program.cs` and **remove any existing code** so you can start fresh.

### Step 2 — Set up the API

Define a basic Minimal API: configure the builder, create an **in-memory task list**, and add a
`GET` route that returns every task.

```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// In-memory data store
var tasks = new List<TaskItem>();

// Read: return all tasks
app.MapGet("/tasks", () => Results.Ok(tasks));

app.Run();

// Task model
record TaskItem(int Id, string Name, bool IsCompleted);
```

> A `record` gives a concise, immutable model; the `List<TaskItem>` lives in memory, so data
> resets every time the app restarts.

### Step 3 — Create the CRUD endpoints

Add the remaining endpoints so the API can **Create, Update, and Delete** tasks. Each endpoint
returns the correct HTTP status code: `201 Created`, `200 OK`, `204 No Content`, or `404 Not Found`.

```csharp
// Create: add a new task
app.MapPost("/tasks", (TaskItem task) =>
{
    tasks.Add(task);
    return Results.Created($"/tasks/{task.Id}", task);
});

// Update: replace an existing task by id
app.MapPut("/tasks/{id}", (int id, TaskItem updated) =>
{
    var index = tasks.FindIndex(t => t.Id == id);
    if (index == -1)
        return Results.NotFound($"Task {id} not found.");

    tasks[index] = updated;
    return Results.Ok(updated);
});

// Delete: remove a task by id
app.MapDelete("/tasks/{id}", (int id) =>
{
    var task = tasks.FirstOrDefault(t => t.Id == id);
    if (task is null)
        return Results.NotFound($"Task {id} not found.");

    tasks.Remove(task);
    return Results.NoContent();
});
```

Place these calls **before** `app.Run();`. Run the API with:

```bash
dotnet run
```

Note the URL the app listens on (the default is `http://localhost:5000`).

### Step 4 — Test the API with Postman

With the API running, open Postman and exercise each endpoint.

- **GET** — send a `GET` request to `http://localhost:5000/tasks` to retrieve all tasks.
- **POST** — send a `POST` request to `http://localhost:5000/tasks` with a JSON body:

  ```json
  {
    "id": 1,
    "name": "Learn .NET Minimal API",
    "isCompleted": false
  }
  ```

- **PUT** — send a `PUT` request to `http://localhost:5000/tasks/1` with the updated task body.
- **DELETE** — send a `DELETE` request to `http://localhost:5000/tasks/1` to remove the task.

---

## ▶️ Expected result

Each request returns the expected payload and status code: `GET` lists the tasks, `POST` echoes
the new task with `201 Created`, `PUT` returns the updated task (or `404` for a missing id), and
`DELETE` returns `204 No Content` (or `404`). A follow-up `GET` reflects every change — proving the
CRUD API works end-to-end.

---

## ☑️ Definition of done

- [ ] `MinimalApiDemo` web project created with `dotnet new web` and `Program.cs` cleared
- [ ] In-memory `List<TaskItem>` and a `TaskItem` model defined
- [ ] `GET /tasks` returns all tasks with `200 OK`
- [ ] `POST /tasks` adds a task and returns `201 Created`
- [ ] `PUT /tasks/{id}` updates a task, returning `200 OK` or `404 Not Found`
- [ ] `DELETE /tasks/{id}` removes a task, returning `204 No Content` or `404 Not Found`
- [ ] All four endpoints verified in Postman against `http://localhost:5000/tasks`

---

## 🔑 Key concepts

- **REST over HTTP verbs** — each CRUD operation maps to its idiomatic verb (`GET`/`POST`/`PUT`/`DELETE`)
  on a single `/tasks` resource, keeping the API predictable and uniform.
- **Minimal API** — .NET's `MapGet`/`MapPost`/`MapPut`/`MapDelete` define routes directly in `Program.cs`
  with no controllers, ideal for small, focused services.
- **Meaningful status codes** — returning `201`, `200`, `204`, and `404` (via `Results.*`) lets clients
  react correctly instead of guessing from the body.
- **In-memory state is ephemeral** — the `List<TaskItem>` resets on restart; swapping it for a database
  is the natural next step toward a production API.
- **Test the contract, not the code** — Postman drives the API exactly as a real client would, confirming
  routes, payloads, and status codes behave as designed.
