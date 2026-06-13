# Using Async and Await

**Course 2 — Introduction to Programming With C#** · Module 4 · Lesson 1 · `You Try It!`

> Build a small C# console app that runs several **asynchronous** methods. You'll write
> `async` methods that `await Task.Delay` to simulate slow work (like downloading data),
> run two of them **in parallel** with `Task.WhenAll`, and add **exception handling** with
> `try`/`catch` — all inside the console project you created at the start of the course.

---

## 🎯 Objective

Learn how to use the `async` and `await` keywords in C# to keep an application responsive
while it performs time-consuming work, run multiple asynchronous tasks concurrently, and
handle errors that occur inside asynchronous methods.

---

## 🗂️ What you will build

A single-file console program in **`Program.cs`** (namespace `UsingAsyncAwait`) containing:

| Member                  | Responsibility                                                        |
| ----------------------- | --------------------------------------------------------------------- |
| `DownloadDataAsync()`   | First async method — simulates a 3-second download, wrapped in `try`/`catch` |
| `DownloadDataAsync2()`  | Second async method — simulates a 2-second download                   |
| `Main(string[] args)`   | `async` entry point — starts both tasks and `await`s `Task.WhenAll`   |

**Flow:** `Main → start DownloadDataAsync + DownloadDataAsync2 → Task.WhenAll → "All downloads completed."`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code
- The console application you created at the start of the course (or scaffold a new one with `dotnet new console`)

---

## 🛠️ Steps

> Remove any existing code in the `Program.cs` file of your console application and build
> up the code from each step in that single file.

### Step 1 — Understand asynchronous programming

Asynchronous programming lets your application start a long-running task and continue
doing other work instead of blocking until that task finishes — which keeps the program
responsive. In C# the building blocks are:

- `async` — marks a method as asynchronous so it can use `await`.
- `await` — pauses the method (without blocking the thread) until an awaited `Task` completes.
- `Task` / `Task.Delay` — represent work in progress; `Task.Delay` simulates work that takes time.

### Step 2 — Create an asynchronous method

Create a simple asynchronous method that simulates a task taking time to complete, like
downloading data.

- In `Program.cs`, create a class called `Program`.
- Inside it, create a method named `DownloadDataAsync` and mark it with the `async` keyword.
- Inside the method, use `await Task.Delay` to simulate a delay.
- Print a message before and after the delay to show when the method starts and ends.

```csharp
namespace UsingAsyncAwait
{
    class Program
    {
        // Step 2: First asynchronous method
        public async Task DownloadDataAsync()
        {
            Console.WriteLine("DownloadDataAsync started ...");
            await Task.Delay(3000); // Simulate 3-second delay
            Console.WriteLine("DownloadDataAsync completed.");
        }
    }
}
```

### Step 3 — Run an asynchronous method from `Main`

Create a `Main` method and call the async method, awaiting the simulated download delay.

- Below the `DownloadDataAsync` method, create a `Main` method (make it `async Task` so it can `await`).
- In `Main`, create an instance of the `Program` class.
- Call `DownloadDataAsync` using `await`.

```csharp
// Step 3: Main method (entry point)
static async Task Main(string[] args)
{
    Program program = new();
    await program.DownloadDataAsync();
}
```

> ▶️ To check your answer, run the Visual Studio Code console application. If you receive an
> error when you run the code, go to the reading on the next page to compare your code to the
> correct answer.

### Step 4 — Run multiple asynchronous methods in parallel

Run multiple asynchronous methods at once to see how they can execute simultaneously.

- Below the `DownloadDataAsync` method, create a second method named `DownloadDataAsync2`.
- Update `Main` to use `Task.WhenAll` to run `DownloadDataAsync` and `DownloadDataAsync2` in parallel.
- Observe how both methods run at the same time — by starting each task *before* awaiting,
  the 2-second and 3-second delays overlap instead of adding up.

```csharp
// Step 4: Second asynchronous method
public async Task DownloadDataAsync2()
{
    Console.WriteLine("DownloadDataAsync2 started...");
    await Task.Delay(2000); // Simulate 2-second delay
    Console.WriteLine("DownloadDataAsync2 completed");
}

// Step 3 + 4: Main method (entry point)
static async Task Main(string[] args)
{
    Program program = new();

    // Step 4: Run both async methods in parallel
    Task task1 = program.DownloadDataAsync();
    Task task2 = program.DownloadDataAsync2();
    await Task.WhenAll(task1, task2);

    Console.WriteLine("All downloads completed.");
}
```

> ▶️ To check your answer, run the Visual Studio Code console application. If you receive an
> error when you run the code, compare your code to the correct answer on the next page.

### Step 5 — Handle exceptions in asynchronous methods

Add error handling to an asynchronous method using a `try`/`catch` block.

- Modify `DownloadDataAsync` to wrap its body in a `try`/`catch` block.
- Simulate an error by throwing an exception inside the `try`.
- Catch the exception and display an error message.

```csharp
// Step 2 + 5: First asynchronous method with error handling
public async Task DownloadDataAsync()
{
    try
    {
        Console.WriteLine("DownloadDataAsync started ...");
        await Task.Delay(3000); // Simulate 3-second delay
        throw new Exception("Simulated download error.");
        Console.WriteLine("DownloadDataAsync completed.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error in DownloadDataAsync: {ex.Message}");
    }
}
```

#### Full program

Putting every step together, `Program.cs` looks like this:

```csharp
namespace UsingAsyncAwait
{
    class Program
    {
        // Step 2: First asynchronous method
        public async Task DownloadDataAsync()
        {
            try
            {
                Console.WriteLine("DownloadDataAsync started ...");
                await Task.Delay(3000); // Simulate 3-second delay
                throw new Exception("Simulated download error.");
                Console.WriteLine("DownloadDataAsync completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DownloadDataAsync: {ex.Message}");
            }
        }

        // Step 4: Second asynchronous method
        public async Task DownloadDataAsync2()
        {
            Console.WriteLine("DownloadDataAsync2 started...");
            await Task.Delay(2000); // Simulate 2-second delay
            Console.WriteLine("DownloadDataAsync2 completed");
        }

        // Step 3 + 4: Main method (entry point)
        static async Task Main(string[] args)
        {
            Program program = new();

            // Step 4: Run both async methods in parallel
            Task task1 = program.DownloadDataAsync();
            Task task2 = program.DownloadDataAsync2();
            await Task.WhenAll(task1, task2);

            Console.WriteLine("All downloads completed.");
        }
    }
}
```

---

## ▶️ Expected result

Run the app with `dotnet run`. Both methods start almost immediately (parallel), and
because `DownloadDataAsync2` finishes its 2-second delay before `DownloadDataAsync`
finishes its 3-second delay, the output looks like:

```text
DownloadDataAsync started ...
DownloadDataAsync2 started...
DownloadDataAsync2 completed
Error in DownloadDataAsync: Simulated download error.
All downloads completed.
```

`DownloadDataAsync` reports the simulated error (instead of crashing) because the exception
is caught inside its `try`/`catch`, and `"All downloads completed."` prints only after
`Task.WhenAll` confirms both tasks are done.

---

## ☑️ Definition of done

- [ ] Existing code cleared from `Program.cs` and a `Program` class created in namespace `UsingAsyncAwait`
- [ ] `DownloadDataAsync` is `async`, uses `await Task.Delay(3000)`, and prints start/end messages
- [ ] `Main` is `async Task`, instantiates `Program`, and awaits the async method
- [ ] `DownloadDataAsync2` added and both tasks run in parallel via `Task.WhenAll`
- [ ] `DownloadDataAsync` wraps its body in `try`/`catch` and reports the simulated exception
- [ ] `dotnet run` shows both methods overlapping and `"All downloads completed."` printed last

---

## 🔑 Key concepts

- **`async` + `await` keep work non-blocking** — `await` suspends the method until a `Task`
  completes without freezing the thread, so the application stays responsive during slow work.
- **Start tasks, then await for concurrency** — assigning the calls to `Task` variables
  *before* awaiting lets them run together; `Task.WhenAll` then waits for all of them, so a
  2s and a 3s delay overlap (~3s total) instead of adding up (~5s).
- **An async entry point** — making `Main` return `async Task` is what allows you to `await`
  directly inside it.
- **Handle exceptions where they occur** — a `try`/`catch` *inside* an async method catches
  failures from awaited work, letting one task fail gracefully without bringing down the others.
- **`Task.Delay` simulates real async I/O** — it stands in for genuinely slow operations
  (network, disk, database) so you can practice the pattern without external dependencies.
